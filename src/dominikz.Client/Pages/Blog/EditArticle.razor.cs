using dominikz.Client.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.Client.Pages.Blog;

public partial class EditArticle
{
    [Parameter] public Guid? ArticleId { get; set; }
    [Inject] internal BlogEndpoints? BlogEndpoints { get; set; }
    [Inject] internal DownloadEndpoints? DownloadEndpoints { get; set; }
    [Inject] internal NavigationManager? NavManager { get; set; }

    private EditContext? _editContext;
    private EditWithImageWrapper<EditArticleVm> _data = new();

    private bool _isEnabled;
    private bool _isDraft;
    private List<string> _tagRecommendations = new();

    protected override async Task OnInitializedAsync()
    {
        var articleLoaded = await LoadArticleIfRequired();
        if (articleLoaded == false)
        {
            _data.ViewModel.Id = Guid.NewGuid();
            _data.ViewModel.PublishDate = DateTime.UtcNow.AddDays(1);
        }

        await LoadRecommendations();
        _editContext = new EditContext(_data);
        _isEnabled = true;
    }

    private async Task LoadRecommendations()
    {
        var recommendations = await BlogEndpoints!.GetTagsByCategory(_data.ViewModel.Category);
        _tagRecommendations = recommendations
            .Except(_data.ViewModel.Tags)
            .ToList();
    }

    private async Task<bool> LoadArticleIfRequired()
    {
        if (ArticleId == null)
            return false;

        var article = await BlogEndpoints!.GetDraftById(ArticleId.Value);
        if (article == null)
            return false;

        _data.ViewModel = article;
        
        var file = await DownloadEndpoints!.Image(article.Id, ImageSizeEnum.Original);
        if (file == null)
            return true;

        _data.Images.Add(file.Value);
        return true;
    }

    private void CallIsDraftChanged(bool isDraft)
    {
        _isDraft = isDraft;
        if (_isDraft == false)
        {
            _data.ViewModel.PublishDate ??= DateTime.UtcNow.AddDays(1);
            return;
        }

        _data.ViewModel.PublishDate = null;
    }

    private async Task OnCategoryChanged(ArticleCategoryEnum category)
    {
        if (category == _data.ViewModel.Category)
            return;

        _data.ViewModel.Category = category;
        await LoadRecommendations();
    }

    private async Task OnSaveClicked()
    {
        if (_editContext == null || _editContext.Validate() == false)
            return;

        var article = ArticleId == null
            ? await BlogEndpoints!.Add(_data.ViewModel, _data.Images)
            : await BlogEndpoints!.Update(_data.ViewModel, _data.Images);

        if (article == null)
            return;

        NavManager?.NavigateTo($"/blog/{article.Id}");
    }
}