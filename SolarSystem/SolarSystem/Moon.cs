using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SolarSystem
{
    public class Moon : GameEntity
    {
        private Earth Earth { get; set; }
        private float Spin { get; set; }
        private VertexPositionColor[] pointList = new VertexPositionColor[2];
        private Vector3 relativePosition;
        private Vector3 RelativePosition
        {
            get { return relativePosition; }
            set { relativePosition = value; Position = Earth.Position + relativePosition; }
        }

        VertexDeclaration vertexDeclaration;
        VertexBuffer vertexBuffer;
        private VertexPositionColor[] orbitPointList;
        short[] lineStripIndices;
        private const int points = 500;

        public const float Radius = 3f;
        public const float RevolutionRadius = 20f;
        public const float RevolutionPeriod = 0.75f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;
        public const float RotationPeriod = RevolutionPeriod;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        private static readonly BasicEffect BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);

        public Moon(Earth earth)
        {
            Earth = earth;
            Spin = 0;

            RelativePosition = new Vector3(0, 0, -RevolutionRadius);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            ModelName = "sphere";

            DiffuseColor = Color.Gray.ToVector3();

            InitializePointList();
            InitializeLineStrip();

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;
        }

        public override void Update(GameTime gameTime)
        {
            // delta time
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Axial rotation
            var angle = RevolutionAngularSpeed * dt;
            RelativePosition = Vector3.Transform(RelativePosition, Matrix.CreateRotationY(angle));

            // Revolution
            var rotationAxis = Up;
            rotationAxis.Normalize();
            rotationAxis *= Radius * 2f;
            pointList[0] = new VertexPositionColor(Position + rotationAxis, Color.Red);
            pointList[1] = new VertexPositionColor(Position - rotationAxis, Color.Red);

            Spin += RotationAngularSpeed * dt;
            if (Spin > MathHelper.TwoPi) Spin -= MathHelper.TwoPi;
            rotationAxis.Normalize();
            LocalTransform = Scale*Matrix.CreateFromAxisAngle(rotationAxis, -Spin);
        }

        public override void Draw(GameTime gameTime)
        {
            BasicEffect.View = Game1.Instance.Camera.View;
            BasicEffect.Projection = Game1.Instance.Camera.Projection;

            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                                 pointList, 0, 1);
                //DrawRevolutionOrbit();
            }

            base.Draw(gameTime);
        }

        /* Initialize point list on the orbit to be drawn */
        private void InitializePointList()
        {
            vertexDeclaration = new VertexDeclaration(VertexPositionTexture.VertexDeclaration.GetVertexElements());

            orbitPointList = new VertexPositionColor[points];

            // Add points to the orbit point list
            for (int i = 0; i < points - 1; i++)
            {
                float theta = ((float)MathHelper.TwoPi / points) * i;

                float x = RevolutionRadius * (float)Math.Sin(theta);
                float z = RevolutionRadius * (float)Math.Cos(theta);
                orbitPointList[i] = new VertexPositionColor(new Vector3(x, 0, z), Color.White);
            }
            // The last point is the same with the starting point
            orbitPointList[points - 1] = new VertexPositionColor(new Vector3(0, 0, RevolutionRadius), Color.White);

            // Initialize the vertex buffer, allocating memory for each vertex.
            vertexBuffer = new VertexBuffer(Game1.Instance.GraphicsDevice, typeof(VertexPositionNormalTexture),
                                    250, BufferUsage.WriteOnly | BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer.SetData<VertexPositionColor>(orbitPointList);
        }

        private void InitializeLineStrip()
        {
            // Initialize an array of indices of type short.
            lineStripIndices = new short[points];

            // Populate the array with references to indices in the vertex buffer.
            for (int i = 0; i < points; i++)
            {
                lineStripIndices[i] = (short)(i);
            }
        }

        /* Draw lines to connect two points continuously */
        private void DrawRevolutionOrbit()
        {
            for (int i = 0; i < orbitPointList.Length; i++)
                orbitPointList[i].Color = Color.Red;

            Game1.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                orbitPointList,
                0,                  // vertex buffer offset to add to each element of the index buffer
                points,             // number of vertices to draw
                lineStripIndices,
                0,                  // first index element to read
                points - 1          // number of primitives to draw
            );

            for (int i = 0; i < orbitPointList.Length; i++)
                orbitPointList[i].Color = Color.White;
        }
    }
}
