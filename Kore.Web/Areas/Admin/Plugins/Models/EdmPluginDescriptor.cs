﻿using Kore.Web.Plugins;

namespace Kore.Web.Areas.Admin.Plugins.Models
{
    public class EdmPluginDescriptor
    {
        public string Group { get; set; }

        public string FriendlyName { get; set; }

        public string Version { get; set; }

        public string Author { get; set; }

        public string SystemName { get; set; }

        public int DisplayOrder { get; set; }

        public bool Installed { get; set; }

        public static implicit operator EdmPluginDescriptor(PluginDescriptor other)
        {
            return new EdmPluginDescriptor
            {
                Group = other.Group,
                FriendlyName = other.FriendlyName,
                Version = other.Version,
                Author = other.Author,
                SystemName = other.SystemName,
                DisplayOrder = other.DisplayOrder,
                Installed = other.Installed
            };
        }
    }
}