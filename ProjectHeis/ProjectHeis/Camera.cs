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

        private float distance = 100.0f;
        private float height = 20.0f;

        public Camera(Game game, Entity entity)
            : base(game)
        {
            this.entity = entity;

            target = entity.Position;
            position = entity.Position - entity.Direction * distance + new Vector3(0, height, 0);

            View = Matrix.CreateLookAt(position, target, up);

            float aspectRatio = (float)Game.GraphicsDevice.Viewport.Width / (float)Game.GraphicsDevice.Viewport.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.01f, 20000.0f);

            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            target = entity.Position;
            position = entity.Position - entity.Direction * distance + new Vector3(0, height, 0);

            View = Matrix.CreateLookAt(position, target, up);

            base.Update(gameTime);
        }
    }
}
