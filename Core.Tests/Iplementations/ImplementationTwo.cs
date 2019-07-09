namespace Core.Tests.Iplementations
{
    using Contracts;
    using MEFLight.Attributes;

    [Export(typeof(IContractTwo))]
    public class ImplementationTwo : IContractOne, IContractTwo
    {
    }
}
