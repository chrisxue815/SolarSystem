using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Monitor : GameEntity
    {
        private SpriteFont Font { get; set; }
        private SpriteFont Font2 { get; set; }
        private List<string> Info { get; set; }
        private List<string> Help { get; set; }
        private List<string> Speed { get; set; }

        private Text3D Vernal { get; set; }
        private Text3D Summer { get; set; }
        private Text3D Autumnal { get; set; }
        private Text3D Winter { get; set; }

        //TODO: leap year
        private readonly int[] NumDaysOfMonths = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

        private readonly string[] NamesOfMonths =
            { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public Monitor()
        {
            const float radius = Earth.RevolutionRadius;
            Vernal = new Text3D(new Vector3(0, 0, -radius));
            Summer = new Text3D(new Vector3(-radius, 0, 0));
            Autumnal = new Text3D(new Vector3(0, 0, radius));
            Winter = new Text3D(new Vector3(radius, 0, 0));

            Info = new List<string>();
            Help = new List<string>();
            Speed = new List<string>();

            // Help info
            Help.Add("");
            Help.Add("Esc: Quit");
            Help.Add("Alt-Enter: Full screen");
            Help.Add("");
            Help.Add("Up: Speedup");
            Help.Add("Down: Slowdown");
            Help.Add("Space: Pause");
            Help.Add("");
            Help.Add("P: Earth revolution plane");
            Help.Add("O: Earth rotation   plane");
            Help.Add("L: Moon  revolution plane");
            Help.Add("K: Moon  rotation   plane");
            Help.Add("");
            Help.Add("1: Overlook");
            Help.Add("2: Axial tilt");
            Help.Add("3: Side - sunrise");
            Help.Add("4: Side - sunset");
            Help.Add("5: Arctic");
            Help.Add("6: Look up to the sky");
            Help.Add("");
            Help.Add("L-click: Launch a satellite");
            Help.Add("R-click: Remove satellites");
        }

        public override void LoadContent()
        {
            Font = Game.Content.Load<SpriteFont>(@"Fonts\font1");
            Font2 = Game.Content.Load<SpriteFont>(@"Fonts\font2");

            Vernal.Model = Game.Content.Load<Model>(@"Models\vernal");
            Summer.Model = Game.Content.Load<Model>(@"Models\summer");
            Autumnal.Model = Game.Content.Load<Model>(@"Models\autumnal");
            Winter.Model = Game.Content.Load<Model>(@"Models\winter");
        }

        /* Update date and time */
        public override void Update(float dt)
        {
            Info.Clear();

            var minsFromMidday = Game.Earth.Revolution * 1440 / Earth.RevolutionAngularSpeed + 720;
            var dayFromVernalEquinox = (int)(minsFromMidday / 1440);

            var month = 2;
            var day = 21;

            // Calculate date and time
            for (; ; )
            {
                var numDaysInMonth = NumDaysOfMonths[month];

                if (day + dayFromVernalEquinox > numDaysInMonth)
                {
                    dayFromVernalEquinox += day - numDaysInMonth;
                    day = 0;
                    month = (month + 1)%12;
                }
                else
                {
                    day += dayFromVernalEquinox;
                    break;
                }
            }

            minsFromMidday %= 1440;
            var hour = (int)(minsFromMidday / 60);
            var minute = (int)(minsFromMidday % 60);

            var date = string.Format("UTC Date: {0} {1}", NamesOfMonths[month], day);
            var time = string.Format("UTC Time: {0:D2}:{1:D2}", hour, minute);

            Info.Add(date);
            Info.Add(time);
        }

        public override void Draw(float dt)
        {
            // Draw 3D text to indicate vernal, summer, autumnal and winter equinox
            const float piOver8 = MathHelper.PiOver2/4;
            var angle = Game.Earth.Revolution;
            if (angle > 0 && angle < piOver8 || angle > piOver8 * 15)
            {
                Vernal.Draw(dt);
            }
            else if (angle > piOver8 * 3 && angle < piOver8 * 5)
            {
                Summer.Draw(dt);
            }
            else if (angle > piOver8 * 7 && angle < piOver8 * 9)
            {
                Autumnal.Draw(dt);
            }
            else if (angle > piOver8 * 11 && angle < piOver8 * 13)
            {
                Winter.Draw(dt);
            }

            // Draw date and time
            var pos = new Vector2(10, 10);
            foreach (var info in Info)
            {
                Game.SpriteBatch.DrawString(Font, info, pos, Color.White);
                pos += new Vector2(0, 20);
            }

            // Draw speed
            string relativeSpeed;
            var speed = Game.Setting.Speed;
            if ((Math.Abs(speed) / 86400) > 0)
            {
                relativeSpeed = string.Format("{0:N1} day/s", (float)speed / 86400);
            }
            else if ((Math.Abs(speed) / 3600) > 0)
            {
                relativeSpeed = string.Format("{0:N1} hour/s", (float)speed / 3600);
            }
            else if ((Math.Abs(speed) / 60) > 0)
            {
                relativeSpeed = string.Format("{0:N1} min/s", (float)speed / 60);
            }
            else
            {
                relativeSpeed = string.Format("{0:N1} s/s", (float)speed);
            }

            var runningSpeed = string.Format("Speed: x{0:N0} ({1})", Game.Setting.Speed, relativeSpeed);
            Game.SpriteBatch.DrawString(Font, runningSpeed, new Vector2(560, 10), Color.White);

            // Draw help info
            pos = new Vector2(1120, 10);
            Game.SpriteBatch.DrawString(Font, "H: Help", pos, Color.White);
            if (Game.Setting.ShowHelp)
            {
                foreach (var help in Help)
                {
                    pos += new Vector2(0, 20);
                    Game.SpriteBatch.DrawString(Font2, help, pos, Color.White);
                }
            }
        }
    }
}
