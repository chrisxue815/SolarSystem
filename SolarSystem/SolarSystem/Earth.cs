using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSystem
{
    public class Earth : GameEntity
    {
        private Sun Sun { get; set; }
        private float Spin { get; set; }
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

        private Effect Effect { get; set; }
        private Texture2D DayTexture { get; set; }
        private Texture2D NightTexture { get; set; }
        private Texture2D CloudTexture { get; set; }
        private Texture2D NormalMapTexture { get; set; }

        private Vector4 globalAmbient = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        private Vector4 sunlightDirection = new Vector4(Vector3.Forward, 0.0f);
        private Vector4 sunlightColor = new Vector4(1.0f, 0.941f, 0.898f, 1.0f);
        private Vector4 ambient = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
        private Vector4 diffuse = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        private Vector4 specular = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        private float shininess = 20.0f;
        private float cloudStrength = 1.15f;
        private BoundingSphere bounds;

        public const float Radius = 2f;
        public const float RevolutionRadius = 100f;
        public const float RevolutionPeriod = 10f;
        public const float RevolutionAngularSpeed = MathHelper.TwoPi / RevolutionPeriod;
        public const float RotationPeriod = RevolutionPeriod / 365 * 100;
        public const float RotationAngularSpeed = MathHelper.TwoPi / RotationPeriod;

        private static readonly BasicEffect BasicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);

        public Earth(Sun sun)
        {
            Sun = sun;
            Spin = 0;
            RotationAxisPointList = new VertexPositionColor[2];

            RelativePosition = new Vector3(0, 0, -RevolutionRadius);

            Scale = Matrix.CreateScale(new Vector3(Radius, Radius, Radius));

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.Identity;
        }

        public override void LoadContent()
        {
            Model = Game1.Instance.Content.Load<Model>(@"Models\earth");
            Effect = Game1.Instance.Content.Load<Effect>(@"Effects\earth");
            DayTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures\earth_day_color_spec");
            NightTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures\earth_night_color");
            CloudTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures\earth_clouds_alpha");
            NormalMapTexture = Game1.Instance.Content.Load<Texture2D>(@"Textures\earth_nrm");

            // Calculate the bounding sphere of the Earth model and bind the
            // custom Earth effect file to the model.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                bounds = BoundingSphere.CreateMerged(bounds, mesh.BoundingSphere);

                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = Effect;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // delta time
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // revolution
            var angle = RevolutionAngularSpeed * dt;
            RelativePosition = Vector3.Transform(RelativePosition, Matrix.CreateRotationY(angle));

            // rotation axis
            const float eclipticObliquity = -0.409f;
            var rotationAxis = Vector3.Transform(Up, Matrix.CreateRotationZ(eclipticObliquity));
            rotationAxis.Normalize();
            rotationAxis *= Radius * 10f;
            RotationAxisPointList[0] = new VertexPositionColor(Position + rotationAxis, Color.Red);
            RotationAxisPointList[1] = new VertexPositionColor(Position - rotationAxis, Color.Red);

            // rotation
            Spin += RotationAngularSpeed * dt;
            if (Spin > MathHelper.TwoPi) Spin -= MathHelper.TwoPi;
            rotationAxis.Normalize();
            LocalTransform = Scale*Matrix.CreateRotationZ(eclipticObliquity)*
                             Matrix.CreateFromAxisAngle(rotationAxis, Spin);

            var fromSun = Position - Sun.Position;
            fromSun.Normalize();
            sunlightDirection = new Vector4(fromSun, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            // draw rotation axis
            BasicEffect.View = Game1.Instance.Camera.View;
            BasicEffect.Projection = Game1.Instance.Camera.Projection;

            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game1.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                                 RotationAxisPointList, 0, 1);
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
                        e.Parameters["view"].SetValue(Game1.Instance.Camera.View);
                        e.Parameters["projection"].SetValue(Game1.Instance.Camera.Projection);
                        e.Parameters["cameraPos"].SetValue(new Vector4(Game1.Instance.Camera.Position, 1.0f));
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
    }
}
