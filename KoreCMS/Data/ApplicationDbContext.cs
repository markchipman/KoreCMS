﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Kore.Configuration.Domain;
using Kore.Data;
using Kore.Data.EntityFramework;
using Kore.EntityFramework;
using Kore.Infrastructure;
using Kore.Localization;
using Kore.Localization.Domain;
using Kore.Security.Membership;
using Kore.Tasks.Domain;
using Kore.Web.ContentManagement;
using Kore.Web.ContentManagement.Areas.Admin.Media.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Kore.Web.ContentManagement.Areas.Admin.Widgets.Domain;
using Kore.Web.ContentManagement.Messaging.Domain;
using Kore.Web.Security.Membership.Permissions;
using KoreCMS.Data.Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using LanguageEntity = Kore.Localization.Domain.Language;
using PermissionEntity = KoreCMS.Data.Domain.Permission;

namespace KoreCMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>,
        ISupportSeed,
        IKoreDbContext,
        IKoreCmsDbContext,
        IKoreSecurityDbContext
    {
        #region Constructors

        static ApplicationDbContext()
        {
#if DEBUG
            Database.SetInitializer<ApplicationDbContext>(new DropCreateSeedDatabaseIfModelChanges<ApplicationDbContext>());
#else
            Database.SetInitializer<ApplicationDbContext>(new CreateSeedDatabaseIfNotExists<ApplicationDbContext>());
#endif
        }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        #endregion Constructors

        #region IKoreDbContext Members

        public DbSet<LanguageEntity> Languages { get; set; }

        public DbSet<LocalizableString> LocalizableStrings { get; set; }

        public DbSet<ScheduledTask> ScheduledTasks { get; set; }

        public DbSet<Setting> Settings { get; set; }

        #endregion IKoreDbContext Members

        #region IKoreCmsDbContext Members

        public DbSet<HistoricPage> HistoricPages { get; set; }

        public DbSet<MediaPart> MediaParts { get; set; }

        public DbSet<MediaPartType> MediaPartTypes { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<MessageTemplate> MessageTemplates { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<QueuedEmail> QueuedEmails { get; set; }

        public DbSet<QueuedSms> QueuedSms { get; set; }

        public DbSet<Widget> Widgets { get; set; }

        public DbSet<Zone> Zones { get; set; }

        #endregion IKoreCmsDbContext Members

        #region IKoreSecurityDbContext Members

        public DbSet<PermissionEntity> Permissions { get; set; }

        #endregion IKoreSecurityDbContext Members

        #region ISupportSeed Members

        public virtual void Seed()
        {
            InitializeMembership();
            InitializeLocalizableStrings();
        }

        #endregion ISupportSeed Members

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var configurations = EngineContext.Current.ResolveAll<IEntityTypeConfiguration>();

            foreach (dynamic configuration in configurations)
            {
                modelBuilder.Configurations.Add(configuration);
            }
        }

        private static void InitializeMembership()
        {
            var membershipService = EngineContext.Current.Resolve<IMembershipService>();

            if (membershipService == null)
            {
                return;
            }

            var adminUser = membershipService.GetUserByName("admin@test.com");

            if (adminUser == null)
            {
                membershipService.InsertUser(new KoreUser { UserName = "admin@test.com", Email = "admin@test.com" }, "Admin@123");
                adminUser = membershipService.GetUserByName("admin@test.com");
            }

            if (adminUser != null)
            {
                var administratorsRole = membershipService.GetRoleByName("Administrators");
                if (administratorsRole == null)
                {
                    membershipService.InsertRole(new KoreRole { Name = "Administrators" });
                    administratorsRole = membershipService.GetRoleByName("Administrators");
                    membershipService.AssignUserToRoles(adminUser.Id, new[] { administratorsRole.Id });
                }
            }

            if (membershipService.SupportsRolePermissions)
            {
                var permissions = membershipService.GetAllPermissions();

                if (!permissions.Any())
                {
                    var permissionProviders = EngineContext.Current.ResolveAll<IPermissionProvider>();
                    var toInsert = permissionProviders.SelectMany(x => x.GetPermissions()).Select(x => new KorePermission
                    {
                        Name = x.Name,
                        Category = x.Category,
                        Description = x.Description
                    });
                    foreach (var permission in toInsert)
                    {
                        membershipService.InsertPermission(permission);
                    }
                }
            }
        }

        private void InitializeLocalizableStrings()
        {
            var localizedStringsProviders = EngineContext.Current.ResolveAll<IDefaultLocalizableStringsProvider>();

            var toInsert = new HashSet<LocalizableString>();
            foreach (var provider in localizedStringsProviders)
            {
                foreach (var translation in provider.GetTranslations())
                {
                    foreach (var localizedString in translation.LocalizedStrings)
                    {
                        toInsert.Add(new LocalizableString
                        {
                            Id = Guid.NewGuid(),
                            CultureCode = translation.CultureCode,
                            TextKey = localizedString.Key,
                            TextValue = localizedString.Value
                        });
                    }
                }
            }
            LocalizableStrings.AddRange(toInsert);
            SaveChanges();
        }
    }
}