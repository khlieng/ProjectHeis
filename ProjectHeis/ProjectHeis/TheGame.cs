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
        private Model treeModel;

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

            effect = new BasicEffect(GraphicsDevice);
        }
        Entity player;
        Entity floor;
        Entity floor2;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("arial");

            box = Content.Load<Model>("box");
            (box.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();
            treeModel = Content.Load<Model>("tree");
            (treeModel.Meshes[0].MeshParts[0].Effect as BasicEffect).EnableDefaultLighting();

            floor = new Entity(this, box);
            floor.Scale = new Vector3(1f, 0.5f, 1f);
            floor.Position = new Vector3(0, -40, 0);

            floor2 = new Entity(this, box);
            floor2.Position = new Vector3(160, 0, 0);
            floor2.Scale = new Vector3(0.2f, 0.005f, 0.2f);

            Entity floor3 = new Entity(this, box);
            floor3.Position = new Vector3(240, 0, 0);
            floor3.Scale = new Vector3(0.2f, 0.005f, 0.2f);

            player = new Entity(this, box);
            player.Position = new Vector3(0, 100, 0);
            player.Controller = new KeyboardController(player);
            player.Scale = new Vector3(0.1f);
            player.Gravity = true;

            Entity tree = new Entity(this, treeModel);
            tree.Scale = new Vector3(0.15f);
            tree.Position = new Vector3(50, 10, 50);
            Entity tree2 = new Entity(this, treeModel);
            tree2.Scale = new Vector3(0.17f);
            tree2.Position = new Vector3(30, 5, 40);
            Entity tree3 = new Entity(this, treeModel);
            tree3.Scale = new Vector3(0.2f);
            tree3.Position = new Vector3(40, 5, 10);

            Terrain terrain = new Terrain(this);
            terrain.Initialize();
            SkyDome skyDome = new SkyDome(this);
            skyDome.Initialize();

            staticEntities.Add(floor);
            staticEntities.Add(floor2);
            staticEntities.Add(floor3);
            movingEntities.Add(player);

            Camera = new Camera(this, player);

            Components.Add(terrain);
            Components.Add(skyDome);
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
                        Vector3 halfWidthMoving = (m.BB.Max - m.BB.Min) * 0.5f;
                        Vector3 halfWidthStatic = (s.BB.Max - s.BB.Min) * 0.5f;

                        Vector3 centerMoving = m.BB.Min + new Vector3(halfWidthMoving.X, halfWidthMoving.Y, halfWidthMoving.Z);
                        Vector3 centerStatic = s.BB.Min + new Vector3(halfWidthStatic.X, halfWidthStatic.Y, halfWidthStatic.Z); ;

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
                                m.VelocityY = 0;
                                if (-overlap[max] > 0)
                                {
                                    if (!m.Floored)
                                    {
                                        m.Floored = true;
                                    }
                                }
                                break;

                            case 2:
                                projection = new Vector3(0, 0, -overlap[max]);
                                m.Position += projection;
                                break;
                        }

                    }
                    if (m.VelocityY != 0)
                        m.Floored = false;
                }
            }            

            info = "";
            info += "\nX: " + player.Position.X + " Y: " + player.Position.Y + " Z: " + player.Position.Z;
            info += "\npMin: " + player.BB.Min + ", pMax: " + player.BB.Max;
            info += "\nf1Min: " + floor.BB.Min + ", f1Max: " + floor.BB.Max;
            info += "\nf2Min: " + floor2.BB.Min + ", f2Max: " + floor2.BB.Max;
            info += "\nPlayer floored: " + player.Floored;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            device.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.Solid
            };

            device.DepthStencilState = new DepthStencilState
            {
                DepthBufferEnable = true
            };

            device.Clear(Color.Black);
            
            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, info, new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }
    }
}
