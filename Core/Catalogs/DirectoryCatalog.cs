namespace MEFLight.Catalogs
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Defenitions;

    public class DirectoryCatalog : CompositionCatalog
    {
        private readonly AggregateCatalog _catalog;

        public DirectoryCatalog(string directory)
        {
            _catalog = new AggregateCatalog();
            Initialize(directory);
        }

        public DirectoryCatalog()
            : this(null)
        {
        }

        private void Initialize(string directory)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var files = Directory.EnumerateFiles(Path.Combine(baseDirectory, directory ?? string.Empty), "*.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var asmCat = new AssemblyCatalog(file);

                    if (asmCat.Parts.ToList().Count > 0)
                    {
                        _catalog.Catalogs.Add(asmCat);
                    }
                }
                catch (ReflectionTypeLoadException e)
                {

                }
                catch (BadImageFormatException)
                {
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts => _catalog.Parts;
    }
}
