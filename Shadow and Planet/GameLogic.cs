using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;
using Shadow_and_Planet.Entities;

namespace Shadow_and_Planet
{
    public enum GameState
    {
        Over,
        InPlay,
        HighScore,
        Attract
    };

    public class GameLogic : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
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

        GameState GameMode = GameState.Over;

        public Player PlayerRef { get => ThePlayer; }
        public Base BaseRef { get => TheBase; }
        public PirateControl PirirateRef { get => Pirates; }
        public GameState TheGameMode { get => GameMode; }

        public GameLogic(Game game) : base(game)
        {

            TheBase = new Base(game);
            TheBackground = new Background(game);
            ThePlayer = new Player(game, this);
            Pirates = new PirateControl(game, this);
            Asteroids = new AsteroidControl(game, this);

            StartGameText = new Words(game);
            InstructionText1 = new Words(game);
            InstructionText2 = new Words(game);
            InstructionText3 = new Words(game);
            InstructionText4 = new Words(game);

            game.Components.Add(this);
        }

        public override void Initialize()
        {

            base.Initialize();
            Services.AddLoadable(this);
            Services.AddBeginable(this);
        }

        public void LoadContent()
        {

        }

        public void BeginRun()
        {
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

        }

        public override void Update(GameTime gameTime)
        {
            GameStateSwitch();

            base.Update(gameTime);
        }

        public void GameOver()
        {
            StartGameText.ShowWords(true);
            InstructionText1.ShowWords(true);
            InstructionText2.ShowWords(true);
            InstructionText3.ShowWords(true);
            InstructionText4.ShowWords(true);
            BackgroundSongPlaying = false;
            //MediaPlayer.Stop();
            GameMode = GameState.Attract;
        }

        void GameStateSwitch()
        {
            switch(GameMode)
            {
                case GameState.Over:
                    GameOver();
                    break;

                case GameState.Attract:
                    Attract();
                    break;
                case GameState.InPlay:
                    GamePlay();
                    break;
            }
        }

        void Attract()
        {
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
                GameMode = GameState.InPlay;
                return;
            }

            if (!BackgroundSongPlaying && BackgroundSongOn)
            {
                //MediaPlayer.Play(BackgroundMusic);
                //MediaPlayer.IsRepeating = true;
                BackgroundSongPlaying = true;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.O))
                {
                    //MediaPlayer.Stop();
                    BackgroundSongPlaying = false;
                    BackgroundSongOn = false;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.M))
                {
                    BackgroundSongOn = true;
                }
            }
        }

        void GamePlay()
        {
            if (!ThePlayer.Active)
                GameMode = GameState.Over;
        }
    }
}
