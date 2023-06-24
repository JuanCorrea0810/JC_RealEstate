using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Tests.xUnit
{
    public class UrlHelperMock : IUrlHelper
    {
        public ActionContext ActionContext => throw new NotImplementedException();

        public string Action(UrlActionContext actionContext)
        {
            return "https://www.google.com";
        }

        [return: NotNullIfNotNull("contentPath")]
        public string Content(string contentPath)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalUrl([NotNullWhen(true)] string url)
        {
            throw new NotImplementedException();
        }

        public string Link(string routeName, object values)
        {
            throw new NotImplementedException();
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            throw new NotImplementedException();
        }
    }
}
