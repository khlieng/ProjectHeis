using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * SkyDome er basert på http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Skydome.php
 * Koden er skrevet om slik at den kan brukes i XNA 4.0
 * Texture2D er et random bilde hentet fra www.google.no
 * dome.x er selve domen med kordinater, det er den Riemers har laget.
 */
namespace ProjectHeis
{
    class SkyDome : DrawableGameComponent
    {
        Effect effect;
        Texture2D cloudMap;
        Model skyDome;

        public SkyDome(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            effect = Game.Content.Load<Effect>("Effect/effects");
            skyDome = Game.Content.Load<Model>("Models/dome"); skyDome.Meshes[0].MeshParts[0].Effect = effect.Clone();
            cloudMap = Game.Content.Load<Texture2D>("Images/dome1");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = new DepthStencilState
            {
                DepthBufferEnable = true
            };
            Matrix[] modelTransforms = new Matrix[skyDome.Bones.Count];
            skyDome.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(0, -0.3f, 0) * Matrix.CreateScale(10000);
            foreach (ModelMesh mesh in skyDome.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(TheGame.Camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(TheGame.Camera.Projection);
                    currentEffect.Parameters["xTexture"].SetValue(cloudMap);
                    currentEffect.Parameters["xEnableLighting"].SetValue(false);
                }
                mesh.Draw();
            }
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
    }
}
