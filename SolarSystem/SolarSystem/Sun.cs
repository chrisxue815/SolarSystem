using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Sun : GameEntity
    {
        public const float Radius = 15f;

        public Sun(float x, float y, float z)
        {
            var pos = new Vector3(x, y, z);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));
            LocalTransform = Scale;

            ModelName = "sphere";

            DiffuseColor = Color.Yellow.ToVector3();
        }
    }
}
