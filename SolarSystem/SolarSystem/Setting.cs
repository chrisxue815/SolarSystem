using Microsoft.Xna.Framework.Input;

namespace SolarSystem
{
    public class Setting : GameEntity
    {
        public int Perspective { get; set; }
        public int Speed { get; set; }
        public bool Pause { get; set; }

        public bool FullScreen { get; set; }
        public bool ShowEarthRevolutionAxis { get; set; }
        public bool ShowEarthRotationAxis { get; set; }
        public bool ShowMoonRevolutionAxis { get; set; }
        public bool ShowMoonRotationAxis { get; set; }
        public bool ShowHelp { get; set; }

        private readonly int[] PossibleSpeeds = { 1, 1000, 10000, 100000, 500000, 1000000, 5000000, 10000000 };
        private int speedIndex;

        private Keys[] PreviousKeys { get; set; }

        public Setting()
        {
            Perspective = 1;
            Speed = PossibleSpeeds[0];
            Pause = false;

            FullScreen = false;
            ShowHelp = false;
            ShowEarthRevolutionAxis = false;
            ShowEarthRotationAxis = false;
            ShowMoonRevolutionAxis = false;
            ShowMoonRotationAxis = false;
        }

        public override void Update(float dt)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                if (PreviousKeys.Contains(key)) continue;

                switch (key)
                {
                    case Keys.D1:
                        Perspective = 1;
                        break;
                    case Keys.D2:
                        Perspective = 2;
                        break;
                    case Keys.D3:
                        Perspective = 3;
                        break;
                    case Keys.H:
                        ShowHelp = !ShowHelp;
                        break;
                    case Keys.Enter:
                        var state = Keyboard.GetState();
                        if (state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt))
                            FullScreen = !FullScreen;
                        break;
                    case Keys.O:
                        ShowEarthRotationAxis = !ShowEarthRotationAxis;
                        break;
                    case Keys.P:
                        ShowEarthRevolutionAxis = !ShowEarthRevolutionAxis;
                        break;
                    case Keys.K:
                        ShowMoonRotationAxis = !ShowMoonRotationAxis;
                        break;
                    case Keys.L:
                        ShowMoonRevolutionAxis = !ShowMoonRevolutionAxis;
                        break;
                    case Keys.Up:
                        if (speedIndex <= PossibleSpeeds.Length - 2)
                        {
                            Pause = false;
                            Speed = PossibleSpeeds[++speedIndex];
                        }
                        break;
                    case Keys.Down:
                        if (speedIndex >= 1)
                        {
                            Pause = false;
                            Speed = PossibleSpeeds[--speedIndex];
                        }
                        break;
                    case Keys.Space:
                        Pause = !Pause;
                        Speed = Pause ? 0 : PossibleSpeeds[speedIndex];
                        break;
                }
            }

            PreviousKeys = keys;
        }
    }
}
