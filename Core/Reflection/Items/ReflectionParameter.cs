namespace MEFLight.Reflection.Items
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal class ReflectionParameter : ReflectionItem
    {
        private readonly ParameterInfo _parameter;

        public ReflectionParameter(ParameterInfo parameter)
        {
            this._parameter = parameter;
        }

        public ParameterInfo UnderlyingParameter
        {
            get
            {
                return this._parameter;
            }
        }

        public override string Name
        {
            get
            {
                return this.UnderlyingParameter.Name;
            }
        }

        public override string GetDisplayName()
        {
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} (Parameter=\"{1}\")", new object[2]
            {
                (object) this.UnderlyingParameter.Member.Name,
                (object) this.UnderlyingParameter.Name
            });
        }

        public override Type ReturnType
        {
            get
            {
                return this.UnderlyingParameter.ParameterType;
            }
        }

        public override ReflectionItemType ItemType
        {
            get
            {
                return ReflectionItemType.Parameter;
            }
        }
    }
}
