using Microsoft.Xna.Framework;
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
        List<Mod> RockRadar;
        GameLogic GameLogicRef;
        Player PlayerRef;
        PirateControl PiratesRef;

        XnaModel RockRadarModel;

        public AsteroidControl(Game game, GameLogic gameLogic) : base(game)
        {
            Asteroids = new List<Asteroid>();
            Chunks = new List<Chunk>();
            RockRadar = new List<Mod>();

            GameLogicRef = gameLogic;
            PlayerRef = gameLogic.PlayerRef;
            PiratesRef = gameLogic.PirirateRef;

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
            for (int i = 0; i < 6; i++)
            {
                Asteroids.Add(new Asteroid(Game, PlayerRef, PiratesRef));
                ActivateRockRadar();

                if (Services.RandomMinMax(0, 10) > 5)
                {
                    Asteroids.Last().Position = new Vector3(Services.RandomMinMax(-4000, -2000),
                        Services.RandomMinMax(-4000, -2000), 0);

                    Asteroids.Last().RotationVelocity = new Vector3(Services.RandomMinMax(-1, 1),
                        0, Services.RandomMinMax(-0.5f, 0.5f));
                }
                else
                {
                    Asteroids.Last().Position = new Vector3(Services.RandomMinMax(2000, 4000),
                        Services.RandomMinMax(2000, 4000), 0);

                    Asteroids.Last().RotationVelocity = new Vector3(Services.RandomMinMax(-1, 1),
                        0, Services.RandomMinMax(-0.5f, 0.5f));
                }
            }
        }

        public void LoadContent()
        {
            RockRadarModel = PlayerRef.Load("core/cube");
        }

        public override void Update(GameTime gameTime)
        {
            CheckOtherAsteroidCollusion();
            int i = 0;

            foreach (Asteroid rock in Asteroids)
            {
                if (rock.Hit)
                {
                    rock.Hit = false;
                    EjectChunk(rock);
                }

                foreach(Pirate pirate in PiratesRef.Pirates)
                {
                    pirate.CheckMissileHit(rock);

                    foreach (Chunk chunk in Chunks)
                    {
                        if (chunk.Active)
                        {
                            if (rock.CirclesIntersect(chunk))
                            {
                                chunk.Bumped(rock.Position, rock.Velocity);
                            }

                            if (pirate.Active)
                            {
                                if (pirate.CirclesIntersect(chunk))
                                {
                                    pirate.Bumped(chunk.Position, chunk.Velocity);
                                    chunk.Bumped(pirate.Position, pirate.Velocity);
                                }
                            }
                        }

                    }
                }

                if (PlayerRef.Active)
                {
                    Vector3 offset = PlayerRef.
                        VelocityFromVectors(PlayerRef.Position, rock.Position, 80);
                    offset.Z = 250;
                    RockRadar[i].Position = PlayerRef.Position + offset;
                    i++;
                }
                else
                {
                    foreach(Mod radar in RockRadar)
                    {
                        radar.Active = false;
                    }
                }
            }

            base.Update(gameTime);
        }

        public void NewGame()
        {
            foreach(Mod radar in RockRadar)
            {
                radar.Active = true;
            }
        }

        void ActivateRockRadar()
        {
            RockRadar.Add(new Mod(Game));
            RockRadar.Last().SetModel(RockRadarModel);
            RockRadar.Last().Scale = 1.5f;
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
                Chunks.Add(new Chunk(Game, PlayerRef));
            }

            Chunks[freeChunk].Spawn(rock.Position);
        }
    }
}
