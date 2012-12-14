using System.Collections.Generic;

namespace SolarSystem
{
    public class SatelliteManager : GameEntity
    {
        private List<Satellite> Children { get; set; }

        public SatelliteManager()
        {
            Children = new List<Satellite>();
        }

        public override void Update(float dt)
        {
            if (Game.Setting.LaunchSatellite)
            {
                var satellite = new Satellite();
                satellite.LoadContent();
                Children.Add(satellite);
            }

            if (Game.Setting.RemoveSatellite)
            {
                Children.Clear();
            }

            foreach (var child in Children)
            {
                child.Update(dt);
            }
        }

        public override void Draw(float dt)
        {
            foreach (var child in Children)
            {
                child.Draw(dt);
            }
        }
    }
}
