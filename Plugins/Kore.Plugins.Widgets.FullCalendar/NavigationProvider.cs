﻿using Kore.Localization;
using Kore.Web.Navigation;

namespace Kore.Plugins.Widgets.FullCalendar
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
            builder.Add(T(Kore.Web.KoreWebLocalizableStrings.Plugins.Title),
                menu => menu.Add(T(LocalizableStrings.FullCalendar), "5", item => item
                    .Url("#plugins/widgets/fullcalendar")
                    //.Action("Index", "Calendar", new { area = Constants.RouteArea })
                    .IconCssClass("fa fa-calendar")
                    .Permission(FullCalendarPermissions.ReadCalendar)));
        }
    }
}