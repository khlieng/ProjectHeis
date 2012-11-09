using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHeis
{
    public class Entity : DrawableGameComponent
    {
        public float Rotation { get; set; }
        private Vector3 position;

        public Texture2D Texture { get; set; }
        public float Alpha { get; set; }

        

       

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                /*world = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(value);
                BB = UpdateBoundingBox(model, Matrix.CreateScale(Scale) * Matrix.CreateTranslation(value));*/
            }
        }
        public Vector3 Direction { get; set; }
        public float VelocityY { get; set; }
        public Vector3 Scale { get; set; }
        public bool Gravity { get; set; }
        public BoundingBox BB
        {
            get
            {
                return UpdateBoundingBox(model, Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position));
            }
        }
        public bool Floored { get; set; }

        private Matrix world;
        private Model model;

        private Vector3 color;
        public Color Color
        {
            get 
            {
                return new Color(color);
            }
            set 
            {
                color = new Vector3(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f);
            }
        }

        public IController Controller { get; set; }

        public Entity(Game game, Model model)
            : base(game)
        {
            this.model = model;
            Scale = Vector3.One;
            Direction = Vector3.Forward;
            Gravity = false;
            Color = Color.DarkGray;
            Alpha = 1;
            
            Initialize();

            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Controller != null)
            {
                Controller.Update(gameTime);
            }

            if (Gravity)
            {
                VelocityY -= 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += new Vector3(0, VelocityY, 0);
            }
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            BasicEffect b = (BasicEffect)model.Meshes[0].Effects[0];
            if (Texture != null)
            {
                b.TextureEnabled = true;
                b.Texture = Texture;
            }
            else
            {
                b.Alpha = Alpha;
            }
            world = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(Position);

            /*BB = UpdateBoundingBox(model, Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position));*/

            BasicEffect e = (BasicEffect)model.Meshes[0].Effects[0];
            Vector3 prev = e.DiffuseColor;
            e.DiffuseColor = color;
            
            model.Draw(world, TheGame.Camera.View, TheGame.Camera.Projection);

            e.TextureEnabled = false;
            e.DiffuseColor = prev;

            base.Draw(gameTime);
        }

        private BoundingBox UpdateBoundingBox(Model model, Matrix world)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), world);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            float d = (max.Z - min.Z) / 2.0f;

            // Create and return bounding box
            return new BoundingBox(new Vector3(min.X, min.Y, min.Z - d), new Vector3(max.X, max.Y, max.Z - d));
        }
    }
}
