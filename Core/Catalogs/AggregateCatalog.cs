namespace MEFLight.Catalogs
{
    using System.Collections.Generic;
    using System.Linq;
    using Collections;
    using Defenitions;

    public class AggregateCatalog : CompositionCatalog
    {
        private CompositionCatalogCollection _catalogs;

        public AggregateCatalog()
            : this((IEnumerable<CompositionCatalog>)null)
        {
        }

        public AggregateCatalog(params CompositionCatalog[] catalogs)
            : this((IEnumerable<CompositionCatalog>)catalogs)
        {
        }

        public AggregateCatalog(IEnumerable<CompositionCatalog> catalogs)
        {
            this._catalogs = new CompositionCatalogCollection(catalogs);
        }

        public ICollection<CompositionCatalog> Catalogs
        {
            get
            {
                return (ICollection<CompositionCatalog>)this._catalogs;
            }
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return this._catalogs.SelectMany((catalog => catalog)).GetEnumerator();
        }

    }
}
