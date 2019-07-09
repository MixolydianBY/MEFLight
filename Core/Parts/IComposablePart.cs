
namespace MEFLight.Parts
{
    using System.Collections.Generic;
    using Defenitions;

    public interface IComposablePart
    {
         IEnumerable<ExportDefinition> ExportDefinitions { get; }

         IEnumerable<ImportDefinition> ImportDefinitions { get; }

         void Activate();

         object GetActivatedInstance();
    }
}
