﻿using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using Kore.Data;
using Kore.Data.EntityFramework;

namespace Kore.Plugins.Watchdog.Data.Domain
{
    [DataContract]
    public class WatchdogInstance : IEntity
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Password { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class WatchdogInstanceMap : EntityTypeConfiguration<WatchdogInstance>, IEntityTypeConfiguration
    {
        public WatchdogInstanceMap()
        {
            ToTable("Kore_Maintenance_WatchdogInstances");
            HasKey(x => x.Id);
            Property(x => x.Url).HasMaxLength(255).IsRequired();
            Property(x => x.Password).HasMaxLength(255).IsRequired();
        }
    }
}