using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProj
{
    using MEFLight.Attributes;

    public class ClassWithMethod
    {
        private string _data;

        [Export(typeof(Func<string, ClassWithMethod>))]
        public ClassWithMethod Initialize(string data)
        {
            _data = data;
            return this;
        }

        public string Data
        {
            get
            {
                if (_data != null)
                    return _data;
                throw new Exception("Value wasn't intialized");
            }
        }
    }
}
