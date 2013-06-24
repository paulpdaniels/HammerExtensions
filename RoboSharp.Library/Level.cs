using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RoboSharp.Library
{
    public class Level
    {
        private List<Entity> _entities = new List<Entity>();

        [Section("versioninfo")]
        public VersionInfo VersionInfo { get; set; }

        [Section("viewsettings")]
        public ViewSettings ViewSettings { get; set; }

        [Section("world")]
        public World World { get; set; }

        public IEnumerable<Entity> Entities
        {
            get { return _entities; }
        }

        [Section("entity")]
        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        [Section("cordon")]
        public Cordon Cordon { get; set; }

    }
}
