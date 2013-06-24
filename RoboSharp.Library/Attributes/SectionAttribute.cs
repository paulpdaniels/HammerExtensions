using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboSharp.Library
{

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class SectionAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string _sectionName;

        // This is a positional argument
        public SectionAttribute(string sectionName)
        {
            this._sectionName = sectionName;
        }

        public string SectionName
        {
            get { return _sectionName; }
        }

    }
}
