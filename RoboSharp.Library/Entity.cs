using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RoboSharp.Library
{
    public class Entity
    {
        [Key("id")]
        public int Id { get; set; }

        [Key("classname")]
        public string Classname { get; set; }

        [Key("angles")]
        public string AngleString { get; set; }

        [Key("origin")]
        public string OriginString
        {
            get { return Origin.ToString(); }
            set
            {
                string[] tokens = value.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                Origin = new Vector3 { X = float.Parse(tokens[0]), Y = float.Parse(tokens[1]), Z = float.Parse(tokens[2]) };
            }

        }

        public Vector3 Origin { get; set; }

    }
}
