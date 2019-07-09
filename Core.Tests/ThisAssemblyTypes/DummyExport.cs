namespace Core.Tests.ThisAssemblyTypes
{
    using System;
    using MEFLight.Attributes;
    using TestProj;

    [Export]
    public class DummyExport
    {
        [ImportingConstructor]
        public DummyExport(DummyImport import, IAnotherAssemblyContract contract)
        {
            LocalImport = import;
            AnotherAssemblyObject = contract;
        }

        public DummyImport LocalImport { get; set; }

        public IAnotherAssemblyContract AnotherAssemblyObject { get; set; }

        public DummyExport()
        {
            Console.WriteLine("I am initialized!");
        }
    }
}
