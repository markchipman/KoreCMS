﻿using System.Linq;
using Autofac;
using Kore.Infrastructure;

namespace Kore.Data.EntityFramework
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            var entityTypeConfigurations = typeFinder
                .FindClassesOfType(typeof(IEntityTypeConfiguration))
                .ToList();

            foreach (var configuration in entityTypeConfigurations)
            {
                builder.RegisterType(configuration).As(typeof(IEntityTypeConfiguration)).InstancePerLifetimeScope();
            }
        }

        public int Order
        {
            get { return 0; }
        }

        #endregion IDependencyRegistrar Members
    }
}