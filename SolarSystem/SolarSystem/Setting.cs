using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Setting : GameEntity
    {
        public int Perspective { get; set; }
        public bool ShowRevolutionAxis { get; set; }
        public bool ShowRotationAxis { get; set; }
        public int Speed { get; set; }

        private readonly int[] PossibleSpeeds = { 1, 1000, 10000, 100000, 1000000, 5000000, 10000000 };
        private int speedIndex;


        private Keys[] PreviousKeys { get; set; }

        public Setting()
        {
            Perspective = 1;
            ShowRevolutionAxis = false;
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
                else if (key == Keys.P)
                {
                    ShowRevolutionAxis = !ShowRevolutionAxis;
                }
                else if (key == Keys.O)
                {
                    ShowRotationAxis = !ShowRotationAxis;
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
