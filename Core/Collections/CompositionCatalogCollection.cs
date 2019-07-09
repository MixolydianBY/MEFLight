namespace MEFLight.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Catalogs;
    using Defenitions;

    public class CompositionCatalogCollection : ICollection<CompositionCatalog>, IEnumerable<CompositionCatalog>
    {
        private readonly object _lock = new object();
        private List<CompositionCatalog> _catalogs = new List<CompositionCatalog>();

        public CompositionCatalogCollection(IEnumerable<CompositionCatalog> catalogs)
        {
            catalogs = catalogs ?? Enumerable.Empty<CompositionCatalog>();
            this._catalogs = new List<CompositionCatalog>(catalogs);
        }

        public int Count
        {
            get
            {
                return this._catalogs.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(CompositionCatalog item)
        {
            Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions = new Lazy<IEnumerable<ComposablePartDefinition>>((Func<IEnumerable<ComposablePartDefinition>>)(() => (IEnumerable<ComposablePartDefinition>)item.ToArray<ComposablePartDefinition>()), LazyThreadSafetyMode.PublicationOnly);

            lock(_lock)
            {
                this._catalogs.Add(item);
            }
        }

        public void Clear()
        {
            _catalogs.Clear();
        }

        public bool Contains(CompositionCatalog item)
        {
            return this._catalogs.Contains(item);
        }

        public void CopyTo(CompositionCatalog[] array, int arrayIndex)
        {
            this._catalogs.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CompositionCatalog> GetEnumerator()
        {
            return _catalogs.GetEnumerator();
        }

        public bool Remove(CompositionCatalog item)
        {
            if (!this._catalogs.Contains(item))
                return false;

            lock (_lock)
            {
                return _catalogs.Remove(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
