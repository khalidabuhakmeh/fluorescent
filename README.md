Fluorescent
===========

Use RouteTables to generate a JavaSciprt object with routes for client side use. Heavily influenced by [Restful Routing](http://restfulrouting.com) and works best in conjunction with it.

## Getting Started

Use NuGet to install the package.

```
    > Install-Package Fluorescent
```

## How To Use

You first need to define your structure using the fluent interface. The object you define in C# will look like the final JavaScript object, so feel free to nest (or not) your structure the way you like. Also feel free to exclude actions that are not relevant to your client.

**Currently this is designed to work with ASP.NET MVC exclusively, but could be tweaked to work with anything that uses RouteTables / RouteCollections. That includes ASP.NET WebForms and WebAPI.**

The following C# code:

```CSharp
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
})
.ToJs();
```
Will generate the following JSON object that you can put in a page or output to a file.

```JavaScript
Routes = {
    root: {
        url: '/',
        method: 'GET',
        resolve: function (params) {
            return Routes.resolveUrl(this.url, params);
        }
    },
    blogs: {
        index: {
            url: '/blogs',
            method: 'GET',
            resolve: function (params) {
                return Routes.resolveUrl(this.url, params);
            }
        },
        show: {
            url: '/blogs/:id',
            method: 'POST',
            resolve: function (params) {
                return Routes.resolveUrl(this.url, params);
            }
        },
        create: {
            url: '/blogs',
            method: 'GET',
            resolve: function (params) {
                return Routes.resolveUrl(this.url, params);
            }
        },
        update: {
            url: '/blogs/:id',
            method: 'PUT',
            resolve: function (params) {
                return Routes.resolveUrl(this.url, params);
            }
        },
        destroy: {
            url: '/blogs/:id',
            method: 'DELETE',
            resolve: function (params) {
                return Routes.resolveUrl(this.url, params);
            }
        },
        posts: {
            show: {
                url: '/blogs/:blogId/posts/:id',
                method: 'GET',
                resolve: function (params) {
                    return Routes.resolveUrl(this.url, params);
                }
            }
        }
    },
    gallery: {
        index: {
            url: '/galleries',
            method: 'GET',
            resolve: function (params) {
                return Routes.resolveUrl(this.url, params);
            }
        }
    }
};
Routes.resolveUrl = function (e, t) {
    var n = [];
    for (var r in t) {
        if (e.indexOf(r) < 0) {
            n.push(r + '=' + t[r]);
        }
    }
    var i = e.replace(/:(\w+)/g, function (n, r) {
        var i = t[r];
        if (!i) {
            throw 'missing route value for ' + r + ' in ' + e;
        }
        return i;
    });
    if (i.indexOf('/:') > 0) {
        throw 'not all route values were matched';
    }
    return n.length === 0 ? i : i + '?' + n.join('&');
};
```

Then from the client, you can access this object with the following (the example uses JQuery):

```JavaScript
var resource = Routes.root;
$.ajax({
	url : resource.url,
	method: resource.method,
	success : function () {
		console.log('this worked!');
	}
});

```

The other thing is you can resolve / add query string parameters:

```JavaScript
var blog = { id : 1, title : 'new blog' }
var resource = Routes.blogs.update;
$.ajax({
	// calling resolve will give you /blogs/1?title=new blog
	url : resource.resolve(blog),
	method: resource.method,
	success : function () {
		console.log('this worked!');
	}
});

```

The resolve method on the client side it really ment to match values in the route, any other values should be sent through the proper means (form data or json data).

# Contribution

Feel free to send me a pull request if you find a bug or have solved an issue. I like tests, but I understand that some of this project is about generating JavaScript so at least describe the issue for me.

Thanks,

Khalid Abuhakmeh