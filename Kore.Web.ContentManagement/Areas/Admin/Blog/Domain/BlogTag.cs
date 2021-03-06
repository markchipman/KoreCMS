﻿using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using Kore.Data.EntityFramework;
using Kore.Tenants.Domain;

namespace Kore.Web.ContentManagement.Areas.Admin.Blog.Domain
{
    public class BlogTag : ITenantEntity
    {
        private ICollection<BlogPostTag> posts;

        public int Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        public string UrlSlug { get; set; }

        public ICollection<BlogPostTag> Posts
        {
            get { return posts ?? (posts = new HashSet<BlogPostTag>()); }
            set { posts = value; }
        }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class TagMap : EntityTypeConfiguration<BlogTag>, IEntityTypeConfiguration
    {
        public TagMap()
        {
            ToTable(CmsConstants.Tables.BlogTags);
            HasKey(x => x.Id);
            Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            Property(x => x.UrlSlug).IsRequired().HasMaxLength(255).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}