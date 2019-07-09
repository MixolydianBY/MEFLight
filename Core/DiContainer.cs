namespace MEFLight
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Catalogs;
    using Defenitions;
    using Parts;
    using Reflection;

    public class DiContainer
    {
        private readonly CompositionCatalog _catalog;
        private readonly ConcurrentDictionary<ExportDefinition, IComposablePart> _parts;

        public DiContainer(CompositionCatalog catalog)
        {
           _catalog = catalog;
            _parts = new ConcurrentDictionary<ExportDefinition, IComposablePart>();
        }

        public void LoadImports(object target)
        {
            if (target == null)
            {
                return;
            }

            var masterPart = AttributeDiscovery.Instance.CreatePart(target);
            masterPart.MapImports(FetchImport);

           _parts.Values.OrderBy(x => x, new PartsComparer()).ToList().ForEach((p) => 
           {
               p.Activate();
           });
            
            masterPart.Activate();
        }

        private IEnumerable<IComposablePart> FetchImport(ImportDefinition importDefinition)
        {
            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exportPair = _catalog.GetExports(importDefinition);

            IDictionary<ExportDefinition, ComposablePart> parts =
                exportPair.ToDictionary(x => x.Item2, y => y.Item1.CreatePart());

            List<IComposablePart> finalParts = new List<IComposablePart>();

            foreach (var pair in parts)
            {
                pair.Value.ExportDefinitions
                    .Where(x => x.IsMethod)
                    .ToList().ForEach((e) => RegisterExportedParts(e));

                if (!_parts.TryGetValue(pair.Key, out IComposablePart part))
                {
                    pair.Value.MapImports(FetchImport);
                    part = pair.Value;
                    _parts.AddOrUpdate(pair.Key, part, ((definition, part1) => part));
                }

                finalParts.Add(part);
            }

            return finalParts;
        }

        private void RegisterExportedParts(ExportDefinition definition)
        {
            var part = AttributeDiscovery.Instance.CreatePart(definition.IdentityType, definition.Member);
            _parts.AddOrUpdate(definition, part, (exportDefinition, composablePart) => part);
        }
    }
}
