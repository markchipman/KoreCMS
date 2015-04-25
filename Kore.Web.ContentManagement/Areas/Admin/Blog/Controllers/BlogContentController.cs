﻿using System.Web.Mvc;
using Kore.Data;
using Kore.Web.ContentManagement.Areas.Admin.Blog.Domain;
using Kore.Web.Mvc;

namespace Kore.Web.ContentManagement.Areas.Admin.Blog.Controllers
{
    [RouteArea("")]
    [RoutePrefix("blog")]
    public class BlogContentController : KoreController
    {
        private readonly IRepository<BlogEntry> blogRepository;

        public BlogContentController(IRepository<BlogEntry> blogRepository)
        {
            this.blogRepository = blogRepository;
        }

        [Route("")]
        public ActionResult Index()
        {
            var result = View();

            // If someone has provided a custom template (see LocationFormatProvider)
            if (result.View != null)
            {
                return result;
            }
            // Else return default template
            return View("Kore.Web.ContentManagement.Areas.Admin.Blog.Views.Blog.Index");
        }
    }
}