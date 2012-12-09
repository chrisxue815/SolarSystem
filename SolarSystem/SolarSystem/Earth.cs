using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SolarSystem
{
    public class Earth : GameEntity
    {
        private Sun Sun { get; set; }
        private float Spin { get; set; }
        private Vector3 RotationAxisHalf { get; set; }

        private Vector3 relativePosition;
        private Vector3 RelativePosition
        {
            get { return relativePosition; }
            set
            {
                relativePosition = value;
                Position = Sun.Position + relativePosition;
            }
        }

        private VertexPositionColor[] RotationAxisPointList { get; set; }

        // contents
        private Effect Effect { get; set; }
        private Texture2D DayTexture { get; set; }
        private Texture2D NightTexture { get; set; }
        private Texture2D CloudTexture { get; set; }
        private Texture2D NormalMapTexture { get; set; }

        // rendering
        private Vector4 globalAmbient = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        private Vector4 sunlightDirection = new Vector4(Vector3.Forward, 0.0f);
        private Vector4 sunlightColor = new Vector4(1.0f, 0.941f, 0.898f, 1.0f);
        private Vector4 ambient = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
        private Vector4 diffuse = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        private Vector4 specular = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        private float shininess = 20.0f;
        private float cloudStrength = 1.15f;
        private BoundingSphere bounds;
        private BasicEffect BasicEffect { get; set; }

        // attributes
        public const float Radius = 2f;
        public const float EclipticObliquity = -0.409f;

        // drawing revolution orbit
        VertexDeclaration vertexDeclaration;
        VertexBuffer vertexBuffer;
        private VertexPositionColor[] orbitPointList;
        short[] lineStripIndices;
        private const int points = 500;

        // revolution
        public const float RevolutionRadius = 100f;
        public const float RevolutionPeriod = 365.2564f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;

        // rotation
        public const float RotationPeriod = 1f;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        public Earth()
        {
            Sun = Game.Sun;
            Spin = 0;
            RotationAxisPointList = new VertexPositionColor[2];

            RelativePosition = new Vector3(0, 0, -RevolutionRadius);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            BasicEffect = new BasicEffect(Game.GraphicsDevice);
            
            InitializePointList();
            InitializeLineStrip();

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;

            // up
            Up = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ(EclipticObliquity));
            Up.Normalize();

            // rotation axis
            RotationAxisHalf = Up * Radius * 7f;
        }

        public override void LoadContent()
        {
            Model = Game.Content.Load<Model>(@"Models\earth");
            Effect = Game.Content.Load<Effect>(@"Effects\earth");
            DayTexture = Game.Content.Load<Texture2D>(@"Textures\earth_day_color_spec");
            NightTexture = Game.Content.Load<Texture2D>(@"Textures\earth_night_color");
            CloudTexture = Game.Content.Load<Texture2D>(@"Textures\earth_clouds_alpha");
            NormalMapTexture = Game.Content.Load<Texture2D>(@"Textures\earth_nrm");

            // Calculate the bounding sphere of the Earth model and bind the
            // custom Earth effect file to the model.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                bounds = BoundingSphere.CreateMerged(bounds, mesh.BoundingSphere);

                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = Effect;
            }
        }

        public override void Update(float dt)
        {
            // revolution
            var revolutionAngle = RevolutionAngularSpeed * dt;
            RelativePosition = Vector3.Transform(RelativePosition, Matrix.CreateRotationY(revolutionAngle));

            // rotation axis
            RotationAxisPointList[0] = new VertexPositionColor(Position + RotationAxisHalf, Color.Red);
            RotationAxisPointList[1] = new VertexPositionColor(Position - RotationAxisHalf, Color.Red);

            // rotation
            var rotationAngle = RotationAngularSpeed * dt;
            if (rotationAngle > MathHelper.Pi) rotationAngle = 1f;
            Spin += rotationAngle;
            if (Spin > MathHelper.TwoPi) Spin %= MathHelper.TwoPi;
            LocalTransform = Scale * Matrix.CreateRotationZ(EclipticObliquity) *
                             Matrix.CreateFromAxisAngle(Up, Spin);

            // sunlight direction
            var fromSun = Position - Sun.Position;
            fromSun.Normalize();
            sunlightDirection = new Vector4(fromSun, 0);
        }

        public override void Draw(float dt)
        {
            if (Game.Setting.ShowAxis)
            {
                // draw rotation axis
                BasicEffect.View = Game.Camera.View;
                BasicEffect.Projection = Game.Camera.Projection;

                foreach (var pass in BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                           RotationAxisPointList, 0, 1);
                }
                
                DrawRevolutionOrbit();
            }

            // draw model
            if (Model != null)
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (Effect e in mesh.Effects)
                    {
                        e.CurrentTechnique = e.Techniques["EarthWithClouds"];
                        e.Parameters["cloudStrength"].SetValue(cloudStrength);

                        e.Parameters["world"].SetValue(LocalTransform * Matrix.CreateTranslation(Position));
                        e.Parameters["view"].SetValue(Game.Camera.View);
                        e.Parameters["projection"].SetValue(Game.Camera.Projection);
                        e.Parameters["cameraPos"].SetValue(new Vector4(Game.Camera.Position, 1.0f));
                        e.Parameters["globalAmbient"].SetValue(globalAmbient);
                        e.Parameters["lightDir"].SetValue(sunlightDirection);
                        e.Parameters["lightColor"].SetValue(sunlightColor);
                        e.Parameters["materialAmbient"].SetValue(ambient);
                        e.Parameters["materialDiffuse"].SetValue(diffuse);
                        e.Parameters["materialSpecular"].SetValue(specular);
                        e.Parameters["materialShininess"].SetValue(shininess);
                        e.Parameters["landOceanColorGlossMap"].SetValue(DayTexture);
                        e.Parameters["cloudColorMap"].SetValue(CloudTexture);
                        e.Parameters["nightColorMap"].SetValue(NightTexture);
                        e.Parameters["normalMap"].SetValue(NormalMapTexture);
                    }

                    mesh.Draw();
                }
            }
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
                orbitPointList[i] = new VertexPositionColor(new Vector3(x, Position.Y, z), Color.White);
            }
            // The last point is the same with the starting point
            orbitPointList[points - 1] = new VertexPositionColor(new Vector3(Sun.Position.X, Position.Y, RevolutionRadius), Color.White);

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
