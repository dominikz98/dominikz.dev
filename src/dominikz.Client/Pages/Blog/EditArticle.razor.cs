using dominikz.Client.Wrapper;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
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
    private EditArticleWrapper _vm = new();

    private bool _isEnabled;
    private bool _isDraft;
    private List<string> _tagRecommendations = new();

    protected override async Task OnInitializedAsync()
    {
        var articleLoaded = await LoadArticleIfRequired();
        if (articleLoaded == false)
        {
            _vm.Id = Guid.NewGuid();
            _vm.PublishDate = DateTime.UtcNow.AddDays(1);   
        }

        await LoadRecommendations();
        _editContext = new EditContext(_vm);
        _isEnabled = true;
    }

    private async Task LoadRecommendations()
    {
        var recommendations = await BlogEndpoints!.GetTagsByCategory(_vm.Category);
        _tagRecommendations = recommendations
            .Except(_vm.Tags)
            .ToList();
    }

    private async Task<bool> LoadArticleIfRequired()
    {
        if (ArticleId == null)
            return false;

        var article = await BlogEndpoints!.GetDraftById(ArticleId.Value);
        if (article == null)
            return false;

        _vm = new EditArticleWrapper()
        {
            Id = _vm.Id,
            Category = _vm.Category,
            Tags = _vm.Tags,
            PublishDate = _vm.PublishDate,
            Title = _vm.Title,
            HtmlText = _vm.HtmlText
        };

        var file = await DownloadEndpoints!.Image(article.Id, ImageSizeEnum.Original);
        if (file == null)
            return true;

        _vm.Images.Add(file.Value);
        return true;
    }

    private void CallIsDraftChanged(bool isDraft)
    {
        _isDraft = isDraft;
        if (_isDraft == false)
        {
            _vm.PublishDate ??= DateTime.UtcNow.AddDays(1);
            return;
        }

        _vm.PublishDate = null;
    }

    private async Task OnCategoryChanged(ArticleCategoryEnum category)
    {
        if (category == _vm.Category)
            return;

        _vm.Category = category;
        await LoadRecommendations();
    }

    private async Task OnSaveClicked()
    {
        if (_editContext == null || _editContext.Validate() == false)
            return;

        var article = ArticleId == null
            ? await BlogEndpoints!.Add(_vm, _vm.Images)
            : await BlogEndpoints!.Update(_vm, _vm.Images);

        if (article == null)
            return;

        NavManager?.NavigateTo($"/blog/{article.Id}");
    }
}