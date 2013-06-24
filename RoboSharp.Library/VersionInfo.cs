using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboSharp.Library
{
    public class VersionInfo
    {
        [Key("editorversion")]
        public int EditorVersion { get; set; }

        [Key("editorbuild")]
        public int EditorBuild { get; set; }

        [Key("mapversion")]
        public int MapVersion { get; set; }

        [Key("formatversion")]
        public int FormatVersion { get; set; }

        [Key("prefab")]
        public int Prefab { get; set; }

    }
}
