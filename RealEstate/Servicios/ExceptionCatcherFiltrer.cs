using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;
using System.Text;

namespace RealEstate.Servicios
{
    public class ExceptionCatcherFiltrer: ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionCatcherFiltrer> logger;

        public ExceptionCatcherFiltrer(ILogger<ExceptionCatcherFiltrer> logger)
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            var typeException = context.Exception.GetType().Name;
            var message = context.Exception.Message;
            var source = context.Exception.Source;   
            var stackTrace = context.Exception.StackTrace;
            var helpLink = context.Exception.HelpLink;
            var place = context.Exception.TargetSite.DeclaringType.FullName;
            var typeMember = context.Exception.TargetSite.MemberType.ToString();
            var ipUser = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var queryString = context.HttpContext.Request.QueryString.Value;
            var routeValues = context.RouteData.Values;
            StringBuilder rv = new StringBuilder();
            string routeValuesString = null;
            foreach ( var routeValue in routeValues ) 
            {
                rv.Append(routeValue.Key + " = " + routeValue.Value + "; ");   
            }
            if (rv.Length != 0)
            {
                routeValuesString = rv.ToString();
            }
            using (LogContext.PushProperty("TypeException", typeException))
            using (LogContext.PushProperty("Source", source))
            using (LogContext.PushProperty("StackTrace", stackTrace))
            using (LogContext.PushProperty("HelpLink", helpLink))
            using (LogContext.PushProperty("Place", place))
            using (LogContext.PushProperty("TypeMember", typeMember))
            using (LogContext.PushProperty("IP", ipUser))
            using (LogContext.PushProperty("QueryString", queryString))
            using (LogContext.PushProperty("RouteValues", routeValuesString))
            {
                logger.LogError(message);
            }
            base.OnException(context);
        }
    }
}
