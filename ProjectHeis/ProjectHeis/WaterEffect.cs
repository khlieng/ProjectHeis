using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/**
 * Klassen skal brukes på lage vann på terenge
 * Basert på Rimers tutorial, men alt måtte skrives om til XNA 4.0
 * 
 */

namespace ProjectHeis 
{   

    class WaterEffect : DrawableGameComponent
    {

        const float waterHeight = 5.0f;
        RenderTarget2D refractionRenderTarget;
        Texture2D refractionMap;

        RenderTarget2D reflectionRenderTarget;
        Texture2D reflectionMap;

        VertexBuffer waterVertexBuffer;

        Vector3 windDirection = new Vector3(1, 0, 0);

        //Constructor
        public WaterEffect(Game game) : base(game){}//end of constructor

       
        public override void Initialize()
        {
            base.Initialize();
        }//end of Initialize

        protected override void LoadContent()
        {
            PresentationParameters pp = device.PresentationParameters;
            refractionRenderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            base.LoadContent();
        }//end of LoadContent


        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        }//end of Draw()

        private Plane CreatePlane(float height, Vector3 planeNormalDirection, Matrix currentViewMatrix, bool clipSide)
        {
            planeNormalDirection.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormalDirection, height);
            if (clipSide) planeCoeffs *= -1;
            Plane finalPlane = new Plane(planeCoeffs);
            return finalPlane;
        }

    }//end of WaterEffect.cs
}//end of nameSpace
