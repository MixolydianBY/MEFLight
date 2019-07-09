namespace MEFLight.Parts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Defenitions;

    public class PartsComparer : IComparer<IComposablePart>
    {
        public int Compare(IComposablePart x, IComposablePart y)
        {
            if (x == null || y == null)
            {
                throw new InvalidOperationException();
            }

            ImportDefinition[] xImports = x.ImportDefinitions?.ToArray();
            ImportDefinition[] yImports = y.ImportDefinitions?.ToArray();
    
            if(xImports == null && yImports == null)
            {
                return 0;
            }
            else if (xImports == null && yImports != null)
            {
                return -1;
            }     
            else if(xImports != null && yImports == null)
            {
                return 1;
            }
            else if (xImports == null && yImports.Length == 0)
            {
                return -1;
            }
            else if (xImports.Length == 0 && yImports == null)
            {
                return 1;
            }

            if (xImports.Length < yImports.Length)
            {
                return -1;
            }

            if (xImports.Length > yImports.Length)
            {
                return 1;
            }

            if (xImports.Length == yImports.Length)
            {
                return 0;
            }

            return 0;
        }
    }
}
