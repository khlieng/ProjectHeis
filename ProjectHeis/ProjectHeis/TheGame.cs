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
//using Microsoft.Xna.Framework.Media.Song;

/**
 * Musikk og effekter er hentet fra: http://soundfxnow.com/sound-fx/elevator-bell/
 * 
 * 
 * 
 */
namespace ProjectHeis
{
    public class TheGame : Game
    {

        #region Variables
      // private SoundEffect ElevatortMusic;
       // private SoundEffect LiftDingDong;
        private Song ElevatortMusic;
        private Song ElevatorBell;

        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;

        public static BasicEffect Effect;
        private SpriteBatch spriteBatch;

        private KeyboardState prevKeyboard;

        private Model box;
        private Model treeModel;

        private Texture2D textureBuilding;

        private SpriteFont font;
        private SpriteFont bigFont;
        public static string info = "TEST";
        private string floorNumber = "";
        private string elevatorQueue = "";

        private Entity player;
        private Entity floor;
        private Entity[] floors;
        private ElevatorDoors[] doors;
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
        private int elevatorTargetFloor;
        private Queue<int> elevatorTargetFloors = new Queue<int>();
        private bool hasTarget;

        private List<Entity> movingEntities = new List<Entity>();
        private List<Entity> staticEntities = new List<Entity>();

        public static Camera Camera { get; private set; }

        private BloomComponent bloom; 
        #endregion

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

            Effect = new BasicEffect(GraphicsDevice);

            bloom = new BloomComponent(this);
            bloom.Settings = BloomSettings.PresetSettings[5];
            bloom.Initialize();
            Components.Add(bloom);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            font = Content.Load<SpriteFont>("Sprites/arial");
            bigFont = Content.Load<SpriteFont>("Sprites/big");

            box = Content.Load<Model>("Models/box");
            (box.Meshes[0].Effects[0] as BasicEffect).EnableDefaultLighting();
            treeModel = Content.Load<Model>("Models/tree");
            (treeModel.Meshes[0].MeshParts[0].Effect as BasicEffect).EnableDefaultLighting();

            textureBuilding = Content.Load<Texture2D>("Texture/Building_texture3");
           /*effect.TextureEnabled = true;
            effect.Texture = textureBuilding;*/

          //  ElevatortMusic = Content.Load<Song>("Audio\\elevatorMusic");
           // ElevatorBell = Content.Load<Song>("Audio\\ElevatorBell");


            #region Player
            player = new Entity(this, box);
            player.Position = new Vector3(0, 120, 0);
            player.Controller = new KeyboardController(player);
            player.Scale = new Vector3(0.01f, 0.02f, 0.01f);
            player.Gravity = true;
            player.Color = Color.Purple;
            #endregion

            #region floor
            floor = new Entity(this, box);
            floor.Scale = new Vector3(0.99f, 0.5f, 0.99f);
            floor.Position = new Vector3(0, -50, 0);
            #endregion

            #region walls
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
            #endregion

            floors = new Entity[20];
            doors = new ElevatorDoors[20];
            floorNumbers = new BoundingBox[20];
            elevatorFronts = new BoundingBox[20];
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i] = new Entity(this, box);
                floors[i].Position = new Vector3(0, i * 50, 0);
                floors[i].Scale = new Vector3(0.99f, 0.02f, 0.99f);

                doors[i] = new ElevatorDoors(this, i, box);

                BoundingBox bb = floors[i].BB;
                bb.Min.X -= 45;
                bb.Max.Y += 40;
                floorNumbers[i] = bb;

                bb.Min.X += 45;
                bb.Max.X -= 175;
                bb.Max.Z -= 150;
                elevatorFronts[i] = bb;
            }

            #region elevator
            elevator = new Entity(this, box);
            elevator.Color = Color.LimeGreen;
            elevator.Scale = new Vector3(0.2f, 0.02f, 0.2f);
            elevator.Position = new Vector3(-120, 0, -80);
            #endregion

            #region Tree model
            Entity tree = new Entity(this, treeModel);
            tree.Scale = new Vector3(0.15f);
            tree.Position = new Vector3(50, 10, 50);
            Entity tree2 = new Entity(this, treeModel);
            tree2.Scale = new Vector3(0.17f);
            tree2.Position = new Vector3(30, 5, 40);
            Entity tree3 = new Entity(this, treeModel);
            tree3.Scale = new Vector3(0.2f);
            tree3.Position = new Vector3(40, 5, 10);
            #endregion

            #region Skydome and Terrain
            Terrain terrain = new Terrain(this);
            terrain.Initialize();
            SkyDome skyDome = new SkyDome(this);
            skyDome.Initialize();
            #endregion

            #region AddDoors
            staticEntities.Add(floor);
            for (int i = 0; i < floors.Length; i++)
            {
                staticEntities.Add(floors[i]);
                staticEntities.Add(doors[i].LeftDoor);
                staticEntities.Add(doors[i].RightDoor);
                //staticEntities.Add(doors[i * 2]);
                //staticEntities.Add(doors[i * 2 + 1]);
            }
            #endregion

            #region add component
            staticEntities.Add(wall);
            staticEntities.Add(wall2);
            staticEntities.Add(wall3);
            staticEntities.Add(wall4);
            staticEntities.Add(elevator);
            movingEntities.Add(player);

            Components.Add(terrain);
            Components.Add(skyDome);
            #endregion

            #region elevatorButtons
            elevatorButtons = new TextButton[20];
            for (int i = 0; i < elevatorButtons.Length; i++)
            {
                elevatorButtons[i] = new TextButton(this, new Vector2(50 + i * 60, 650), (i+1)+"", bigFont);
                elevatorButtons[i].Click += (o, e) =>
                {
                    int targetFloor = int.Parse((o as TextButton).Text);
                    TrySelectFloor(targetFloor);
                };
                //elevatorButtons[i].Visible = true;
                elevatorButtons[i].Enabled = false;
                elevatorButtons[i].DrawOrder = 10;
            }
            #endregion

            #region ShaftWall(elevator shaft)
            Entity shaftWall1 = new Entity(this, box);
            shaftWall1.Position = new Vector3(-121, 450, -100);
            shaftWall1.Scale = new Vector3(0.21f, 5, 0.02f);
            //shaftWall1.Texture = Content.Load<Texture2D>("trans");
            Entity shaftWall2 = new Entity(this, box);
            shaftWall2.Position = new Vector3(-121, 450, -60);
            shaftWall2.Scale = new Vector3(0.21f, 5, 0.02f);
            //shaftWall2.Texture = Content.Load<Texture2D>("trans");
            shaftWall1.Alpha = 0.5f;
            shaftWall2.Alpha = 0.5f;
            #endregion
            staticEntities.Add(shaftWall1);
            staticEntities.Add(shaftWall2);

            Camera = new Camera(this, player);
            Camera.Initialize();
        }

        protected override void UnloadContent()
        {
        }

        int cd = 0;
        
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

            elevatorQueue = "Target: " + elevatorTargetFloor + "\nHeisQ:\n";
            foreach (var f in elevatorTargetFloors)
                elevatorQueue += f + "\n";
            
            if (cd > 0)
            {
                cd -= gameTime.ElapsedGameTime.Milliseconds;
                if (cd <= 0 && !hasTarget && elevatorTargetFloors.Count > 0)
                {
                    CloseDoors(currentFloor);
                    SetElevatorTargetFloor(elevatorTargetFloors.Dequeue());
                    hasTarget = true;
                }
            }
            else if(!hasTarget && elevatorTargetFloors.Count > 0)
            {
                CloseDoors(currentFloor);
                SetElevatorTargetFloor(elevatorTargetFloors.Dequeue());
                hasTarget = true;
            }

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

                if (elevator.Position.Y == elevatorTargetY)
                {
                    OpenDoors(elevatorTargetFloor);
                    hasTarget = false;
                    cd = 3000;
                }
            }
            
            

            base.Update(gameTime);

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
                        floorNumber += "\n...og du stAr foran heisen. Trykk (E) for Apne\n Velg din etasje med museklikk.";
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
                                //elevatorButtons[i].Visible = true;
                            }
                        }

                        Vector3 halfWidthMoving = (m.BB.Max - m.BB.Min) * 0.5f;
                        Vector3 halfWidthStatic = (s.BB.Max - s.BB.Min) * 0.5f;

                        Vector3 centerMoving = m.BB.Min + new Vector3(halfWidthMoving.X, halfWidthMoving.Y, halfWidthMoving.Z);
                        Vector3 centerStatic = s.BB.Min + new Vector3(halfWidthStatic.X, halfWidthStatic.Y, halfWidthStatic.Z); ;

                        Vector3 distance = centerMoving - centerStatic;

                        float[] overlap = { (halfWidthMoving.X + halfWidthStatic.X) - Math.Abs(distance.X),
                                            (halfWidthMoving.Y + halfWidthStatic.Y) - Math.Abs(distance.Y),
                                            (halfWidthMoving.Z + halfWidthStatic.Z) - Math.Abs(distance.Z) };

                        int min = Array.IndexOf(overlap, overlap.Min(o => Math.Abs(o)));
                        Vector3 projection;

                        switch (min)
                        {
                            case 0:
                                projection = new Vector3(distance.X > 0 ? overlap[min] : -overlap[min], 0, 0);
                                m.Position += projection;
                                break;

                            case 1:
                                projection = new Vector3(0, distance.Y > 0 ? overlap[min] : -overlap[min], 0);
                                m.Position += projection;
                                m.VelocityY = 0;
                                if (overlap[min] > 0)
                                {
                                    if (!m.Floored)
                                    {
                                        m.Floored = true;
                                    }
                                }
                                break;

                            case 2:
                                projection = new Vector3(0, 0, distance.Z > 0 ? overlap[min] : -overlap[min]);
                                m.Position += projection;
                                break;
                        }
                    }
                    if (m.VelocityY != 0)
                        m.Floored = false;
                }
                if (!onElevator)
                {
                    // if (MediaPlayer.State != MediaState.Stopped)
                    // MediaPlayer.Stop();

                    for (int i = 0; i < 20; i++)
                    {
                        elevatorButtons[i].Enabled = false;
                        //elevatorButtons[i].Visible = false;
                    }
                }
                else
                {
                    //if (MediaPlayer.State != MediaState.Playing)
                    // MediaPlayer.Play(ElevatortMusic);

                }
            }

            info += "\nX: " + player.Position.X + " Y: " + player.Position.Y + " Z: " + player.Position.Z;
            info += "\npMin: " + player.BB.Min + ", pMax: " + player.BB.Max;
            info += "\nf1Min: " + floor.BB.Min + ", f1Max: " + floor.BB.Max;
            //info += "\nf2Min: " + floor2.BB.Min + ", f2Max: " + floor2.BB.Max;
            info += "\nPlayer floored: " + player.Floored;

            Camera.Update(gameTime);
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
            spriteBatch.DrawString(font, elevatorQueue, new Vector2(1100, 10), Color.White);
            spriteBatch.End();
        }


        #region MethodsToElevator
        private void SetElevatorTargetFloor(int floor)
        {
            elevatorTargetFloor = floor;
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

        private void RequestElevator(int floor)
        {
            if (elevatorTargetFloors.Count < 1 || elevatorTargetFloors.Peek() != currentFloor)
            {
                elevatorTargetFloors.Enqueue(currentFloor);
            }
        }

        private void TrySelectFloor(int floor)
        {
            if (elevatorTargetFloors.Count > 0)
            {
                if (floor != currentFloor && elevatorTargetFloors.Peek() != floor && elevatorTargetFloors.Last() != floor)
                {
                    elevatorTargetFloors.Enqueue(floor);
                }
            }
            else if (floor != currentFloor)
            {
                elevatorTargetFloors.Enqueue(floor);
            }
        }

        private void OpenDoors(int floor)
        {
           // MediaPlayer.Play(ElevatorBell);
            doors[floor - 1].Open();
        }

        private void CloseDoors(int floor)
        {
            doors[floor - 1].Close();
        }
        #endregion
    }
}
