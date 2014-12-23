﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Kore.Infrastructure;
using Kore.Web.Configuration;
using Kore.Web.IO;

namespace Kore.Web.Mvc.Resources
{
    public class ScriptRegister : ResourceRegister
    {
        private readonly UrlHelper urlHelper;
        private readonly WebPageBase viewPage;

        public ScriptRegister(IWorkContext workContext, WebPageBase viewPage = null)
            : base(workContext)
        {
            this.viewPage = viewPage;
            urlHelper = EngineContext.Current.Resolve<UrlHelper>();
        }

        protected override string BundleBasePath
        {
            get { return "~/bundles/js/"; }
        }

        protected override string ResourceType
        {
            get { return "text/javascript"; }
        }

        protected override string VirtualBasePath
        {
            get { return KoreWebConfigurationSection.WebInstance.Resources.Scripts.VirtualBasePath; }
        }

        //public override void IncludeBundle(string bundleName, int? order = null)
        //{
        //    if (!BundleCollection.Scripts.ContainsKey(bundleName)) return;

        //    var files = BundleCollection.Scripts[bundleName];
        //    foreach (var file in files)
        //    {
        //        var resourceEntry = Include(file);
        //        if (order.HasValue)
        //        {
        //            resourceEntry.Order = order.Value;
        //        }
        //    }
        //}

        public IDisposable AtFoot()
        {
            return new CaptureScope(viewPage, s => base.IncludeInline(s.ToHtmlString()));
        }

        public override void IncludeInline(string code, bool ignoreExists = false)
        {
            base.IncludeInline("<script type=\"text/javascript\">" + code + "</script>", ignoreExists);
        }

        //public override MvcHtmlString Render(string bundleName)
        //{
        //    if (!BundleCollection.Scripts.ContainsKey(bundleName)) return null;

        //    var files = BundleCollection.Scripts[bundleName];
        //    var sb = new StringBuilder();

        //    foreach (var file in files)
        //    {
        //        sb.AppendLine(BuildResource(string.Concat(VirtualBasePath, "/", file)));
        //    }

        //    return new MvcHtmlString(sb.ToString());
        //}

        protected override string BuildInlineResources(IEnumerable<string> resources)
        {
            return string.Format("{0}{1}{0}", System.Environment.NewLine, string.Join(System.Environment.NewLine, resources));
        }

        protected override string BuildResource(string url)
        {
            return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", urlHelper.Content(url));
        }

        private class CaptureScope : IDisposable
        {
            private readonly Action<IHtmlString> callback;
            private readonly WebPageBase viewPage;

            public CaptureScope(WebPageBase viewPage, Action<IHtmlString> callback)
            {
                this.viewPage = viewPage;
                this.callback = callback;
                viewPage.OutputStack.Push(new HtmlStringWriter());
            }

            void IDisposable.Dispose()
            {
                var writer = (HtmlStringWriter)viewPage.OutputStack.Pop();
                callback(writer);
            }
        }
    }
}