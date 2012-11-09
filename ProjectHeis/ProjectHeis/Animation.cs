using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectHeis
{
    public class Animation : GameComponent
    {
        private Entity entity;
        private Vector3 positionDelta;
        private int duration;
        private int elapsed;
        private bool done;

        public Animation(Game game, Entity entity, Vector3 positionDelta, int duration)
            : base(game)
        {
            this.entity = entity;
            this.positionDelta = new Vector3(positionDelta.X / duration, positionDelta.Y / duration, positionDelta.Z / duration);
            this.duration = duration;

            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!done)
            {
                elapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsed >= duration)
                {
                    done = true;
                }

                entity.Position += positionDelta * gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }
    }
}
