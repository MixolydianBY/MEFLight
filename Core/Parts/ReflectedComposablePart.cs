namespace MEFLight.Parts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Defenitions;
    using Services;

    internal class ReflectedComposablePart : ComposablePart
    {
        private volatile object _cachedInstance;
        private MemberInfo _memberInfo;
        private object _lock = new object();
        private ReflectedComposablePartDefinition _definition;
        private volatile ConcurrentDictionary<ImportDefinition, Lazy<object>> _importValues;
        private string _reflectedPartName;

        public ReflectedComposablePart(MemberInfo concreteMember)
        {
            this._memberInfo = concreteMember;
        }

        public ReflectedComposablePart(ReflectedComposablePartDefinition definition)
        {
            this._definition = definition;
        }

        public ReflectedComposablePart(ReflectedComposablePartDefinition definition, object instance)
        {
            _cachedInstance = instance;
            _definition = definition;
        }

        protected object CachedInstance
        {
            get
            {
                lock (this._lock)
                    return this._cachedInstance;
            }
        }

        protected string PartName
        {
            get
            {
                lock (this._lock)
                    if (_memberInfo != null)               
                        return this._memberInfo.Name;
                return _definition.OrigTypeName;
            }
        }

        public ReflectedComposablePartDefinition Definition
        {
            get
            {
                return this._definition;
            }
        }

        private IDictionary<ImportDefinition, Lazy<object>> ImportValues
        {
            get
            {
                ConcurrentDictionary<ImportDefinition, Lazy<object>> dictionary = this._importValues;
                if (dictionary == null)
                {
                    lock (this._lock)
                    {
                        dictionary = this._importValues;
                        if (dictionary == null)
                        {
                            dictionary = new ConcurrentDictionary<ImportDefinition, Lazy<object>>();
                            this._importValues = dictionary;
                        }
                    }
                }
                return dictionary;
            }
        }

        public override sealed IEnumerable<ImportDefinition> ImportDefinitions
        {
            get
            {
                return this.Definition.ImportDefinitions;
            }
        }

        public override sealed IEnumerable<ExportDefinition> ExportDefinitions
        {
            get
            {
                return this.Definition.ExportDefinitions;
            }
        }

        public override bool IsActivated => _cachedInstance != null;


        public override void Activate()
        {
            IDictionary<string, Lazy<object>> valuesLazy = ImportValues.ToDictionary(
                                            x => x.Key.MemberName,
                                            v => v.Value);

            if (CachedInstance == null && this.Definition.ParameterlessConstructor)
            {
                _cachedInstance = CustomActivator.CreateInstance(Definition.GetPartType());

                var unpackValues = valuesLazy.ToDictionary(k => k.Key, v => v.Value.Value);
                CustomActivator.PopulateObjectProperties(_cachedInstance, unpackValues);
            }
            else if (this.Definition.ParameterlessConstructor)
            {
                var unpackValues = valuesLazy.ToDictionary(k => k.Key, v => v.Value.Value);
                CustomActivator.PopulateObjectProperties(_cachedInstance, unpackValues);
            }
            else
            {
                var constructor = Definition.GetConstructor();
                var requiredParameters = constructor.GetParameters();

                //Sorting imports to match constructor order
                object[] unpackValues = new SortedDictionary<string, Lazy<object>>(valuesLazy)
                    .Zip(requiredParameters, (lazyValues, required) => new { lazyValues, required })
                    .OrderBy(x => x.required.Name )
                    .Select(x => x.lazyValues.Value.Value)
                    .ToArray<object>();

               _cachedInstance = Definition.GetConstructor().Invoke(unpackValues);
            }          
        }

        public override void MapImports(Func<ImportDefinition, IEnumerable<IComposablePart>> callBack)
        {
            foreach (var definition in this.ImportDefinitions)
            {
                IEnumerable<IComposablePart> fetchResult = callBack?.Invoke(definition).ToList();

                if (!(fetchResult ?? throw new InvalidOperationException()).Any())
                {
                    throw new InvalidOperationException($"Could not found any Exports matching: {definition.ContractName}");
                }

                Func<object> activatedObject = () =>
                {
                    IEnumerable<IComposablePart> innerPart = fetchResult;

                    if (definition.IsCollection)
                    {
                        return innerPart.Select(p => ((IComposablePart)p).GetActivatedInstance());
                    }
                    else
                    {
                        return innerPart.FirstOrDefault()?.GetActivatedInstance();
                    }
                };

                Lazy<object> value = new Lazy<object>(activatedObject);
                
                ((ConcurrentDictionary<ImportDefinition, Lazy<object>>)this.ImportValues).AddOrUpdate(
                definition, value, (k, v) => value);
            }       
        }

        public override string ToString()
        {
            return _definition.OrigTypeName;
        }

        public override object GetActivatedInstance()
        {

            return this.CachedInstance;
        }
    }
}
