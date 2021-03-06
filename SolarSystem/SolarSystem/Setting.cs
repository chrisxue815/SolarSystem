﻿using Microsoft.Xna.Framework.Input;

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

        public bool LaunchSatellite { get; set; }
        public bool RemoveSatellite { get; set; }

        private static readonly int[] PossibleSpeeds =
            {
                -10000000, -5000000, -1000000, -500000, -100000, -50000, -10000, -1000, -1, 0,
                1, 1000, 10000, 50000, 100000, 500000, 1000000, 5000000, 10000000
            };
        private static readonly int ZeroIndex = PossibleSpeeds.Length / 2;
        private int SpeedIndex { get; set; }

        private Keys[] PreviousKeys { get; set; }

        private ButtonState PreviousLeftButton { get; set; }
        private ButtonState PreviousRightButton { get; set; }

        public Setting()
        {
            Perspective = 1;
            SpeedIndex = ZeroIndex + 1;
            Speed = PossibleSpeeds[SpeedIndex];
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
            if (!Game.IsActive) return;

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
                    case Keys.D4:
                        Perspective = 4;
                        break;
                    case Keys.D5:
                        Perspective = 5;
                        break;
                    case Keys.D6:
                        Perspective = 6;
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
                        if (Pause)
                        {
                            Pause = false;
                            SpeedIndex = ZeroIndex + 1;
                        }
                        else if (SpeedIndex <= PossibleSpeeds.Length - 2)
                        {
                            ++SpeedIndex;
                        }
                        Speed = PossibleSpeeds[SpeedIndex];
                        break;
                    case Keys.Down:
                        if (Pause)
                        {
                            Pause = false;
                            SpeedIndex = ZeroIndex - 1;
                        }
                        else if (SpeedIndex >= 1)
                        {
                            --SpeedIndex;
                        }
                        Speed = PossibleSpeeds[SpeedIndex];
                        break;
                    case Keys.Space:
                        Pause = !Pause;
                        Speed = Pause ? 0 : PossibleSpeeds[SpeedIndex];
                        break;
                }
            }

            PreviousKeys = keys;

            // left button clicked
            var leftButton = Mouse.GetState().LeftButton;
            if (PreviousLeftButton == ButtonState.Released && leftButton == ButtonState.Pressed)
            {
                LaunchSatellite = true;
            }
            else
            {
                LaunchSatellite = false;
            }
            PreviousLeftButton = leftButton;

            // right button clicked
            var rightButton = Mouse.GetState().RightButton;
            if (PreviousRightButton == ButtonState.Released && rightButton == ButtonState.Pressed)
            {
                RemoveSatellite = true;
            }
            else
            {
                RemoveSatellite = false;
            }
            PreviousRightButton = rightButton;
        }
    }
}
