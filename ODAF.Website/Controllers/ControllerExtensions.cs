using System;
using System.Web.Mvc;
using System.Linq.Expressions;
using Microsoft.Web.Mvc;

/// <summary>
/// Summary description for ControllerExtensions
/// </summary>
public static class ControllerExtensions
{
    public static string BuildUrlFromExpression<TController>(this Controller controller, Expression<Action<TController>> expression) 
        where TController:System.Web.Mvc.Controller
    {
        string action = LinkBuilder.BuildUrlFromExpression<TController>(controller.Url.RequestContext, controller.Url.RouteCollection, expression);
        // we use this instead of controller.Request.Url.Authority - which will return an internal Azure port number
        string host = controller.Request.Headers["Host"]; 
        
        return String.Format("{0}://{1}{2}", controller.Request.Url.Scheme, host, action);
    }

    public static string BuildUrlFromExpression<TController>(this ViewPage viewPage, Expression<Action<TController>> expression)
        where TController : System.Web.Mvc.Controller
    {
        string action = LinkBuilder.BuildUrlFromExpression<TController>(viewPage.Url.RequestContext, viewPage.Url.RouteCollection, expression);
        // we use this instead of controller.Request.Url.Authority - which will return an internal Azure port number
        string host = viewPage.Request.Headers["Host"];

        return String.Format("{0}://{1}{2}", viewPage.Request.Url.Scheme, host, action);
    }

}
