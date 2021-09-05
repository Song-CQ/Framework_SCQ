using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Endogine.Tools
{
    /// <summary>
    /// Gets the value from an object.property and sets it to another object.property
    /// </summary>
    public class Relay
    {
        ObjectProperty _from;
        ObjectProperty _to;

        public Relay(object readFromObject, string readFromProperty, object writeToObject, string writeToProperty)
        {
            this._from = new ObjectProperty(readFromObject, readFromProperty);
            this._to = new ObjectProperty(writeToObject, writeToProperty);
        }

        public void Update()
        {
            this._to.Value = this._from.Value;
        }
    }
}
