namespace MEFLight.Parts
{
    using System;
    using System.Collections.Generic;
    using Defenitions;

    public abstract class ComposablePart : IComposablePart
    {

        public abstract IEnumerable<ExportDefinition> ExportDefinitions { get; }


        public abstract IEnumerable<ImportDefinition> ImportDefinitions { get; }

        public abstract bool IsActivated { get; }

        public abstract void Activate();

        public abstract object GetActivatedInstance();

        public abstract void MapImports(Func<ImportDefinition, IEnumerable<IComposablePart>> callBack);
    }
}
