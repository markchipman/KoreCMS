﻿using System;
using Kore.ComponentModel;
using Kore.Data;

namespace Kore.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public abstract class ContentBlockBase : IEntity, IContentBlock
    {
        #region IContentBlock Members
        
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public int Order { get; set; }
        
        public bool Enabled { get; set; }
        
        public abstract string Name { get; }

        [LocalizedDisplayName(KoreCmsLocalizableStrings.ContentBlocks.Model.ZoneId)]
        public Guid ZoneId { get; set; }
        
        [LocalizedDisplayName(KoreCmsLocalizableStrings.ContentBlocks.Model.CustomTemplatePath)]
        public string CustomTemplatePath { get; set; }
        
        public Guid? PageId { get; set; }

        public bool Localized { get; set; }
        
        public string CultureCode { get; set; }
        
        public Guid? RefId { get; set; }

        public abstract string DisplayTemplatePath { get; }

        public abstract string EditorTemplatePath { get; }

        #endregion IContentBlock Members

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}