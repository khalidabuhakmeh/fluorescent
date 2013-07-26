using System.Web.Mvc;
using System.Web.Routing;
using RestfulRouting;
using Fluorescent.Tests.Controllers;

namespace Fluorescent.Tests
{
    public class TestRoutes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.Root<HomeController>(x => x.Index());
            map.Resources<BlogsController>(blogs =>
            {
                blogs.Resources<PostsController>();
            });
            map.Resources<GalleriesController>();
        }

        public static void Start(RouteCollection routes)
        {
            routes.MapRoutes<TestRoutes>();
        }
    }
}