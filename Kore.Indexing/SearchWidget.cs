﻿using System;
using Kore.Web.ContentManagement.Areas.Admin.Widgets;

namespace Kore.Indexing
{
    public class SearchWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Search Form Widget"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Kore.Indexing.Views.Shared.DisplayTemplates.SearchWidget"; }
        }

        public override string EditorTemplatePath
        {
            get { throw new NotSupportedException(); }
        }
    }
}