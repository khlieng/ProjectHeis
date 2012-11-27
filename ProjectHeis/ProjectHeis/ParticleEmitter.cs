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
        private VertexPositionTexture[] vertices;

        public ParticleEmitter(Game game)
            : base(game)
        {
            vertices = new VertexPositionTexture[6];
            vertices[0] = new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 0.0f));
            vertices[1] = new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 1.0f));
            vertices[2] = new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1.0f, 1.0f));

            vertices[3] = new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 0.0f));
            vertices[4] = new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1.0f, 1.0f));
            vertices[5] = new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0.0f), new Vector2(1.0f, 0.0f));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var pass in TheGame.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();


            }

            base.Draw(gameTime);
        }
    }
}
