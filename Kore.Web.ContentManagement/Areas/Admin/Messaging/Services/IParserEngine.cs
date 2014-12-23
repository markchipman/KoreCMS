﻿using Kore.Web.ContentManagement.Areas.Admin.Messaging.Models;

namespace Kore.Web.ContentManagement.Messaging.Services
{
    public interface IParserEngine
    {
        int Priority { get; }

        string ParseTemplate(string template, ParseTemplateContext context, WorkContext workContext);
    }
}