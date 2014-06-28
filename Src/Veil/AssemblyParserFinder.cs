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

    /// <summary>
    /// Scans the app domain for assemblies and types
    /// </summary>
    internal static class AppDomainAssemblyTypeScanner
    {
        static AppDomainAssemblyTypeScanner()
        {
            LoadAssembliesWithVeilReferences();
        }

        /// <summary>
        /// Nancy core assembly
        /// </summary>
        private static Assembly veilAssembly = typeof(VeilEngine).Assembly;

        /// <summary>
        /// App domain type cache
        /// </summary>
        private static IEnumerable<Type> types;

        /// <summary>
        /// App domain assemblies cache
        /// </summary>
        private static IEnumerable<Assembly> assemblies;

        /// <summary>
        /// Indicates whether the all Assemblies, that references a Nancy assembly, have already been loaded
        /// </summary>
        private static bool referencingAssembliesLoaded;

        /// <summary>
        /// The default assemblies for scanning.
        /// Includes the nancy assembly and anything referencing a nancy assembly
        /// </summary>
        public static Func<Assembly, bool>[] AssembliesToScan = new Func<Assembly, bool>[]
        {
            x => x.GetReferencedAssemblies().Any(r => r.Name.StartsWith("Veil", StringComparison.OrdinalIgnoreCase))
        };

        /// <summary>
        /// Gets app domain types.
        /// </summary>
        public static IEnumerable<Type> Types
        {
            get
            {
                return types;
            }
        }

        /// <summary>
        /// Gets app domain types.
        /// </summary>
        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                return assemblies;
            }
        }

        /// <summary>
        /// Refreshes the type cache if additional assemblies have been loaded.
        /// Note: This is called automatically if assemblies are loaded using LoadAssemblies.
        /// </summary>
        public static void UpdateTypes()
        {
            UpdateAssemblies();

            types = (from assembly in assemblies
                     from type in assembly.SafeGetExportedTypes()
                     where !type.IsAbstract
                     select type).ToArray();
        }

        /// <summary>
        /// Updates the assembly cache from the appdomain
        /// </summary>
        private static void UpdateAssemblies()
        {
            assemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where AssembliesToScan.Any(asm => asm(assembly))
                          where !assembly.IsDynamic
                          where !assembly.ReflectionOnly
                          select assembly).ToArray();
        }

        /// <summary>
        /// Loads any assembly that references a Veil assembly.
        /// </summary>
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

        /// <summary>
        /// Gets all types implementing a particular interface/base class
        /// </summary>
        /// <typeparam name="TType">Type to search for</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of types.</returns>
        public static IEnumerable<Type> TypesOf<TType>()
        {
            return Types.Where(typeof(TType).IsAssignableFrom);
        }

        /// <summary>
        /// Returns the directories containing dll files. It uses the default convention as stated by microsoft.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.appdomainsetup.privatebinpathprobe.aspx"/>
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