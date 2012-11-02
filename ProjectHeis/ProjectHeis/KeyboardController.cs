using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectHeis
{
    public class KeyboardController : IController
    {
        private Entity entity;
        private KeyboardState prevKeyboard;
        private float rot;

        public KeyboardController(Entity entity)
        {
            this.entity = entity;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (prevKeyboard.IsKeyUp(Keys.Space) &&
                keyboard.IsKeyDown(Keys.Space))
            {
                entity.VelocityY = 4;
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                entity.Position += entity.Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * 50.0f;
            }
            else if (keyboard.IsKeyDown(Keys.S))
            {
                entity.Position -= entity.Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * 50.0f;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                float prev = rot;
                rot += (float)gameTime.ElapsedGameTime.TotalSeconds * 3.0f;

                entity.Rotation = rot;
                entity.Direction = Vector3.Transform(entity.Direction, Matrix.CreateRotationY(-(prev - rot)));
            }
            else if (keyboard.IsKeyDown(Keys.D))
            {
                float prev = rot;
                rot -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3.0f;

                entity.Rotation = rot;
                entity.Direction = Vector3.Transform(entity.Direction, Matrix.CreateRotationY(-(prev - rot)));
            }
        }
    }
}
