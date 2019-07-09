namespace MEFLight.Defenitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Parts;

    public abstract class ComposablePartDefinition
    {
        internal static readonly IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> _EmptyExports = Enumerable.Empty<Tuple<ComposablePartDefinition, ExportDefinition>>();
    
        public abstract IEnumerable<ExportDefinition> ExportDefinitions { get; }
      
        public abstract IEnumerable<ImportDefinition> ImportDefinitions { get; }

        public abstract  IEnumerable<MemberInfo> ExportMemberInfos { get; }

        public abstract string OrigTypeName { get; }


        internal virtual bool TryGetExports(ImportDefinition definition, out Tuple<ComposablePartDefinition, ExportDefinition> singleMatch, out IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> multipleMatches)
        {
            singleMatch = (Tuple<ComposablePartDefinition, ExportDefinition>)null;
            multipleMatches = (IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>>)null;
            List<Tuple<ComposablePartDefinition, ExportDefinition>> tupleList = (List<Tuple<ComposablePartDefinition, ExportDefinition>>)null;
            Tuple<ComposablePartDefinition, ExportDefinition> tuple = (Tuple<ComposablePartDefinition, ExportDefinition>)null;
            bool flag = false;
            foreach (ExportDefinition exportDefinition in this.ExportDefinitions)
            {
                if (definition.IsConstraintSatisfiedBy(exportDefinition))
                {
                    flag = true;
                    if (tuple == null)
                    {
                        tuple = new Tuple<ComposablePartDefinition, ExportDefinition>(this, exportDefinition);
                    }
                    else
                    {
                        if (tupleList == null)
                        {
                            tupleList = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
                            tupleList.Add(tuple);
                        }
                        tupleList.Add(new Tuple<ComposablePartDefinition, ExportDefinition>(this, exportDefinition));
                    }
                }
            }
            if (!flag)
                return false;
            if (tupleList != null)
                multipleMatches = (IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>>)tupleList;
            else
                singleMatch = tuple;
            return true;
        }

        internal abstract ComposablePart CreatePart();

        public override string ToString()
        {
            return OrigTypeName;
        }
    }
}
