namespace MEFLight.Defenitions
{
    using System;
    using System.Reflection;

    public class ExportDefinition
    {
        private readonly string _contractName;
        private readonly Type _identityType;
        private readonly MemberInfo _member;

        public ExportDefinition()
        {

        }

        public ExportDefinition(string contractName, MemberInfo member)
            : this((Type)null, contractName, member)
        {
        }

        public ExportDefinition(Type identityType, MemberInfo member)
            : this(identityType, null, member)
        {
        }

        public ExportDefinition(Type identityType, string contractName, MemberInfo member)
        {
            this._identityType = identityType;
            this._contractName = contractName;
            this._member = member;
        }

        public virtual MemberInfo Member
        {
            get
            {
                if (this._member != null)
                    return this._member;
                throw new InvalidOperationException(); //todo: make exception
            }
        }

        public virtual string ContractName
        {
            get
            {
                if (this._contractName != null)
                    return this._contractName;
                throw new InvalidOperationException(); //todo: make exception
            }
        }

        public virtual bool IsMethod
        {
            get
            {
                if (this._member != null)
                    return this._member.MemberType == MemberTypes.Method;
                throw new InvalidOperationException(); //todo: make exception
            }
        }

        public virtual Type IdentityType
        {
            get
            {
                if (this._identityType != null)
                    return this._identityType;
                throw new InvalidOperationException(); //todo: make exception
            }
        }
    }
}
