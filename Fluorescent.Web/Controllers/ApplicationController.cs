using System;
using System.Web.Mvc;
using RestfulRouting.Format;

namespace Fluorescent.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
        protected ActionResult RespondTo(Action<FormatCollection> format)
        {
            return new FormatResult(format);
        }
    }

    public class BlogsController : Controller
    {
    }

    public class GalleriesController : Controller
    {
    }

    public class PostsController : Controller
    {
    }
}