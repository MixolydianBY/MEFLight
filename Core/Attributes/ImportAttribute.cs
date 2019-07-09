namespace MEFLight.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ImportAttribute : Attribute, IAttributedImport
    {
        public ImportAttribute()
            : this((string)null)
        {
        }

        public ImportAttribute(Type contractType)
            : this((string)null, contractType)
        {
        }

        public ImportAttribute(string contractName)
            : this(contractName, (Type)null)
        {
        }

        public ImportAttribute(string contractName, Type contractType)
        {
            this.ContractName = contractName;
            this.ContractType = contractType;
        }

        public string ContractName { get; }

        public Type ContractType { get; }
    }
}
