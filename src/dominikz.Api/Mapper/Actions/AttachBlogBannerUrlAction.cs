using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace dominikz.Api.Mapper.Actions
{
    public class AttachBlogBannerUrlAction : IMappingAction<Blogpost, VMBlogpostPreview>
    {
        private readonly IActionContextAccessor _contextAccessor;
        private readonly IUrlHelper _urlHelper;

        public AttachBlogBannerUrlAction(IActionContextAccessor contextAccessor, IUrlHelper urlHelper)
        {
            _contextAccessor = contextAccessor;
            _urlHelper = urlHelper;
        }

        public void Process(Blogpost source, VMBlogpostPreview destination, ResolutionContext context)
        {
            var scheme = _contextAccessor.ActionContext.HttpContext.Request.Scheme;
            var host = _contextAccessor.ActionContext.HttpContext.Request.Host.Value;
            var path = _urlHelper.Content($"~/assets/images/blog/{source.Banner}");
            destination.Banner = $"{scheme}://{host}{path}";
        }
    }
}
