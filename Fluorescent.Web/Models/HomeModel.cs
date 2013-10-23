using Fluorescent.Core;

namespace Fluorescent.Web.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
            Routes = Core.Routes.New("Routes", routes =>
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
        }

        public Node Routes { get; set; }
    }
}