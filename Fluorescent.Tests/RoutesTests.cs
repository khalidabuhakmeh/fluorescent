using System.Diagnostics;
using System.Web.Routing;
using FluentAssertions;
using Fluorescent.Core;
using Fluorescent.Tests.Controllers;
using Xunit;

namespace Fluorescent.Tests
{
    public class RoutesTests
    {
        public RouteCollection RouteCollection { get; set; }

        public RoutesTests()
        {
            RouteCollection = new RouteCollection();
            TestRoutes.Start(RouteCollection);
        }

        [Fact]
        public void Can_generate_json_object_using_fluent_interface_for_javascript_files()
        {
            var result =
                Routes.New("Routes", routes =>
                {
                    routes.Get("root").To("index", "home");
                    routes.Group("blogs", blogs =>
                    {
                        blogs.Get("index").To("index", "blogs");
                        blogs.Post("show").To("show", "blogs").With("id");
                        blogs.Get("create").To("create", "blogs");
                        blogs.Put("update").To("update", "blogs").With("id");
                        blogs.Delete("destroy").To("destroy", "blogs").With("id");

                        blogs.Group("posts", posts =>
                        {
                            posts.Get("show").To("show", "posts").With("blogId", "id");
                        });
                    });
                    routes.Group("gallery", gallery =>
                    {
                        gallery.Get("index").To("index", "galleries");
                    });
                });

            result.Should().NotBeNull();
            
            var json = result.ToJs(RouteCollection);

            json.Should().Contain("root");
            json.Should().Contain("blogs");
            json.Should().Contain("posts");
            json.Should().Contain("gallery");

            Debug.WriteLine(json);
        }

        [Fact]
        public void Can_generate_json_object_using_fluent_interface_wrapped_in_a_function()
        {
            var result =
                Routes.New("Routes", routes =>
                {
                    routes.Get("root").To("index", "home");
                    routes.Group("blogs", blogs =>
                    {
                        blogs.Get("index").To("index", "blogs");
                        blogs.Post("show").To("show", "blogs").With("id");
                        blogs.Get("create").To("create", "blogs");
                        blogs.Put("update").To("update", "blogs").With("id");
                        blogs.Delete("destroy").To("destroy", "blogs").With("id");

                        blogs.Group("posts", posts =>
                        {
                            posts.Get("show").To("show", "posts").With("blogId", "id");
                        });
                    });
                    routes.Group("gallery", gallery =>
                    {
                        gallery.Get("index").To("index", "galleries");
                    });
                });

            result.Should().NotBeNull();

            var json = result.ToJson(RouteCollection);

            json.Should().Contain("root");
            json.Should().Contain("blogs");
            json.Should().Contain("posts");
            json.Should().Contain("gallery");

            Debug.WriteLine(json);
        }

        [Fact]
        public void Can_connect_two_seperate_js_routes()
        {
            var root = Routes.New("Routes", cfg => { });
            var blogs = Routes.New("blogs", cfg =>
            {
                cfg.Get("index").To("index", "blogs");
            });

            root.Connect(blogs);

            var json = root.ToJs(RouteCollection);
            json.Should().Contain("blogs");

            Debug.WriteLine(json);
        }

        [Fact]
        public void Can_define_routes_using_types_and_expression()
        {
            var blogs = Routes.New("blogs", cfg =>
            {
                cfg.Get("index").To<BlogsController>(x => x.Index());
            });

            var json = blogs.ToJs(RouteCollection);
            json.Should().Contain("blogs");

            Debug.WriteLine(json);
        }

        [Fact]
        public void Can_still_resolve_a_route_that_has_controller_on_the_end()
        {
            new Node("blogs").GetControllerName("blogscontroller")
                .Should().Be("blogs");
        }

        [Fact]
        public void Can_get_controller_name_from_type_name()
        {
            new Node("blogs").GetControllerName<BlogsController>()
                .Should().Be("Blogs");
        }
    }
}
