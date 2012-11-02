using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectHeis
{
    public class Camera : GameComponent
    {
        private Entity entity;
        private Vector3 position;
        private Vector3 target;
        private Vector3 up = Vector3.Up;
        
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public Camera(Game game, Entity entity)
            : base(game)
        {
            this.entity = entity;

            target = entity.Position;
            position = entity.Position - entity.Direction * 200 + new Vector3(0, 50, 0);

            View = Matrix.CreateLookAt(position, target, up);

            float aspectRatio = (float)Game.GraphicsDevice.Viewport.Width / (float)Game.GraphicsDevice.Viewport.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.01f, 1000.0f);

            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            target = entity.Position;
            position = entity.Position - entity.Direction * 200 + new Vector3(0, 50, 0);

            View = Matrix.CreateLookAt(position, target, up);

            base.Update(gameTime);
        }
    }
}
