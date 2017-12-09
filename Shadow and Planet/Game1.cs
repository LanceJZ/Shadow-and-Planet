using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Shadow_and_Planet.Entities;
using Engine;

namespace Shadow_and_Planet
{
    using PO = PositionedObject;
    using GameServices = Services;
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;

        Player ThePlayer;
        Base TheBase;
        Background TheBackground;
        AsteroidControl Asteroids;
        PirateControl Pirates;

        Words StartGameText;
        Words InstructionText1;
        Words InstructionText2;
        Words InstructionText3;
        Words InstructionText4;

        Song BackgroundMusic;

        bool BackgroundSongPlaying;
        bool BackgroundSongOn;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferWidth = 1200;
            Graphics.PreferredBackBufferHeight = 900;
            Graphics.PreferMultiSampling = true; //Error in MonoGame 3.6 for DirectX, fixed in next dev version.
            Graphics.PreparingDeviceSettings += SetMultiSampling;
            Graphics.ApplyChanges();
            Graphics.GraphicsDevice.RasterizerState = new RasterizerState(); //Must be after Apply Changes.
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";

            TheBase = new Base(this);
            ThePlayer = new Player(this, TheBase);
            Pirates = new PirateControl(this, ThePlayer);
            TheBackground = new Background(this);
            Asteroids = new AsteroidControl(this, ThePlayer, Pirates);

            StartGameText = new Words(this);
            InstructionText1 = new Words(this);
            InstructionText2 = new Words(this);
            InstructionText3 = new Words(this);
            InstructionText4 = new Words(this);
        }

        private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            PresentationParameters PresentParm = eventArgs.GraphicsDeviceInformation.PresentationParameters;
            PresentParm.MultiSampleCount = 8;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            BackgroundSongPlaying = false;
            BackgroundSongOn = true;
            // Positive Y is Up. Positive X is Right.
            GameServices.Initialize(Graphics, this, new Vector3(0, 0, 500), 0, 1000);
            // Setup lighting.
            GameServices.DefuseLight = new Vector3(0.6f, 0.5f, 0.7f);
            GameServices.LightDirection = new Vector3(-0.75f, -0.75f, -0.5f);
            GameServices.SpecularColor = new Vector3(0.1f, 0, 0.5f);
            GameServices.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f); // Add some overall ambient light.

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            BackgroundMusic = Content.Load<Song>("SongOne");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void BeginRun()
        {
            GameServices.BeginRun(); //This only happens once in a game.

            Vector3 pos = new Vector3(-210, 80, 150);
            Vector3 pos1 = new Vector3(-210, 240, 150);
            Vector3 pos2 = new Vector3(-210, 200, 150);
            Vector3 pos3 = new Vector3(-210, 160, 150);
            Vector3 pos4 = new Vector3(-300, -120, 150);

            StartGameText.ProcessWords("ANY KEY TO START GAME", pos, 2);
            InstructionText1.ProcessWords("L AND R TO TURN SHIP", pos1, 2);
            InstructionText2.ProcessWords("UP TO THRUST LCTRL TO FIRE", pos2, 2);
            InstructionText3.ProcessWords("LSHIFT TO FIRE MISSILE", pos3, 2);
            InstructionText4.ProcessWords("COLLECT CHEST TO GAIN MISSILE", pos4, 2);

            base.BeginRun();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!ThePlayer.Active)
            {
                StartGameText.ShowWords(true);
                InstructionText1.ShowWords(true);
                InstructionText2.ShowWords(true);
                InstructionText3.ShowWords(true);
                InstructionText4.ShowWords(true);

                if (Keyboard.GetState().GetPressedKeys().Length > 0)
                {
                    ThePlayer.NewGame();
                    Pirates.NewGame();
                    Asteroids.NewGame();
                    StartGameText.ShowWords(false);
                    InstructionText1.ShowWords(false);
                    InstructionText2.ShowWords(false);
                    InstructionText3.ShowWords(false);
                    InstructionText4.ShowWords(false);
                }


                if (BackgroundSongPlaying)
                {
                    MediaPlayer.Stop();
                    BackgroundSongPlaying = false;
                }
            }
            else
            {
                if (!BackgroundSongPlaying && BackgroundSongOn)
                {
                    MediaPlayer.Play(BackgroundMusic);
                    MediaPlayer.IsRepeating = true;
                    BackgroundSongPlaying = true;
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.O))
                    {
                        MediaPlayer.Stop();
                        BackgroundSongPlaying = false;
                        BackgroundSongOn = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.M))
                    {
                        BackgroundSongOn = true;
                    }
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(5, 0, 40));

            base.Draw(gameTime);
        }
    }
}
