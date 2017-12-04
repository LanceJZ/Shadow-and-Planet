using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;

namespace Shadow_and_Planet.Entities
{
    using Mod = AModel;

    public class Pirate : Mod
    {
        PositionedObject DetectPlayer;
        List<Missile> Missiles;
        List<Mod> HealthBar;
        Player PlayerRef;
        Timer ChaseTimer;
        Timer BumpTimer;
        Timer FireTimer;

        XnaModel HealthModel;

        SoundEffect ExplodSound;
        SoundEffect HitSound;
        SoundEffect BumpSound;

        Vector3 NewHeading = Vector3.Zero;

        int HitPoints;
        bool Stop;

        public Pirate(Game game, Player player) : base(game)
        {
            PlayerRef = player;
            ChaseTimer = new Timer(game, 10);
            BumpTimer = new Timer(game);
            FireTimer = new Timer(game);
            Missiles = new List<Missile>();
            HealthBar = new List<Mod>();
            DetectPlayer = new PositionedObject(game);
            DetectPlayer.Radius = 300;
            DetectPlayer.AddAsChildOf(this, true, true);
            LoadContent();
        }

        public override void Initialize()
        {
            Radius = 40;
            Scale = 2;

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Pirate");
            HealthModel = Load("cube - green");
            ExplodSound = LoadSoundEffect("PirateExplode");
            HitSound = LoadSoundEffect("PirateHit");
            BumpSound = LoadSoundEffect("PirateBump");

            BeginRun();
        }

        public override void BeginRun()
        {
            Reset();

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                CheckEdge();
                CheckCollusions();

                if (BumpTimer.Expired)
                {
                    if (BumpTimer.Enabled)
                    {
                        Rotation.X = 0;
                        Rotation.Y = 0;
                        RotationVelocity.X = 0;
                        RotationVelocity.Y = 0;
                        BumpTimer.Enabled = false;
                    }

                    if (ChaseTimer.Expired)
                        ChasePlayer();

                    RotationVelocity.Z = AimAtTarget(NewHeading, Rotation.Z, MathHelper.PiOver4);

                    if (!Stop)
                    {
                        Velocity = SetVelocityFromAngle(Rotation.Z, 100);
                    }
                    else
                    {
                        Velocity = Vector3.Zero;
                    }

                    CheckForPlayer();

                    if (FireTimer.Expired)
                    {
                        FireTimer.Reset(Services.RandomMinMax(3, 10));
                        FireMissile();
                    }
                }
            }

            base.Update(gameTime);
        }

        public void Bumped(Vector3 position, Vector3 velocity)
        {
            BumpSound.Play();
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += velocity * 0.75f;
            Velocity += SetVelocityFromAngle(AngleFromVectors(position, Position), 75);
            RotationVelocity.Z = Services.RandomMinMax(-MathHelper.PiOver4, MathHelper.PiOver4);
            RotationVelocity.X = Services.RandomMinMax(-MathHelper.PiOver4, MathHelper.PiOver4);
            RotationVelocity.Y = Services.RandomMinMax(-MathHelper.PiOver4, MathHelper.PiOver4);
            NewHeading = new Vector3(Services.RandomMinMax(-2000, 2000), Services.RandomMinMax(-1000, 1000), 0);
            BumpTimer.Enabled = true;
            BumpTimer.Reset(Services.RandomMinMax(2, 6));
        }

        public void Reset()
        {
            int lavaLamp = PlayerRef.OreinHold;
            HitPoints = (int)Services.RandomMinMax(5 + lavaLamp, 15 + lavaLamp);
            GenerateHealthBar();

            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
            Active = true;
        }

        public void GameOver()
        {
            Active = false;

            foreach (Mod bar in HealthBar)
            {
                bar.Active = false;
            }
        }

        public void CheckMissileHit(PositionedObject target)
        {
            foreach (Missile missile in Missiles)
            {
                if (missile.CirclesIntersect(target))
                {
                    missile.Active = false;
                }
            }
        }

        void GenerateHealthBar()
        {
            foreach (Mod bar in HealthBar)
            {
                bar.Active = false;
            }

            for (int n = 0; n < HitPoints; n++)
            {
                bool spawnNew = true;
                int freeBar = HealthBar.Count;

                for (int i = 0; i < HealthBar.Count; i++)
                {
                    if (!HealthBar[i].Active)
                    {
                        freeBar = i;
                        spawnNew = false;
                        break;
                    }
                }

                if (spawnNew)
                {
                    HealthBar.Add(new Mod(Game));
                    HealthBar[freeBar].AddAsChildOf(this, false, false);
                    HealthBar[freeBar].SetModel(HealthModel);
                    HealthBar[freeBar].Scale = 1.5f;
                    HealthBar[freeBar].DefuseColor = new Vector3(0, 1, 0);
                }

                HealthBar[freeBar].Position.X = -50;
                HealthBar[freeBar].Position.Y = n;
                HealthBar[freeBar].Active = true;
            }
        }

        void CheckForPlayer()
        {
            if (DetectPlayer.CirclesIntersect(PlayerRef))
            {
                Stop = true;
            }
            else
            {
                Stop = false;
            }
        }

        void FireMissile()
        {
            bool spawnNew = true;
            int freeOne = Missiles.Count;

            for (int i = 0; i < Missiles.Count; i ++)
            {
                if (!Missiles[i].Active)
                {
                    freeOne = i;
                    spawnNew = false;
                    break;
                }
            }

            if (spawnNew)
            {
                Missiles.Add(new Missile(Game));
            }

            Missiles[freeOne].Spawn(Position, Rotation, PlayerRef, 6);
        }

        void CheckEdge()
        {
            if (Position.X > 6000)
                Position.X = -6000;

            if (Position.X < -6000)
                Position.X = 6000;

            if (Position.Y > 4000)
                Position.Y = -4000;

            if (Position.Y < -4000)
                Position.Y = 4000;
        }

        void ChasePlayer()
        {
            ChaseTimer.Reset(9.9f - PlayerRef.OreinHold);
            NewHeading = PlayerRef.Position;
        }

        void CheckCollusions()
        {
            if (CirclesIntersect(PlayerRef))
            {
                PlayerRef.Bumped(Position, Velocity);
                Bumped(PlayerRef.Position, PlayerRef.Velocity);
            }

            if (PlayerRef.CheckShotCollusions(this))
            {
                HitSound.Play();
                HitPoints--;
                GenerateHealthBar();

                if (HitPoints < 1)
                {
                    Hit = true;
                    ExplodSound.Play();
                }
            }

            foreach(Missile missile in Missiles)
            {
                if (missile.Active)
                {
                    if (PlayerRef.CheckShotCollusions(missile))
                    {
                        missile.Active = false;
                    }
                }
            }
        }
    }
}
