namespace MEFLight.Attributes
{
    using System;

    public interface IAttributedImport
    {
        string ContractName { get; }

        Type ContractType { get; }
    }
}
