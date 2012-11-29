using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHeis
{
    public class ParticleEmitter : DrawableGameComponent
    {
        private class Particle
        {
            public Vector3 Position { get; set; }
            public Vector3 Velocity { get; set; }

            public Particle(Vector3 position, Vector3 velocity)
            {
                Position = position;
                Velocity = velocity;
            }
        }

        private VertexPositionTexture[] vertices;
        private BasicEffect effect;
        private List<Particle> particles;

        public Vector3 Position { get; set; }

        public ParticleEmitter(Game game)
            : base(game)
        {
            vertices = new VertexPositionTexture[6 * 3];

            vertices[0] = new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 0.0f));
            vertices[1] = new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 1.0f));
            vertices[2] = new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1.0f, 1.0f));

            vertices[3] = new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 0.0f));
            vertices[4] = new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1.0f, 1.0f));
            vertices[5] = new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector2(1.0f, 0.0f));

            particles = new List<Particle>();
            particles.Add(new Particle(new Vector3(20, 0, 0), Vector3.Zero));

            Position = new Vector3(0, 125, 0);
        }

        protected override void LoadContent()
        {
            effect = new BasicEffect(GraphicsDevice);
            
            effect.Projection = TheGame.Camera.Projection;
            effect.TextureEnabled = true;
            effect.Texture = Game.Content.Load<Texture2D>("Images/terrain3");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            effect.View = TheGame.Camera.View;
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Particle p in particles)
                {
                    effect.World = Matrix.CreateScale(5.0f) * Matrix.CreateConstrainedBillboard(Position + p.Position, TheGame.Camera.Position, Vector3.Up, null, null);

                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2, VertexPositionTexture.VertexDeclaration);
                }
            }

            base.Draw(gameTime);
        }
    }
}
