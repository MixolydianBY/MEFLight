namespace MEFLight.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ExportAttribute : Attribute
    {
        public ExportAttribute()
        {

        }

        public ExportAttribute(Type contractType)
            : this((string)null, contractType)
        {
        }

        public ExportAttribute(string contractName)
            : this(contractName, (Type)null)
        {
        }

        public ExportAttribute(string contractName, Type contractType)
        {
            this.ContractName = contractName;
            this.ContractType = contractType;
        }

        public Type ContractType { get; private set; }

        public string ContractName { get; private set; }
    }
}
