using System.Web.Routing;
using RestfulRouting;
using Fluorescent.Web.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Fluorescent.Web.Routes), "Start")]

namespace Fluorescent.Web
{
    public class Routes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.DebugRoute("routedebug");
            map.Root<HomeController>(x => x.Index());
            map.Resources<BlogsController>(blogs =>
            {
                blogs.Resources<PostsController>();
            });
            map.Resources<GalleriesController>();
        }
    
        public static void Start()
        {
            var routes = RouteTable.Routes;
            routes.MapRoutes<Routes>();
        }
    }
}