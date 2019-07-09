namespace MEFLight.Defenitions
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using Extensions;

    public class ImportDefinition
    {
        internal static readonly string EmptyContractName = string.Empty;
        private readonly string _contractName = ImportDefinition.EmptyContractName;
        private Expression<Func<ExportDefinition, bool>> _constraint;
        private Func<ExportDefinition, bool> _compiledConstraint;
        private Type _requiredTypeIdentity;
        private string _memberName;

        public ImportDefinition()
        {

        }

        public ImportDefinition(string memberName, string contractName) 
            : this(memberName, (Type) null, contractName)
        {

        }

        public ImportDefinition(string memberName, Type contractType)
            : this(memberName, contractType, contractType.FullName)
        {

        }

        public ImportDefinition(string memberName, Type requiredTypeIdentity, string contractName)
        {
            _memberName = memberName;
            _contractName = contractName;
            _requiredTypeIdentity = requiredTypeIdentity;
        }

        public ImportDefinition(Expression<Func<ExportDefinition, bool>> constraint, string contractName) 
        {
            this._constraint = constraint;
            this._contractName = contractName;
        }

        public virtual bool IsCollection
        {
            get
            {
                if(_requiredTypeIdentity != null)
                  return typeof(IEnumerable).IsAssignableFrom(_requiredTypeIdentity);
                return false;
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

        public virtual string MemberName
        {
            get
            {
                if (this._memberName != null)
                    return this._memberName;
                throw new InvalidOperationException(); //todo: make exception
            }
        }

        public virtual Expression<Func<ExportDefinition, bool>> Constraint
        {
            get
            {
                if(_requiredTypeIdentity != null && _constraint == null)
                {
                    ParameterExpression parameter = Expression.Parameter(typeof(ExportDefinition), "exportDefinition");
                    _constraint = GlobalExtensions.CreateConstraint(_requiredTypeIdentity.FullName, parameter);

                    return _constraint;
                }
                else if (_constraint != null)
                {
                    return _constraint;
                }

                throw new InvalidOperationException();
            }
        }

        public virtual bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
        {
            if (this._compiledConstraint == null)
                this._compiledConstraint = this.Constraint.Compile();
            return this._compiledConstraint(exportDefinition);
        }

        public override string ToString()
        {
            return this.Constraint.Body.ToString();
        }
    }
}
