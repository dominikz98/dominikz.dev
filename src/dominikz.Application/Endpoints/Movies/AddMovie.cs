using dominikz.Application.Extensions;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Authorize(Policy = Policies.Media)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/movies")]
public class AddMovie : EndpointController
{
    private readonly IMediator _mediator;

    public AddMovie(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromForm] AddMovieRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class AddMovieRequest : FileUploadWrapper<EditMovieVm>, IRequest<ActionWrapper<MovieDetailVm>>
{
}

public class AddMovieRequestHandler : IRequestHandler<AddMovieRequest, ActionWrapper<MovieDetailVm>>
{
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;
    private readonly IMediator _mediator;

    public AddMovieRequestHandler(DatabaseContext database, IStorageProvider storage, IMediator mediator)
    {
        _database = database;
        _storage = storage;
        _mediator = mediator;
    }

    public async Task<ActionWrapper<MovieDetailVm>> Handle(AddMovieRequest request, CancellationToken cancellationToken)
    {
        // verify
        var expectedFilesCount = 1 + request.ViewModel.Directors.Count + request.ViewModel.Writers.Count + request.ViewModel.Stars.Count;
        if (request.Files.Count != expectedFilesCount)
            return new("Expected file count mismatch");

        // validate
        var alreadyExists = await _database.From<Movie>()
            .AnyAsync(x => EF.Functions.Like(x.Title, request.ViewModel.Title)
                           || x.Id == request.ViewModel.Id, cancellationToken);

        if (alreadyExists)
            return new("Movie already exists");

        // upload poster
        var poster = await UploadAndSaveImage(request.Files, request.ViewModel.Id, cancellationToken);
        if (poster == null)
            return new("Invalid movie poster");

        // save movie
        var toAdd = new Movie().ApplyChanges(request.ViewModel);
        await _database.AddAsync(toAdd, cancellationToken);

        // create/update and link persons
        await CrudPersons(request, cancellationToken);
        await LinkPersonsAndMovie(request, cancellationToken);

        // commit transactions
        await _storage.SaveChanges(cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        // load article detail
        var movie = await _mediator.Send(new GetMovieQuery(toAdd.Id), cancellationToken);
        if (movie is null)
            return new("Error loading movie");

        return new ActionWrapper<MovieDetailVm>(movie);
    }

    private async Task CrudPersons(AddMovieRequest request, CancellationToken cancellationToken)
    {
        var addList = new List<Person>();
        var updateList = new List<Person>();
        var allIdsList = request.ViewModel.Directors.Union(request.ViewModel.Writers)
            .Union(request.ViewModel.Stars)
            .Select(x => x.Id)
            .Distinct()
            .ToList();

        var personsList = await _database.From<Person>()
            .Where(x => allIdsList.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var director in request.ViewModel.Directors)
        {
            var person = personsList.FirstOrDefault(x => x.Id == director.Id);
            CrudPerson(person, director, PersonCategoryFlags.Director, ref addList, ref updateList);
        }


        foreach (var writer in request.ViewModel.Writers)
        {
            var person = personsList.FirstOrDefault(x => x.Id == writer.Id);
            CrudPerson(person, writer, PersonCategoryFlags.Writer, ref addList, ref updateList);
        }

        foreach (var star in request.ViewModel.Stars)
        {
            var person = personsList.FirstOrDefault(x => x.Id == star.Id);
            CrudPerson(person, star, PersonCategoryFlags.Star, ref addList, ref updateList);
        }

        _database.UpdateRange(updateList);
        await _database.AddRangeAsync(addList, cancellationToken);

        foreach (var id in allIdsList)
            await UploadAndSaveImage(request.Files, id, cancellationToken);
    }

    private void CrudPerson(Person? person, EditPersonVm vm, PersonCategoryFlags category, ref List<Person> add, ref List<Person> update)
    {
        if (person == null)
        {
            // add
            add.Add(new Person()
            {
                Id = vm.Id,
                Category = category,
                Name = vm.Name
            });
            return;
        }

        // update
        if (person.Category.HasFlag(category))
            // no update required
            return;

        person.Category = person.Category | category;
        update.Add(person);
    }

    private async Task LinkPersonsAndMovie(AddMovieRequest request, CancellationToken cancellationToken)
    {
        var mappings = new List<MoviesPersonsMapping>();

        foreach (var director in request.ViewModel.Directors)
            mappings.Add(new MoviesPersonsMapping()
            {
                MovieId = request.ViewModel.Id,
                Category = PersonCategoryFlags.Director,
                PersonId = director.Id
            });

        foreach (var writer in request.ViewModel.Writers)
            mappings.Add(new MoviesPersonsMapping()
            {
                MovieId = request.ViewModel.Id,
                Category = PersonCategoryFlags.Writer,
                PersonId = writer.Id
            });

        foreach (var star in request.ViewModel.Stars)
            mappings.Add(new MoviesPersonsMapping()
            {
                MovieId = request.ViewModel.Id,
                Category = PersonCategoryFlags.Star,
                PersonId = star.Id
            });

        await _database.AddRangeAsync(mappings, cancellationToken);
    }

    private async Task<IFormFile?> UploadAndSaveImage(List<IFormFile> files, Guid id, CancellationToken cancellationToken)
    {
        var file = files.GetFileById(id);
        if (file == null)
            return null;

        var image = file.OpenReadStream();
        image.Position = 0;

        var exists = await _storage.Exists(id, cancellationToken);
        await _storage.Upload(id, image, cancellationToken);
        if (exists)
            return file;

        await _database.AddAsync(new StorageFile()
        {
            Id = id,
            Category = FileCategoryEnum.Image,
            Extension = FileIdentifier.GetExtensionByContentType(file.ContentType)
        }, cancellationToken);

        return file;
    }
}