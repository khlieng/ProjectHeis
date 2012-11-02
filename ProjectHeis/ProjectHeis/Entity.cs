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
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public float VelocityY { get; set; }
        public Vector3 Scale { get; set; }
        public bool Gravity { get; set; }
        public BoundingBox BB { get; private set; }

        private Matrix world;
        private Model model;

        public IController Controller { get; set; }

        public Entity(Game game, Model model)
            : base(game)
        {
            this.model = model;
            Scale = Vector3.One;
            Direction = Vector3.Forward;
            Gravity = false;

            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Gravity)
            {
                VelocityY -= 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += new Vector3(0, VelocityY, 0);
                /*if (Position.Y < 0)
                    Position = new Vector3(Position.X, 0, Position.Z);*/
            }

            if (Controller != null)
            {
                Controller.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            world = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(Position);

            BoundingSphere bs = model.Meshes[0].BoundingSphere;
            bs.Transform(world);
            BB = BoundingBox.CreateFromSphere(bs);

            model.Draw(world, TheGame.Camera.View, TheGame.Camera.Projection);

            base.Draw(gameTime);
        }

        private BoundingBox BoundingSphereToAABB(BoundingSphere sphere)
        {
            return new BoundingBox(sphere.Center - Vector3.One * sphere.Radius, sphere.Center + Vector3.One * sphere.Radius);
        }
    }
}
