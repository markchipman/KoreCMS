﻿using System;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Kore.Localization.Domain;
using Kore.Localization.Models;
using Kore.Web.ContentManagement.Areas.Admin.Blog.Domain;
using Kore.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Messaging.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Newsletters.Controllers.Api;
using Kore.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api;
using Kore.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Sitemap.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Sitemap.Models;
using Kore.Web.Infrastructure;

namespace Kore.Web.ContentManagement.Infrastructure
{
    public class WebApiRegistrar : IWebApiRegistrar
    {
        #region IWebApiRegistrar Members

        public void Register(HttpConfiguration config)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            // Blog
            builder.EntitySet<Category>("BlogCategoryApi");
            builder.EntitySet<Post>("BlogPostApi");
            builder.EntitySet<Tag>("BlogTagApi");

            // Content Blocks
            builder.EntitySet<ContentBlock>("ContentBlockApi");
            builder.EntitySet<EntityTypeContentBlock>("EntityTypeContentBlockApi");
            builder.EntitySet<Zone>("ZoneApi");

            // Localization
            builder.EntitySet<Language>("LanguageApi");
            builder.EntitySet<LocalizableString>("LocalizableStringApi");

            // Menus
            builder.EntitySet<Menu>("MenuApi");
            builder.EntitySet<MenuItem>("MenuItemApi");

            // Messaging
            builder.EntitySet<MessageTemplate>("MessageTemplateApi");
            builder.EntitySet<QueuedEmail>("QueuedEmailApi");

            // Pages
            builder.EntitySet<Page>("PageApi");
            builder.EntitySet<PageType>("PageTypeApi");
            builder.EntitySet<PageVersion>("PageVersionApi");
            builder.EntitySet<PageTreeItem>("PageTreeApi");

            // Other
            builder.EntitySet<SitemapConfig>("XmlSitemapApi");
            builder.EntitySet<Subscriber>("SubscriberApi");

            // Action Configurations
            RegisterContentBlockODataActions(builder);
            RegisterLanguageODataActions(builder);
            RegisterLocalizableStringODataActions(builder);
            RegisterMessageTemplateODataActions(builder);
            RegisterPageVersionODataActions(builder);
            RegisterXmlSitemapODataActions(builder);

            config.Routes.MapODataRoute("OData_Kore_CMS", "odata/kore/cms", builder.GetEdmModel());
        }

        #endregion IWebApiRegistrar Members

        private static void RegisterContentBlockODataActions(ODataModelBuilder builder)
        {
            var getByPageIdAction = builder.Entity<ContentBlock>().Collection.Action("GetByPageId");
            getByPageIdAction.Parameter<Guid>("pageId");
            getByPageIdAction.ReturnsCollectionFromEntitySet<ContentBlock>("ContentBlocks");
        }

        private static void RegisterLanguageODataActions(ODataModelBuilder builder)
        {
            var resetLocalizableStringsAction = builder.Entity<Language>().Collection.Action("ResetLocalizableStrings");
            resetLocalizableStringsAction.Returns<IHttpActionResult>();
        }

        private static void RegisterLocalizableStringODataActions(ODataModelBuilder builder)
        {
            var getComparitiveTableAction = builder.Entity<LocalizableString>().Collection.Action("GetComparitiveTable");
            getComparitiveTableAction.Parameter<string>("cultureCode");
            getComparitiveTableAction.ReturnsCollection<ComparitiveLocalizableString>();

            var putComparitiveAction = builder.Entity<LocalizableString>().Collection.Action("PutComparitive");
            putComparitiveAction.Parameter<string>("cultureCode");
            putComparitiveAction.Parameter<string>("key");
            putComparitiveAction.Parameter<ComparitiveLocalizableString>("entity");
            putComparitiveAction.Returns<IHttpActionResult>();

            var deleteComparitiveAction = builder.Entity<LocalizableString>().Collection.Action("DeleteComparitive");
            deleteComparitiveAction.Parameter<string>("cultureCode");
            deleteComparitiveAction.Parameter<string>("key");
            deleteComparitiveAction.Returns<IHttpActionResult>();
        }

        private static void RegisterMessageTemplateODataActions(ODataModelBuilder builder)
        {
            var getTokensAction = builder.Entity<MessageTemplate>().Collection.Action("GetTokens");
            getTokensAction.Parameter<string>("templateName");
            getTokensAction.ReturnsCollection<string>();
        }

        private static void RegisterPageVersionODataActions(ODataModelBuilder builder)
        {
            var restoreVersionAction = builder.Entity<PageVersion>().Action("RestoreVersion");
            restoreVersionAction.Returns<IHttpActionResult>();

            var translateAction = builder.Entity<PageVersion>().Collection.Action("GetCurrentVersion");
            translateAction.Parameter<Guid>("pageId");
            translateAction.Parameter<string>("cultureCode");
            translateAction.Returns<EdmPageVersion>();
        }

        private static void RegisterXmlSitemapODataActions(ODataModelBuilder builder)
        {
            var getConfigAction = builder.Entity<SitemapConfig>().Collection.Action("GetConfig");
            getConfigAction.ReturnsCollection<SitemapConfigModel>();

            var setConfigAction = builder.Entity<SitemapConfig>().Collection.Action("SetConfig");
            setConfigAction.Parameter<int>("id");
            setConfigAction.Parameter<byte>("changeFrequency");
            setConfigAction.Parameter<float>("priority");
            setConfigAction.Returns<IHttpActionResult>();

            var generateAction = builder.Entity<SitemapConfig>().Collection.Action("Generate");
            generateAction.Returns<IHttpActionResult>();
        }
    }
}