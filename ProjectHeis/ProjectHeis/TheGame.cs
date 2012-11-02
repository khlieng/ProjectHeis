using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectHeis
{
    public class TheGame : Game
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;

        private BasicEffect effect;
        private SpriteBatch spriteBatch;

        private Model box;

        private SpriteFont font;
        public static string info = "TEST";

        private List<Entity> movingEntities = new List<Entity>();
        private List<Entity> staticEntities = new List<Entity>();

        public static Camera Camera { get; private set; }

        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitDevice();
        }

        private void InitDevice()
        {
            device = graphics.GraphicsDevice;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            effect = new BasicEffect(graphics.GraphicsDevice);
        }
        Entity player;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("arial");

            box = Content.Load<Model>("box");
            (box.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();

            Entity floor = new Entity(this, box);
            floor.Position = new Vector3(0, -15, 0);
            floor.Scale = new Vector3(1f, 0.005f, 1f);

            Entity floor2 = new Entity(this, box);
            floor2.Position = new Vector3(0, 40, 0);
            floor2.Scale = new Vector3(1f, 0.005f, 1f);

            player = new Entity(this, box);
            player.Controller = new KeyboardController(player);
            player.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            //player.Gravity = true;

            staticEntities.Add(floor);
            staticEntities.Add(floor2);
            movingEntities.Add(player);

            Camera = new Camera(this, player);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var m in movingEntities)
            {
                foreach (var s in staticEntities)
                {
                    if (m.BB.Intersects(s.BB))
                    {
                        info = "Kollisjon!";
                        /*Vector3 halfWidthMoving = m.BB.Max - m.BB.Min;
                        Vector3 halfWidthStatic = s.BB.Max - s.BB.Min;

                        Vector3 centerMoving = m.BB.Min + halfWidthMoving;
                        Vector3 centerStatic = s.BB.Min + halfWidthStatic;

                        Vector3 distance = centerMoving - centerStatic;

                        float[] overlap = { distance.X - (halfWidthMoving.X + halfWidthStatic.X),
                                            distance.Y - (halfWidthMoving.Y + halfWidthStatic.Y),
                                            distance.Z - (halfWidthMoving.Z + halfWidthStatic.Z) };

                        int max = Array.IndexOf(overlap, overlap.Max());
                        Vector3 projection;

                        switch (max)
                        {
                            case 0:
                                projection = new Vector3(-overlap[max], 0, 0);
                                m.Position += projection;
                                break;

                            case 1:
                                projection = new Vector3(0, -overlap[max], 0);
                                m.Position += projection;
                                break;

                            case 2:
                                projection = new Vector3(0, 0, -overlap[max]);
                                m.Position += projection;
                                break;
                        }*/
                    }
                    else
                    {
                        info = "NOPE";
                    }
                    info += "\nX: " + player.Position.X + " Y: " + player.Position.Y + " Z: " + player.Position.Z;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            device.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid
            };

            device.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, info, new Vector2(10, 10), Color.White);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
