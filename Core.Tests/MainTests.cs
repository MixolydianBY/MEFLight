namespace Core.Tests
{
    using System;
    using ContainerBuilder;
    using Contracts;
    using MEFLight.Attributes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestProj;
    using ThisAssemblyTypes;

    [TestClass]
    public class MainTests
    {
        [Import]
        public Func<string, ClassWithMethod> TestMethodFunc { get; set; }

        [Import]
        public DummyExport ClassImport { get; set; }

        [Import]
        public IContractOne ContractImport { get; set; }

        [Import]
        public IContractTwo ContractImportTwo { get; set; }

        [Import(typeof(IContractTwo))]
        public IContractTwo ExplicitContract { get; set; }

        [TestMethod]
        public void ObjectsHasImportedSuccessfully()
        {
            MEFLight.Singleton.ResolveImports(this);

            Assert.IsTrue(ClassImport?.AnotherAssemblyObject != null && ClassImport.LocalImport != null);
            Assert.IsTrue(ContractImport != null);
            Assert.IsTrue(ContractImportTwo != null);
            Assert.IsTrue(ExplicitContract != null);
            Assert.IsTrue(TestMethodFunc != null);

            DelegateHasImportedSuccessfully();
        }

        private void DelegateHasImportedSuccessfully()
        {
            string testString = "Hello!";

            Assert.IsTrue(TestMethodFunc != null);

            ClassWithMethod initializedObj = TestMethodFunc.Invoke(testString);

            Assert.IsTrue(initializedObj.Data.Equals(testString));
        }
    }
}
