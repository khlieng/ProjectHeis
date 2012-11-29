using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*Textbutton klassen brukes for og lage knapper.
 * Textbutton brukes nå til og representere etasjer i bygningen
 * 
 */
namespace ProjectHeis
{
    public class TextButton : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private MouseState prevMouse;
        private Rectangle bounds;

        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public SpriteFont Font { get; set; }

        public event EventHandler Click;

        public TextButton(Game game, Vector2 position, string text, SpriteFont font)
            : base(game)
        {
            Position = position;
            Text = text;
            Font = font;
            Color = Color.White;

            bounds = new Rectangle((int)Position.X, (int)Position.Y, (int)font.MeasureString(text).X, (int)font.MeasureString(text).Y);

            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));

            game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            if (bounds.Contains(mouse.X, mouse.Y))
            {
                if (mouse.LeftButton == ButtonState.Released &&
                    prevMouse.LeftButton == ButtonState.Pressed)
                {
                    OnClick();
                }
            }

            prevMouse = mouse;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(Font, Text, Position, Color);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }
    }
}
