using System;
using Microsoft.AspNetCore.Http;
using SaveWise.DataLayer.Sys;

namespace SaveWise.Api.Common
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => _contextAccessor.HttpContext.User?.Identity?.IsAuthenticated == true;

        public string GetUserId()
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            return _contextAccessor.HttpContext.User.Identity.Name;
        }
    }
}