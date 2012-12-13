namespace SolarSystem
{
    public class Satellite : GameEntity
    {
        public Earth Earth { get; set; }

        public Satellite()
        {
            Earth = Game.Earth;

            ModelName = @"Models\cube";
        }

        public override void Update(float dt)
        {
            
        }
    }
}
