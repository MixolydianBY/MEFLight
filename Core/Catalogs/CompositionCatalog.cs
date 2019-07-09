namespace MEFLight.Catalogs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Defenitions;
    using Extensions;

    public abstract class CompositionCatalog : IEnumerable<ComposablePartDefinition>, IEnumerable
    {
        internal static readonly List<Tuple<ComposablePartDefinition, ExportDefinition>> _EmptyExportsList = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
        private volatile IQueryable<ComposablePartDefinition> _queryableParts;


        public virtual IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                if (this._queryableParts == null)
                {
                    Interlocked.CompareExchange<IQueryable<ComposablePartDefinition>>(ref this._queryableParts, this.AsQueryable<ComposablePartDefinition>(), (IQueryable<ComposablePartDefinition>)null);
                }
                return this._queryableParts;
            }
        }

        public virtual IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            List<Tuple<ComposablePartDefinition, ExportDefinition>> source = (List<Tuple<ComposablePartDefinition, ExportDefinition>>)null;
            IEnumerable<ComposablePartDefinition> candidateParts = this.GetCandidateParts(definition);
            if (candidateParts != null)
            {
                foreach (ComposablePartDefinition composablePartDefinition in candidateParts)
                {
                    Tuple<ComposablePartDefinition, ExportDefinition> singleMatch;
                    IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> multipleMatches;
                    if (composablePartDefinition.TryGetExports(definition, out singleMatch, out multipleMatches))
                        source = source.FastAppendToListAllowNulls<Tuple<ComposablePartDefinition, ExportDefinition>>(singleMatch, multipleMatches);
                }
            }
            return (IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>>)source ?? (IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>>)CompositionCatalog._EmptyExportsList;
        }

        internal virtual IEnumerable<ComposablePartDefinition> GetCandidateParts(ImportDefinition definition)
        {
            return (IEnumerable<ComposablePartDefinition>)this;
        }

        public virtual IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            IQueryable<ComposablePartDefinition> parts = this.Parts;
            if (parts == this._queryableParts)
                return Enumerable.Empty<ComposablePartDefinition>().GetEnumerator();
            return parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
