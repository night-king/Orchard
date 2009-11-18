﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Compilation;
using Autofac;
using Orchard.Packages;

namespace Orchard.Environment {
    //TEMP: This will be replaced by packaging system

    public interface ICompositionStrategy {
        IEnumerable<Assembly> GetAssemblies();
        IEnumerable<Type> GetModuleTypes();
        IEnumerable<Type> GetDependencyTypes();
    }

    public class DefaultCompositionStrategy : ICompositionStrategy {
        private readonly IPackageManager _packageManager;

        public DefaultCompositionStrategy(IPackageManager packageManager) {
            _packageManager = packageManager;
        }

        public IEnumerable<Assembly> GetAssemblies() {
            return _packageManager.ActivePackages()
                .Select(entry => entry.Assembly)
                .Concat(new[]{typeof(IOrchardHost).Assembly});
            //return BuildManager.GetReferencedAssemblies().OfType<Assembly>();
        }

        public IEnumerable<Type> GetModuleTypes() {
            var types = _packageManager.ActivePackages().SelectMany(x => x.ExportedTypes);
            types = types.Concat(typeof (IOrchardHost).Assembly.GetExportedTypes());
            var nonAbstractClasses = types.Where(t => t.IsClass && !t.IsAbstract);
            var modules = nonAbstractClasses.Where(t => typeof(IModule).IsAssignableFrom(t));
            return modules;
        }

        public IEnumerable<Type> GetDependencyTypes() {
            var types = _packageManager.ActivePackages().SelectMany(x => x.ExportedTypes);
            types = types.Concat(typeof(IOrchardHost).Assembly.GetExportedTypes());
            var nonAbstractClasses = types.Where(t => t.IsClass && !t.IsAbstract);
            var modules = nonAbstractClasses.Where(t => typeof(IDependency).IsAssignableFrom(t));
            return modules;
        }
    }
}