﻿using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Piano;

[Tags("music")]
[ApiController]
[Route("api/music")]
public class GetSong : ControllerBase
{
    private readonly IMediator _mediator;

    public GetSong(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("songs/{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetSongQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public class GetSongQuery : IRequest<SongVm?>
{
    public GetSongQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}

public class GetSongQueryHandler : IRequestHandler<GetSongQuery, SongVm?>
{
    private readonly DatabaseContext _database;

    public GetSongQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<SongVm?> Handle(GetSongQuery request, CancellationToken cancellationToken)
    {
        var song = await _database.From<Song>()
            .AsNoTracking()
            .Include(x => x.Segments)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (song is null)
            return null;

        return song.MapToVm();
    }
}