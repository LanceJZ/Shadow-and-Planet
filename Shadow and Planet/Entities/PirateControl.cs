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

    public class PirateControl : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        Player PlayerRef;
        public List<Pirate> Pirates;
        List<Mod> PirateRadar;
        List<LavaLamp> LavaLamps;
        List<Chest> Chests;
        List<Explode> Explosions;

        XnaModel PirateRadarModel;

        SoundEffect LavaLampSound;

        public PirateControl(Game game, Player player) : base(game)
        {
            PlayerRef = player;
            Pirates = new List<Pirate>();
            PirateRadar = new List<Mod>();
            LavaLamps = new List<LavaLamp>();
            Chests = new List<Chest>();
            Explosions = new List<Explode>();

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
        }

        public void LoadContent()
        {
            PirateRadarModel = PlayerRef.Load("core/cube");
            LavaLampSound = PlayerRef.LoadSoundEffect("LavaLampDrop");
        }

        public override void Update(GameTime gameTime)
        {
            CheckPirateCollusion();

            int PiratesActive = 0;

            foreach (Pirate pirate in Pirates)
            {
                if (pirate.Active)
                    PiratesActive++;
            }

            if (PlayerRef.Chests + 1 > PiratesActive)
                SpawnPirate();

            base.Update(gameTime);
        }

        public bool CheckChestPickup(PositionedObject target)
        {
            foreach(Chest chest in Chests)
            {
                if (chest.Active && target.Active)
                {
                    if(chest.CirclesIntersect(target))
                    {
                        chest.Active = false;
                        return true;
                    }
                }
            }

            return false;
        }

        public void NewGame()
        {

        }

        public void GameOver()
        {
            foreach(Pirate pirate in Pirates)
            {
                pirate.GameOver();
            }
        }

        void ActivatePirateRadar()
        {
            PirateRadar.Add(new Mod(Game));
            PirateRadar.Last().SetModel(PirateRadarModel);
            PirateRadar.Last().DefuseColor = new Vector3(1, 0, 0);
            PirateRadar.Last().Scale = 2;
        }

        void DeactivatePirateRadar(int number)
        {
            PirateRadar[number].Active = false;
        }

        void SpawnExploision(Vector3 position, float radius)
        {
            bool spawnNew = true;
            int freeExplosion = Explosions.Count;

            for (int i = 0; i < Explosions.Count; i++)
            {
                if (!Explosions[i].Active)
                {
                    spawnNew = false;
                    freeExplosion = i;
                    break;
                }
            }

            if (spawnNew)
            {
                Explosions.Add(new Explode(Game));
            }

            Explosions[freeExplosion].Spawn(position, radius);
        }

        void CheckPirateCollusion()
        {
            for (int i = 0; i < Pirates.Count; i++)
            {
                if (Pirates[i].Hit && Pirates[i].Active)
                {
                    SpawnExploision(Pirates[i].Position, Pirates[i].Radius * 0.25f);
                    DeactivatePirateRadar(i);
                    Pirates[i].Active = false;
                    Pirates[i].Hit = false;

                    if (Services.RandomMinMax(1, 7) > 4)
                        SpawnChest(Pirates[i].Position);

                    if (Services.RandomMinMax(1, 100) > 95)
                        SpawnLavaLamp(Pirates[i].Position);
                }

                if (PlayerRef.Active)
                {
                    if (Pirates[i].Active)
                    {
                        Vector3 offset = PlayerRef.VelocityFromVectors(PlayerRef.Position,
                            Pirates[i].Position, 60);
                        offset.Z = 250;
                        PirateRadar[i].Position = PlayerRef.Position + offset;
                    }
                }
                else
                {
                    foreach (Mod radar in PirateRadar)
                    {
                        radar.Active = false;
                        GameOver();
                    }
                }

                if (Pirates[i].CheckMissileHit(PlayerRef))
                {
                    PlayerRef.Hit = true;
                }
            }

            foreach (Pirate pirateA in Pirates)
            {
                foreach (Pirate pirateB in Pirates)
                {
                    if (pirateA != pirateB)
                    {
                        if (pirateA.Active && pirateB.Active)
                        {
                            if (pirateA.CirclesIntersect(pirateB))
                            {
                                pirateA.Bumped(pirateB.Position, pirateB.Velocity);
                                pirateB.Bumped(pirateA.Position, pirateA.Velocity);
                                return;
                            }
                        }
                    }
                }
            }
        }

        void SpawnPirate()
        {
            bool spawnNew = true;
            int freePirate = Pirates.Count;

            for (int i = 0; i < Pirates.Count; i++)
            {
                if (!Pirates[i].Active)
                {
                    spawnNew = false;
                    freePirate = i;
                    break;
                }
            }

            if (spawnNew)
            {
                Pirates.Add(new Pirate(Game, PlayerRef));
                ActivatePirateRadar();
            }

            if (Services.RandomMinMax(0, 10) > 5)
            {
                Pirates[freePirate].Position = new Vector3(Services.RandomMinMax(-4000, -3000),
                    Services.RandomMinMax(-4000, -3000), 0);
            }
            else
            {
                Pirates[freePirate].Position = new Vector3(Services.RandomMinMax(4000, 3000),
                    Services.RandomMinMax(4000, 3000), 0);
            }

            PirateRadar[freePirate].Active = true;
            Pirates[freePirate].Reset();
        }

        void SpawnLavaLamp(Vector3 position)
        {
            LavaLampSound.Play();
            bool spawnNew = true;
            int freeLamp = LavaLamps.Count;

            for (int i = 0; i < LavaLamps.Count; i++)
            {
                if (!LavaLamps[i].Active)
                {
                    spawnNew = false;
                    freeLamp = i;
                    break;
                }
            }

            if (spawnNew)
            {
                LavaLamps.Add(new LavaLamp(Game));
            }

            LavaLamps[freeLamp].Position = position;
            LavaLamps[freeLamp].Position.X += Services.RandomMinMax(-30, 30);
            LavaLamps[freeLamp].Position.Y += Services.RandomMinMax(-30, 30);
        }

        void SpawnChest(Vector3 position)
        {
            bool spawnNew = true;
            int freeChest = Chests.Count;

            for (int i = 0; i < Chests.Count; i++)
            {
                if (!Chests[i].Active)
                {
                    spawnNew = false;
                    freeChest = i;
                    break;
                }
            }

            if (spawnNew)
            {
                Chests.Add(new Chest(Game, PlayerRef));
            }

            Chests[freeChest].Position = position;
            Chests[freeChest].Active = true;
        }
    }
}
