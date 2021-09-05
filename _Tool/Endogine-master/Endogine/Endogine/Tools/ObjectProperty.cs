using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Endogine.Tools
{
    public class ObjectProperty
    {
        object _object;
        PropertyInfo _property;

        public ObjectProperty(object obj, string property)
        {
            this._object = obj;
            this._property = Endogine.Serialization.Access.GetPropertyInfoNoCase(obj, property);
        }

        public object Value
        {
            get { return this._property.GetValue(this._object, null); }
            set { this._property.SetValue(this._object, value, null); }
        }
    }
}
