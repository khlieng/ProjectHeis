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

        //Constructor
        public WaterEffect(Game game) : base(game){}//end of constructor

       
        public override void Initialize()
        {
            base.Initialize();
        }//end of Initialize

        protected override void LoadContent()
        {
            base.LoadContent();
        }//end of LoadContent


        
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        }//end of Draw()

    }//end of WaterEffect.cs
}//end of nameSpace
