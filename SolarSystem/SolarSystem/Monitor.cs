using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Monitor : GameEntity
    {
        private SpriteFont Font { get; set; }
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
            Vernal = new Text3D(new Vector3(0, 20, -radius));
            Summer = new Text3D(new Vector3(-radius, 20, 0));
            Autumnal = new Text3D(new Vector3(0, 20, radius));
            Winter = new Text3D(new Vector3(radius, 20, 0));

            Info = new List<string>();
            Help = new List<string>();
            Speed = new List<string>();

            Help.Add("");
            Help.Add("Esc: Quit");
            Help.Add("Alt-Enter: Full screen");
            Help.Add("");
            Help.Add("Up: Speedup");
            Help.Add("Down: Slowdown");
            Help.Add("Space: Pause");
            Help.Add("");
            Help.Add("P: Earth revolution orbit");
            Help.Add("O: Earth rotation   orbit");
            Help.Add("L: Moon  revolution orbit");
            Help.Add("K: Moon  rotation   orbit");
            Help.Add("");
            Help.Add("1: Overlook");
            Help.Add("2: Side - sunrise");
            Help.Add("3: Side - sunset");
            Help.Add("4: Arctic");
            Help.Add("5: Look up to the sky");
            Help.Add("");
            Help.Add("Left-click: launch a satellite");
            Help.Add("Right-click: remove all satellites");
        }

        public override void LoadContent()
        {
            Font = Game.Content.Load<SpriteFont>(@"Fonts\font1");
            Vernal.Model = Game.Content.Load<Model>(@"Models\vernal");
            Summer.Model = Game.Content.Load<Model>(@"Models\summer");
            Autumnal.Model = Game.Content.Load<Model>(@"Models\autumnal");
            Winter.Model = Game.Content.Load<Model>(@"Models\winter");
        }

        public override void Update(float dt)
        {
            Info.Clear();

            var minsFromMidday = Game.Earth.Revolution * 1440 / Earth.RevolutionAngularSpeed + 720;
            var dayFromVernalEquinox = (int)(minsFromMidday / 1440);

            var month = 2;
            var day = 21;

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
            const float piOver8 = MathHelper.PiOver2/4;
            var angle = Game.Earth.Revolution;
            if (angle > 0 && angle < piOver8)
            {
                Vernal.Draw(dt);
            }
            else if (angle > MathHelper.PiOver2 && angle < piOver8 * 5)
            {
                Summer.Draw(dt);
            }
            else if (angle > MathHelper.Pi && angle < piOver8 * 9)
            {
                Autumnal.Draw(dt);
            }
            else if (angle > MathHelper.PiOver2 * 3 && angle < piOver8 * 13)
            {
                Winter.Draw(dt);
            }

            var pos = new Vector2(10, 10);
            foreach (var info in Info)
            {
                Game.SpriteBatch.DrawString(Font, info, pos, Color.White);
                pos += new Vector2(0, 20);
            }

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
            Game.SpriteBatch.DrawString(Font, runningSpeed, new Vector2(630, 10), Color.White);

            pos = new Vector2(1120, 10);
            Game.SpriteBatch.DrawString(Font, "H: Help", pos, Color.White);
            if (Game.Setting.ShowHelp)
            {
                foreach (var help in Help)
                {
                    pos += new Vector2(0, 20);
                    Game.SpriteBatch.DrawString(Font, help, pos, Color.White);
                }
            }
        }
    }
}
