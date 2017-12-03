﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using Engine;

namespace Shadow_and_Planet.Entities
{
    using Mod = Engine.AModel;

    public class AsteroidControl : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        List<Asteroid> Asteroids;
        List<Chunk> Chunks;
        Player PlayerRef;
        PirateControl PiratesRef;

        public AsteroidControl(Game game, Player player, PirateControl pirate) : base(game)
        {
            Asteroids = new List<Asteroid>();
            Chunks = new List<Chunk>();

            PlayerRef = player;
            PiratesRef = pirate;

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);
            Services.AddLoadable(this);

            base.Initialize();
        }

        public void BeginRun()
        {
            for (int i = 0; i < 10; i++)
            {
                Asteroids.Add(new Asteroid(Game, PlayerRef, PiratesRef));
                PlayerRef.ActivateRockRadar();

                if (Services.RandomMinMax(0, 10) > 5)
                {
                    Asteroids.Last().Position = new Vector3(Services.RandomMinMax(-3000, -300),
                        Services.RandomMinMax(-2000, -200), 0);

                    Asteroids.Last().RotationVelocity = new Vector3(Services.RandomMinMax(-1, 1),
                        0, Services.RandomMinMax(-0.5f, 0.5f));
                }
                else
                {
                    Asteroids.Last().Position = new Vector3(Services.RandomMinMax(300, 3000),
                        Services.RandomMinMax(200, 2000), 0);

                    Asteroids.Last().RotationVelocity = new Vector3(Services.RandomMinMax(-1, 1),
                        0, Services.RandomMinMax(-0.5f, 0.5f));
                }
            }
        }

        public void LoadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            CheckOtherAsteroidCollusion();

            foreach (Asteroid rock in Asteroids)
            {
                if (rock.Hit)
                {
                    rock.Hit = false;
                    EjectChunk(rock);
                }
            }

            base.Update(gameTime);
        }

        void CheckOtherAsteroidCollusion()
        {
            foreach (Asteroid asteroidA in Asteroids)
            {
                foreach (Asteroid asteroidB in Asteroids)
                {
                    if (asteroidA != asteroidB)
                    {
                        if (asteroidA.CirclesIntersect(asteroidB))
                        {
                            asteroidA.Bumped(asteroidB.Position, asteroidB.Velocity);
                            asteroidB.Bumped(asteroidA.Position, asteroidA.Velocity);
                        }
                    }
                }
            }
        }

        void EjectChunk(Asteroid rock)
        {
            bool spawnNewChunk = true;
            int freeChunk = Chunks.Count;

            for (int chunk = 0; chunk < freeChunk; chunk++)
            {
                if (!Chunks[chunk].Active)
                {
                    spawnNewChunk = false;
                    freeChunk = chunk;
                    break;
                }
            }

            if (spawnNewChunk)
            {
                Chunks.Add(new Chunk(Game));
                Chunks.Last().PlayerRef = PlayerRef;
            }

            Chunks[freeChunk].Position = rock.Position;
            Chunks[freeChunk].Velocity = new Vector3(Services.RandomMinMax(-150, 150), Services.RandomMinMax(-150, 150), 0);
            Chunks[freeChunk].RotationVelocity = new Vector3(Services.RandomMinMax(-1, 1), Services.RandomMinMax(-1, 1), 0);
        }
    }
}
