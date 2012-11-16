using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHeis
{
    public class ElevatorDoors
    {
        private Game game;

        public Entity LeftDoor { get; private set; }
        public Entity RightDoor { get; private set; }
        public bool Opened { get; private set; }

        public ElevatorDoors(Game game, int floor, Model m)
        {
            this.game = game;

            LeftDoor = new Entity(game, m);
            LeftDoor.Position = new Vector3(-100, floor * 50 + 25, -90);
            LeftDoor.Scale = new Vector3(0.01f, 0.24f, 0.1f);
            RightDoor = new Entity(game, m);
            RightDoor.Position = new Vector3(-100, floor * 50 + 25, -69);
            RightDoor.Scale = new Vector3(0.01f, 0.24f, 0.1f);

        }

        public void Open()
        {
            if (!Opened)
            {
                new Animation(game, LeftDoor, new Vector3(0, 0, -15), 2000).Done += (o, e) => Opened = true;
                new Animation(game, RightDoor, new Vector3(0, 0, 15), 2000);
            }
        }

        public void Close()
        {
            if (Opened)
            {
                new Animation(game, LeftDoor, new Vector3(0, 0, 15), 2000).Done += (o, e) => Opened = false;
                new Animation(game, RightDoor, new Vector3(0, 0, -15), 2000);
            }
        }            
    }
}
