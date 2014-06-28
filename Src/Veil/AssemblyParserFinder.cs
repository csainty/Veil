using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Veil.Parser;

namespace Veil
{
    internal static class AssemblyParserFinder
    {
        private static Lazy<IEnumerable<ITemplateParserRegistration>> registrations = new Lazy<IEnumerable<ITemplateParserRegistration>>(() =>
        {
            return AppDomainAssemblyTypeScanner.TypesOf<ITemplateParserRegistration>().Select(x => (ITemplateParserRegistration)Activator.CreateInstance(x));
        });

        public static IEnumerable<ITemplateParserRegistration> ParserRegistrations
        {
            get { return registrations.Value; }
        }
    }

    internal static class AppDomainAssemblyTypeScanner
    {
        static AppDomainAssemblyTypeScanner()
        {
            LoadAssembliesWithVeilReferences();
        }

        private static Assembly veilAssembly = typeof(VeilEngine).Assembly;

        private static IEnumerable<Type> types;

        private static IEnumerable<Assembly> assemblies;

        private static bool referencingAssembliesLoaded;

        public static Func<Assembly, bool>[] AssembliesToScan = new Func<Assembly, bool>[]
        {
            x => x.GetReferencedAssemblies().Any(r => r.Name.StartsWith("Veil", StringComparison.OrdinalIgnoreCase))
        };

        public static IEnumerable<Type> Types
        {
            get
            {
                return types;
            }
        }

        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                return assemblies;
            }
        }

        public static void UpdateTypes()
        {
            UpdateAssemblies();

            types = (from assembly in assemblies
                     from type in assembly.SafeGetExportedTypes()
                     where !type.IsAbstract
                     select type).ToArray();
        }

        private static void UpdateAssemblies()
        {
            assemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where AssembliesToScan.Any(asm => asm(assembly))
                          where !assembly.IsDynamic
                          where !assembly.ReflectionOnly
                          select assembly).ToArray();
        }

        public static void LoadAssembliesWithVeilReferences()
        {
            if (referencingAssembliesLoaded)
            {
                return;
            }

            UpdateAssemblies();

            var existingAssemblyPaths =
                assemblies.Select(a => a.Location).ToArray();

            foreach (var directory in GetAssemblyDirectories())
            {
                var unloadedAssemblies = Directory
                    .GetFiles(directory, "*.dll")
                    .Where(f => !existingAssemblyPaths.Contains(f, StringComparer.InvariantCultureIgnoreCase)).ToArray();

                foreach (var unloadedAssembly in unloadedAssemblies)
                {
                    Assembly inspectedAssembly = null;
                    try
                    {
                        inspectedAssembly = Assembly.ReflectionOnlyLoadFrom(unloadedAssembly);
                    }
                    catch (BadImageFormatException)
                    {
                        //the assembly maybe it's not managed code
                    }

                    if (inspectedAssembly != null && inspectedAssembly.GetReferencedAssemblies().Any(r => r.Name.StartsWith("Veil", StringComparison.OrdinalIgnoreCase)))
                    {
                        try
                        {
                            Assembly.Load(inspectedAssembly.GetName());
                        }
                        catch
                        {
                        }
                    }
                }
            }

            UpdateTypes();

            referencingAssembliesLoaded = true;
        }

        public static IEnumerable<Type> TypesOf<TType>()
        {
            return Types.Where(typeof(TType).IsAssignableFrom);
        }

        private static IEnumerable<string> GetAssemblyDirectories()
        {
            var privateBinPathDirectories = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath == null
                                                ? new string[] { }
                                                : AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(';');

            foreach (var privateBinPathDirectory in privateBinPathDirectories)
            {
                if (!string.IsNullOrWhiteSpace(privateBinPathDirectory))
                {
                    yield return privateBinPathDirectory;
                }
            }

            if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe == null)
            {
                yield return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }
    }

    internal static class AppDomainAssemblyTypeScannerExtensions
    {
        public static IEnumerable<Type> NotOfType<TType>(this IEnumerable<Type> types)
        {
            return types.Where(t => !typeof(TType).IsAssignableFrom(t));
        }

        public static Type[] SafeGetExportedTypes(this Assembly assembly)
        {
            Type[] types;

            try
            {
                types = assembly.GetExportedTypes();
            }
            catch (FileNotFoundException)
            {
                types = new Type[] { };
            }
            catch (NotSupportedException)
            {
                types = new Type[] { };
            }

            return types;
        }
    }
}