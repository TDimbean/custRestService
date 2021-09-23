using System.Web.Http;

namespace CustomerRestService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var container = Unity.CreateContainer();
            //config.DependencyResolver = new UnityResolver(container);
      


       
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
