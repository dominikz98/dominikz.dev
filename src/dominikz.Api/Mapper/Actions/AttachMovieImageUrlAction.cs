﻿using AutoMapper;
using dominikz.Api.Models;
using dominikz.Endpoints.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace dominikz.Api.Mapper.Actions
{
    public class AttachMovieImageUrlAction : IMappingAction<Movie, VMMovie>
    {
        private readonly IActionContextAccessor _contextAccessor;
        private readonly IUrlHelper _urlHelper;

        public AttachMovieImageUrlAction(IActionContextAccessor contextAccessor, IUrlHelper urlHelper)
        {
            _contextAccessor = contextAccessor;
            _urlHelper = urlHelper;
        }

        public void Process(Movie source, VMMovie destination, ResolutionContext context)
        {
            var scheme = _contextAccessor.ActionContext.HttpContext.Request.Scheme;
            var host = _contextAccessor.ActionContext.HttpContext.Request.Host.Value;
            var path = _urlHelper.Content($"~/assets/images/movies/{source.Id}/");
            destination.ImagesUrl = $"{scheme}://{host}{path}";
        }
    }
}
