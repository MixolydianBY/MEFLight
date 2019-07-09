namespace MEFLight.Defenitions
{
    using System;
    using System.Linq.Expressions;
    using Extensions;

    public class ContractBasedImportDefinition : ImportDefinition
    {
        private Expression<Func<ExportDefinition, bool>> _constraint;

        protected ContractBasedImportDefinition()
        {
        }

        public ContractBasedImportDefinition(string memberName, Type contractType, string contractName) 
            : base(memberName, contractType, contractName)
        {
        }

        public override Expression<Func<ExportDefinition, bool>> Constraint
        {
            get
            {
                if (this._constraint == null)
                {
                    ParameterExpression parameter = Expression.Parameter(typeof(ExportDefinition), "exportDefinition");
                    this._constraint = GlobalExtensions.CreateConstraint(ContractName, parameter);
                }
                    
                return this._constraint;
            }
        }
    }
}
