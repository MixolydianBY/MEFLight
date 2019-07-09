namespace Core.Tests.ContainerBuilder
{
    using System;
    using global::MEFLight;
    using global::MEFLight.Catalogs;

    public class MEFLight
    {
        private static Lazy<MEFLight> _instance = new Lazy<MEFLight>(() => new MEFLight());
        private DiContainer _container;

        private MEFLight()
        {
            var catalog = new DirectoryCatalog("..\\..\\..\\Output");
             _container = new DiContainer(catalog);
        }

        public static MEFLight Singleton => _instance.Value;

        public void ResolveImports(object instance)
        {
            _container.LoadImports(instance);
        }
    }
}
