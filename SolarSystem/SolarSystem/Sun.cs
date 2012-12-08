using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Sun : GameEntity
    {
        public const float Mass = 1.9891e30f;
        //public const float Radius = 6.96e8f;  //TODO
        public const float Radius = 10f;

        public Sun(float x, float y, float z)
        {
            var pos = new Vector3(x, y, z);

            LocalTransform = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            ModelName = "sphere";
        }
    }
}
