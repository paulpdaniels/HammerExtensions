using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboSharp.Library
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class KeyAttribute : Attribute
    {

        public KeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
