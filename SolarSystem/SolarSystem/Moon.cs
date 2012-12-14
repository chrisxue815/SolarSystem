using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Moon : GameEntity
    {
        private Earth Earth { get; set; }
        
        private Vector3 relativePosition;
        private Vector3 RelativePosition
        {
            get { return relativePosition; }
            set
            {
                relativePosition = value;
                Position = Earth.Position + relativePosition;
            }
        }

        public const float Radius = 3f;

        // revolution
        public const float RevolutionRadius = 35f;
        public const float RevolutionPeriod = 29.53f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;

        // rotation
        public const float RotationPeriod = RevolutionPeriod;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;
        private float Rotation { get; set; }

        // drawing revolution orbit
        short[] lineStripIndices;
        private const int points = 500;
        VertexBuffer vertexBuffer;
        private VertexPositionColor[] revolutionPlanePointList;

        // drawing rotation orbit
        VertexBuffer vertexBuffer2;
        private VertexPositionColor[] rotationPlanePointList;
        private VertexPositionColor[] rotationAxisPointList = new VertexPositionColor[2];
        private BasicEffect BasicEffect { get; set; }

        public Moon()
        {
            Earth = Game.Earth;
            Rotation = 0;

            RelativePosition = new Vector3(0, 0, RevolutionRadius);

            Scale = Matrix.CreateScale(Radius);

            ModelName = @"Models\moon";

            DiffuseColor = Color.Gray.ToVector3();

            InitLineStrip();
            
            BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);
            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;
        }

        public override void Update(float dt)
        {
            // Axial rotation
            var angle = RevolutionAngularSpeed * dt;
            RelativePosition = Vector3.Transform(RelativePosition, Matrix.CreateRotationY(angle));

            // Revolution
            var rotationAxis = Up;
            rotationAxis.Normalize();
            rotationAxis *= Radius * 2f;
            rotationAxisPointList[0] = new VertexPositionColor(Position + rotationAxis, Color.Red);
            rotationAxisPointList[1] = new VertexPositionColor(Position - rotationAxis, Color.Red);

            InitRevPointList();
            InitRotPointList();

            Rotation += RotationAngularSpeed * dt;
            if (Rotation > MathHelper.TwoPi) Rotation %= MathHelper.TwoPi;
            rotationAxis.Normalize();
            LocalTransform = Scale * Matrix.CreateFromAxisAngle(rotationAxis, Rotation);
        }

        public override void Draw(float dt)
        {
            if (Game.Setting.ShowMoonRevolutionAxis || Game.Setting.ShowMoonRotationAxis)
            {
                BasicEffect.View = Game.Camera.View;
                BasicEffect.Projection = Game.Camera.Projection;

                foreach (var pass in BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                if (Game.Setting.ShowMoonRevolutionAxis)
                {
                    DrawRevolutionOrbit();
                }

                if (Game.Setting.ShowMoonRotationAxis)
                {
                    Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                           rotationAxisPointList, 0, 1);
                    // Draw the Moon's rotation orbit
                    DrawRotationOrbit();
                }
            }

            base.Draw(dt);
        }

        private void InitLineStrip()
        {
            // Initialize an array of indices of type short.
            lineStripIndices = new short[points];

            // Populate the array with references to indices in the vertex buffer.
            for (int i = 0; i < points; i++)
            {
                lineStripIndices[i] = (short)(i);
            }
        }

        /* Initialize point list on the revolution orbit to be drawn */
        private void InitRevPointList()
        {
            revolutionPlanePointList = new VertexPositionColor[points];

            // Add points to the orbit point list
            for (int i = 0; i < points - 1; i++)
            {
                float theta = (MathHelper.TwoPi / points) * i;

                float x = Earth.Position.X + RevolutionRadius * (float)Math.Sin(theta);
                float z = Earth.Position.Z + RevolutionRadius * (float)Math.Cos(theta);
                revolutionPlanePointList[i] = new VertexPositionColor(new Vector3(x, 0, z), Color.White);
            }
            // The last point is the same with the starting point
            revolutionPlanePointList[points - 1] = revolutionPlanePointList[0];

            // Initialize the vertex buffer, allocating memory for each vertex.
            vertexBuffer = new VertexBuffer(Game1.Instance.GraphicsDevice, typeof(VertexPositionNormalTexture),
                                    250, BufferUsage.WriteOnly | BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer.SetData(revolutionPlanePointList);
        }

        /* Initialize point list on the rotation orbit to be drawn */
        private void InitRotPointList()
        {
            rotationPlanePointList = new VertexPositionColor[points];

            // Add points to the orbit point list
            for (int i = 0; i < points - 1; i++)
            {
                float theta = (MathHelper.TwoPi / points) * i;

                // Translate position relative to the Earth's position
                float x = Position.X + (Radius + 2) * (float)Math.Sin(theta);
                float z = Position.Z + (Radius + 2) * (float)Math.Cos(theta);
                var orbitPos = new Vector3(x, Position.Y, z);

                rotationPlanePointList[i] = new VertexPositionColor(orbitPos, Color.White);
            }
            // The last point is the same with the starting point
            rotationPlanePointList[points - 1] = rotationPlanePointList[0];

            // Initialize the vertex buffer, allocating memory for each vertex.
            vertexBuffer2 = new VertexBuffer(Game.GraphicsDevice, typeof(VertexPositionNormalTexture),
                                    250, BufferUsage.WriteOnly | BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer2.SetData(rotationPlanePointList);
        }

        /* Draw lines to connect two points continuously for revolution orbit */
        private void DrawRevolutionOrbit()
        {
            for (int i = 0; i < revolutionPlanePointList.Length; i++)
                revolutionPlanePointList[i].Color = Color.Red;

            Game.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineStrip,
                revolutionPlanePointList,
                0,                  // vertex buffer offset to add to each element of the index buffer
                points,             // number of vertices to draw
                lineStripIndices,
                0,                  // first index element to read
                points - 1          // number of primitives to draw
            );

            for (int i = 0; i < revolutionPlanePointList.Length; i++)
                revolutionPlanePointList[i].Color = Color.White;
        }

        /* Draw lines to connect two points continuously for rotation orbit */
        private void DrawRotationOrbit()
        {
            for (int i = 0; i < rotationPlanePointList.Length; i++)
                rotationPlanePointList[i].Color = Color.Red;

            Game.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineStrip,
                rotationPlanePointList,
                0,                  // vertex buffer offset to add to each element of the index buffer
                points,             // number of vertices to draw
                lineStripIndices,
                0,                  // first index element to read
                points - 1          // number of primitives to draw
            );

            for (int i = 0; i < rotationPlanePointList.Length; i++)
                rotationPlanePointList[i].Color = Color.White;
        }
    }
}
