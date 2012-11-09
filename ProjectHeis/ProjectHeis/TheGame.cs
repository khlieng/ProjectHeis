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

        private KeyboardState prevKeyboard;

        private Model box;
        private Model treeModel;

        private Texture2D textureBuilding;

        private SpriteFont font;
        private SpriteFont bigFont;
        public static string info = "TEST";
        private string floorNumber = "";

        private Entity player;
        private Entity floor;
        private Entity[] floors;
        private Entity elevator;

        private BoundingBox[] floorNumbers;
        private BoundingBox[] elevatorFronts;
        private int currentFloor;
        private bool inFrontOfElevator;
        private bool onElevator;
        private TextButton[] elevatorButtons;

        private int elevatorDirection;
        private float elevatorSpeed = 50.0f;
        private int elevatorTargetY;

        private List<Entity> movingEntities = new List<Entity>();
        private List<Entity> staticEntities = new List<Entity>();

        public static Camera Camera { get; private set; }

        private BloomComponent bloom;

        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            graphics.PreferMultiSampling = true;            
            graphics.ApplyChanges();

            effect = new BasicEffect(GraphicsDevice);

            bloom = new BloomComponent(this);
            bloom.Settings = BloomSettings.PresetSettings[5];
            bloom.Initialize();
            Components.Add(bloom);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            font = Content.Load<SpriteFont>("arial");
            bigFont = Content.Load<SpriteFont>("big");

            box = Content.Load<Model>("box");
            (box.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();
            treeModel = Content.Load<Model>("tree");
            (treeModel.Meshes[0].MeshParts[0].Effect as BasicEffect).EnableDefaultLighting();

            textureBuilding = Content.Load<Texture2D>("Building_texture3");
           /*effect.TextureEnabled = true;
            effect.Texture = textureBuilding;*/

            
            
            player = new Entity(this, box);
            player.Position = new Vector3(0, 100, 0);
            player.Controller = new KeyboardController(player);
            player.Scale = new Vector3(0.01f, 0.02f, 0.01f);
            player.Gravity = true;
            player.Color = Color.Purple;

            floor = new Entity(this, box);
            floor.Scale = new Vector3(0.99f, 0.5f, 0.99f);
            floor.Position = new Vector3(0, -50, 0);

            Entity wall = new Entity(this, box);
            wall.Position = new Vector3(100, 450, 0);
            wall.Scale = new Vector3(0.02f, 5, 1);
            wall.Texture = textureBuilding;
            Entity wall2 = new Entity(this, box);
            wall2.Position = new Vector3(0, 450, 100);
            wall2.Scale = new Vector3(1, 5, 0.02f);
            wall2.Texture = textureBuilding;
            Entity wall3 = new Entity(this, box);
            wall3.Position = new Vector3(0, 450, -100);
            wall3.Scale = new Vector3(1, 5, 0.02f);
            wall3.Texture = textureBuilding;
            Entity wall4 = new Entity(this, box);
            wall4.Position = new Vector3(-100, 500, 20);
            wall4.Scale = new Vector3(0.02f, 4.5f, 0.8f);
            wall4.Texture = textureBuilding;

            floors = new Entity[20];
            floorNumbers = new BoundingBox[20];
            elevatorFronts = new BoundingBox[20];
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i] = new Entity(this, box);
                floors[i].Position = new Vector3(0, i * 50, 0);
                floors[i].Scale = new Vector3(0.99f, 0.02f, 0.99f);

                BoundingBox bb = floors[i].BB;
                bb.Max.Y += 40;
                floorNumbers[i] = bb;

                bb.Max.X -= 175;
                bb.Max.Z -= 150;
                elevatorFronts[i] = bb;
            }

            elevator = new Entity(this, box);
            elevator.Color = Color.LimeGreen;
            elevator.Scale = new Vector3(0.2f, 0.02f, 0.2f);
            elevator.Position = new Vector3(-120, 0, -78);
            
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
            for (int i = 0; i < floors.Length; i++)
            {
                staticEntities.Add(floors[i]);
            }
            staticEntities.Add(wall);
            staticEntities.Add(wall2);
            staticEntities.Add(wall3);
            staticEntities.Add(wall4);
            staticEntities.Add(elevator);
            movingEntities.Add(player);
            

            Camera = new Camera(this, player);

            Components.Add(terrain);
            Components.Add(skyDome);

            elevatorButtons = new TextButton[20];
            for (int i = 0; i < elevatorButtons.Length; i++)
            {
                elevatorButtons[i] = new TextButton(this, new Vector2(50 + i * 60, 650), (i+1)+"", bigFont);
                elevatorButtons[i].Click += (o, e) =>
                {
                    RequestElevator(int.Parse((o as TextButton).Text));
                };
                elevatorButtons[i].Visible = false;
                elevatorButtons[i].Enabled = false;
            }
        }

        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.E) &&
                prevKeyboard.IsKeyUp(Keys.E))
            {
                if (inFrontOfElevator)
                {
                    RequestElevator(currentFloor);
                }
            }
            prevKeyboard = keyboard;

            if (elevator.Position.Y != elevatorTargetY)
            {
                if (elevatorDirection == 1)
                {
                    if (elevator.Position.Y + elevatorSpeed * elevatorDirection * (float)gameTime.ElapsedGameTime.TotalSeconds > elevatorTargetY)
                    {
                        elevator.Position = new Vector3(elevator.Position.X, elevatorTargetY, elevator.Position.Z);
                    }
                    else
                    {
                        elevator.Position += new Vector3(0, elevatorSpeed * elevatorDirection * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    }
                }
                else
                {
                    if (elevator.Position.Y + elevatorSpeed * elevatorDirection * (float)gameTime.ElapsedGameTime.TotalSeconds < elevatorTargetY)
                    {
                        elevator.Position = new Vector3(elevator.Position.X, elevatorTargetY, elevator.Position.Z);
                    }
                    else
                    {
                        elevator.Position += new Vector3(0, elevatorSpeed * elevatorDirection * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    }
                }
            }
            
            info = "";
            foreach (var m in movingEntities)
            {
                bool found = false;
                inFrontOfElevator = false;
                for (int i = 0; i < floorNumbers.Length; i++)
                {
                    if (m.BB.Intersects(floorNumbers[i]))
                    {
                        floorNumber = "Du er i " + (i + 1) + ". etasje";
                        currentFloor = i + 1;
                        found = true;
                    }
                    if (m.BB.Intersects(elevatorFronts[i]))
                    {
                        floorNumber += "\n...og du stAr foran heisen.";
                        inFrontOfElevator = true;
                    }
                }
                if (!found)
                {
                    floorNumber = "";
                }
                onElevator = false;
                foreach (var s in staticEntities)
                {
                    if (m.BB.Intersects(s.BB))
                    {
                        if (s == elevator)
                        {
                            onElevator = true;
                            for (int i = 0; i < 20; i++)
                            {
                                elevatorButtons[i].Enabled = true;
                                elevatorButtons[i].Visible = true;
                            }
                        }

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
                if (!onElevator)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        elevatorButtons[i].Enabled = false;
                        elevatorButtons[i].Visible = false;
                    }
                }
            }            

            info += "\nX: " + player.Position.X + " Y: " + player.Position.Y + " Z: " + player.Position.Z;
            info += "\npMin: " + player.BB.Min + ", pMax: " + player.BB.Max;
            info += "\nf1Min: " + floor.BB.Min + ", f1Max: " + floor.BB.Max;
            //info += "\nf2Min: " + floor2.BB.Min + ", f2Max: " + floor2.BB.Max;
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

            bloom.BeginDraw();

            device.Clear(Color.Black);
            
            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, info, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(bigFont, floorNumber, new Vector2(600, 10), Color.White);
            spriteBatch.End();
        }
        
        private void RequestElevator(int floor)
        {
            elevatorTargetY = (floor - 1) * 50;
            if (elevator.Position.Y < elevatorTargetY)
            {
                elevatorDirection = 1;
            }
            else
            {
                elevatorDirection = -1;
            }
        }
    }
}
