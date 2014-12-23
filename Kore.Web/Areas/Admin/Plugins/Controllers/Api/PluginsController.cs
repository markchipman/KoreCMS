﻿using System;
using System.Linq;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Kore.Web.Areas.Admin.Plugins.Models;
using Kore.Web.Plugins;

namespace Kore.Web.Areas.Admin.Plugins.Controllers.Api
{
    public class PluginsController : ODataController
    {
        private readonly IPluginFinder pluginFinder;
        private readonly Lazy<IWebHelper> webHelper;

        public PluginsController(IPluginFinder pluginFinder, Lazy<IWebHelper> webHelper)
        {
            this.pluginFinder = pluginFinder;
            this.webHelper = webHelper;
        }

        // GET: odata/kore/cms/Plugins
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public virtual IQueryable<EdmPluginDescriptor> Get()
        {
            return pluginFinder.GetPluginDescriptors(false).Select(x => (EdmPluginDescriptor)x).AsQueryable();
        }
    }
}