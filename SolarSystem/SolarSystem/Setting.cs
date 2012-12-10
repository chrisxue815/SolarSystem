using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Setting : GameEntity
    {
        public int Perspective { get; set; }
        public int Speed { get; set; }
        public bool ShowEarthRevolutionAxis { get; set; }
        public bool ShowEarthRotationAxis { get; set; }
        public bool ShowMoonRevolutionAxis { get; set; }
        public bool ShowMoonRotationAxis { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowParameter { get; set; }

        private readonly int[] PossibleSpeeds = { 1, 1000, 10000, 100000, 500000, 1000000, 5000000, 10000000 };
        private int speedIndex;

        private Keys[] PreviousKeys { get; set; }

        public Setting()
        {
            Perspective = 1;
            ShowEarthRevolutionAxis = false;
            Speed = PossibleSpeeds[0];
        }

        public override void Update(float dt)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                if (PreviousKeys.Contains(key)) continue;

                if (key == Keys.D1)
                {
                    Perspective = 1;
                }
                else if (key == Keys.D2)
                {
                    Perspective = 2;
                }
                else if (key == Keys.D3)
                {
                    Perspective = 3;
                }
                else if (key == Keys.O)
                {
                    ShowEarthRotationAxis = !ShowEarthRotationAxis;
                }
                else if (key == Keys.P)
                {
                    ShowEarthRevolutionAxis = !ShowEarthRevolutionAxis;
                }
                else if (key == Keys.K)
                {
                    ShowMoonRotationAxis = !ShowMoonRotationAxis;
                }
                else if (key == Keys.L)
                {
                    ShowMoonRevolutionAxis = !ShowMoonRevolutionAxis;
                }
                else if (key == Keys.Up)
                {
                    if (speedIndex <= PossibleSpeeds.Length - 2)
                    {
                        Speed = PossibleSpeeds[++speedIndex];
                    }
                }
                else if (key == Keys.Down)
                {
                    if (speedIndex >= 1)
                    {
                        Speed = PossibleSpeeds[--speedIndex];
                    }
                }
            }

            PreviousKeys = keys;
        }
    }
}
