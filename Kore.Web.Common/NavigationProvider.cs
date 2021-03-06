﻿using Kore.Localization;
using Kore.Web.Navigation;

namespace Kore.Web.Common
{
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T(KoreWebLocalizableStrings.General.Configuration),
                menu => menu.Add(T(LocalizableStrings.Regions.Title), "5", item => item
                    .Url("#regions")
                    //.Action("Index", "Region", new { area = Constants.Areas.Regions })
                    .IconCssClass("fa fa-globe")
                    .Permission(Permissions.RegionsRead)));
        }
    }
}