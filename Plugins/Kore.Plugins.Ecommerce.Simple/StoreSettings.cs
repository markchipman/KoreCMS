﻿using System.ComponentModel.DataAnnotations;
using Kore.Web.Configuration;

namespace Kore.Plugins.Ecommerce.Simple
{
    public class StoreSettings : ISettings
    {
        public StoreSettings()
        {
            PageTitle = "Store";
            Currency = "USD $";
            CategoriesPerPage = 10;
            ProductsPerPage = 25;
            ShowOnMenus = true;
            MenuPosition = 0;
        }

        [Display(Name = "Page Title")]
        public string PageTitle { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Categories Per Page")]
        public byte CategoriesPerPage { get; set; }

        [Display(Name = "Products Per Page")]
        public byte ProductsPerPage { get; set; }

        [Display(Name = "Show on Menus")]
        public bool ShowOnMenus { get; set; }

        [Display(Name = "Menu Position")]
        public byte MenuPosition { get; set; }

        [Display(Name = "Use Ajax")]
        public bool UseAjax { get; set; }

        #region ISettings Members

        public string Name
        {
            get { return "Simple Commerce: Store Settings"; }
        }

        public string EditorTemplatePath
        {
            get { return "/Plugins/Plugins.Ecommerce.Simple/Views/Shared/EditorTemplates/StoreSettings.cshtml"; }
        }

        #endregion ISettings Members
    }
}