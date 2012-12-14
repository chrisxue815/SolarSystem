using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Earth : GameEntity
    {
        private Sun Sun { get; set; }

        // relative to sun's position
        private Vector3 relativePosition;
        public Vector3 RelativePosition
        {
            get { return relativePosition; }
            set
            {
                relativePosition = value;
                Position = Sun.Position + relativePosition;
            }
        }

        // revolution constants
        public const float RevolutionRadius = 100f;
        public const float RevolutionPeriod = 365f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;

        // revolution
        public float Revolution { get; set; }
        private Vector3 RevolutionBasis { get; set; }

        // rotation constants
        public const float RotationPeriod = 0.99726956632908f;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        // rotation
        public float Rotation { get; set; }
        private float RotationInFastSpeed { get; set; }
        private Vector3 RotationAxis { get; set; }
        public Matrix RotationTransform { get; set; }

        // effects and textures
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

        // constants
        public const float Radius = 2f;
        public const float EclipticObliquity = -0.409f;

        // drawing revolution orbit
        private short[] lineStripIndices;
        private const int Points = 500;
        private VertexBuffer VertexBuffer { get; set; }
        private VertexPositionColor[] RevolutionPlanePointList { get; set; }

        // drawing rotation orbit
        private VertexBuffer VertexBuffer2 { get; set; }
        private VertexPositionColor[] RotationPlanePointList { get; set; }
        private VertexPositionColor[] RotationAxisPointList { get; set; }

        public Earth()
        {
            Sun = Game.Sun;
            Rotation = 0;
            RotationInFastSpeed = 0;
            RotationAxisPointList = new VertexPositionColor[2];

            // vernal equinox
            RevolutionBasis = new Vector3(0, 0, -RevolutionRadius);

            Scale = Matrix.CreateScale(Radius);

            BasicEffect = new BasicEffect(Game.GraphicsDevice);

            InitLineStrip();
            InitRevPointList();

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;

            // up
            Up = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ(EclipticObliquity));
            Up.Normalize();

            // rotation axis
            RotationAxis = Up * Radius * 7f;
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
            var revolutionAngle = RevolutionAngularSpeed*dt;
            Revolution += revolutionAngle;
            if (Math.Abs(Revolution) > MathHelper.TwoPi) Revolution %= MathHelper.TwoPi;
            if (Revolution < 0) Revolution += MathHelper.TwoPi;
            RelativePosition = Vector3.Transform(RevolutionBasis, Matrix.CreateRotationY(Revolution));

            // rotation axis
            RotationAxisPointList[0] = new VertexPositionColor(Position + RotationAxis, Color.Red);
            RotationAxisPointList[1] = new VertexPositionColor(Position - RotationAxis, Color.Red);

            // rotation
            var rotationAngle = RotationAngularSpeed*dt;
            Rotation += rotationAngle;
            if (Math.Abs(Rotation) > MathHelper.TwoPi) Rotation %= MathHelper.TwoPi;
            if (Rotation < 0) Rotation += MathHelper.TwoPi;

            if (rotationAngle > MathHelper.Pi)
            {
                RotationInFastSpeed += 1f;
                if (Math.Abs(RotationInFastSpeed) > MathHelper.TwoPi) RotationInFastSpeed %= MathHelper.TwoPi;
                if (RotationInFastSpeed < 0) RotationInFastSpeed += MathHelper.TwoPi;
                RotationTransform = Matrix.CreateRotationZ(EclipticObliquity) * Matrix.CreateFromAxisAngle(Up, RotationInFastSpeed);
                LocalTransform = Scale * RotationTransform;
            }
            else
            {
                RotationTransform = Matrix.CreateRotationZ(EclipticObliquity) * Matrix.CreateFromAxisAngle(Up, Rotation);
                LocalTransform = Scale * RotationTransform;
            }

            InitRotPointList();

            // sunlight direction
            var fromSun = Position - Sun.Position;
            fromSun.Normalize();
            sunlightDirection = new Vector4(fromSun, 0);
        }

        public override void Draw(float dt)
        {
            if (Game.Setting.ShowEarthRevolutionAxis || Game.Setting.ShowEarthRotationAxis)
            {
                BasicEffect.View = Game.Camera.View;
                BasicEffect.Projection = Game.Camera.Projection;

                foreach (var pass in BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                if (Game.Setting.ShowEarthRevolutionAxis)
                {
                    DrawRevolutionOrbit();
                }

                if (Game.Setting.ShowEarthRotationAxis)
                {
                    Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                           RotationAxisPointList, 0, 1);
                    // Draw the Earth's rotation orbit
                    DrawRotationOrbit();
                }
            }

            // draw model
            if (Model != null)
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (var e in mesh.Effects)
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

        private void InitLineStrip()
        {
            // Initialize an array of indices of type short.
            lineStripIndices = new short[Points];

            // Populate the array with references to indices in the vertex buffer.
            for (int i = 0; i < Points; i++)
            {
                lineStripIndices[i] = (short)(i);
            }
        }

        /* Initialize point list on the revolution orbit to be drawn */
        private void InitRevPointList()
        {
            RevolutionPlanePointList = new VertexPositionColor[Points];

            // Add points to the orbit point list
            for (int i = 0; i < Points - 1; i++)
            {
                float theta = (MathHelper.TwoPi / Points) * i;

                float x = RevolutionRadius * (float)Math.Sin(theta);
                float z = RevolutionRadius * (float)Math.Cos(theta);
                RevolutionPlanePointList[i] = new VertexPositionColor(new Vector3(x, Position.Y, z), Color.White);
            }
            // The last point is the same with the starting point
            RevolutionPlanePointList[Points - 1] = RevolutionPlanePointList[0];

            // Initialize the vertex buffer, allocating memory for each vertex.
            VertexBuffer = new VertexBuffer(Game1.Instance.GraphicsDevice, typeof(VertexPositionNormalTexture),
                                    250, BufferUsage.WriteOnly | BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            VertexBuffer.SetData(RevolutionPlanePointList);
        }

        /* Initialize point list on the rotation orbit to be drawn */
        private void InitRotPointList()
        {
            RotationPlanePointList = new VertexPositionColor[Points];

            // Add points to the orbit point list
            for (int i = 0; i < Points - 1; i++)
            {
                float theta = (MathHelper.TwoPi / Points) * i;

                float x = Moon.RevolutionRadius * (float)Math.Sin(theta);
                float z = Moon.RevolutionRadius * (float)Math.Cos(theta);

                // Rotate position by EclipticObliquity degree
                var orbitPos = new Vector3(x, Position.Y, z);
                orbitPos = Vector3.Transform(orbitPos, Matrix.CreateRotationZ(EclipticObliquity));

                // Translate position relative to the Earth's position
                orbitPos.X += Position.X;
                orbitPos.Z += Position.Z;

                RotationPlanePointList[i] = new VertexPositionColor(orbitPos, Color.White);
            }
            // The last point is the same with the starting point
            RotationPlanePointList[Points - 1] = RotationPlanePointList[0];

            // Initialize the vertex buffer, allocating memory for each vertex.
            VertexBuffer2 = new VertexBuffer(Game1.Instance.GraphicsDevice, typeof(VertexPositionNormalTexture),
                                    250, BufferUsage.WriteOnly | BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            VertexBuffer2.SetData(RotationPlanePointList);
        }

        /* Draw lines to connect two points continuously for revolution orbit */
        private void DrawRevolutionOrbit()
        {
            for (int i = 0; i < RevolutionPlanePointList.Length; i++)
                RevolutionPlanePointList[i].Color = Color.Red;

            Game.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineStrip,
                RevolutionPlanePointList,
                0,                  // vertex buffer offset to add to each element of the index buffer
                Points,             // number of vertices to draw
                lineStripIndices,
                0,                  // first index element to read
                Points - 1          // number of primitives to draw
            );

            for (int i = 0; i < RevolutionPlanePointList.Length; i++)
                RevolutionPlanePointList[i].Color = Color.White;
        }

        /* Draw lines to connect two points continuously for rotation orbit */
        private void DrawRotationOrbit()
        {
            for (int i = 0; i < RotationPlanePointList.Length; i++)
                RotationPlanePointList[i].Color = Color.Red;

            Game.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineStrip,
                RotationPlanePointList,
                0,                  // vertex buffer offset to add to each element of the index buffer
                Points,             // number of vertices to draw
                lineStripIndices,
                0,                  // first index element to read
                Points - 1          // number of primitives to draw
            );

            for (int i = 0; i < RotationPlanePointList.Length; i++)
                RotationPlanePointList[i].Color = Color.White;
        }
    }
}
