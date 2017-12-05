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
    using Mod = AModel;

    public class Player : Mod
    {
        SoundEffect ThrustSound;
        SoundEffect FireSound;
        SoundEffect HitSound;
        SoundEffect ChestPickupSound;
        SoundEffect DeadSound;
        SoundEffect OreSound;
        SoundEffect DockSound;
        SoundEffect UnDockSound;
        SoundEffect ShotHitSound;

        Base BaseRef;
        Numbers OreCollected;
        Numbers Score;
        Numbers Health;
        Numbers ChestCollected;
        Numbers MissilesLeft;
        Words OreText;
        Words ScoreText;
        Words HealthText;
        Words ChestCollectedText;
        Words MissilesLeftText;
        List<Mod> HealthBar;
        List<Mod> MissileBar;
        XnaModel HealthModel;
        Explode Explosion;

        KeyboardState KeyState;
        KeyboardState KeyStateOld;

        Shot[] Shots = new Shot[5];
        List<Missile> Missiles;

        public PositionedObject MissileTarget;

        Mod Flame;
        Mod BaseRadar;

        Timer DockTimer;
        Timer ThrustTimer;

        public int OreinHold;
        public bool HoldFull;
        public int Chests;
        int HitPoints;
        bool Docked;

        public Player(Game game, Base theBase) : base(game)
        {
            BaseRef = theBase;
            OreCollected = new Numbers(game);
            Score = new Numbers(game);
            Health = new Numbers(game);
            ChestCollected = new Numbers(game);
            MissilesLeft = new Numbers(game);
            OreText = new Words(game);
            ScoreText = new Words(game);
            HealthText = new Words(game);
            ChestCollectedText = new Words(game);
            MissilesLeftText = new Words(game);
            Flame = new Mod(game);
            BaseRadar = new Mod(game);
            DockTimer = new Timer(game);
            ThrustTimer = new Timer(game);
            HealthBar = new List<Mod>();
            Explosion = new Explode(game);
            Missiles = new List<Missile>();
            MissileBar = new List<Mod>();
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);

            Radius = 20;
            Scale = 1;
            BaseRadar.Scale = 5f;
            BaseRadar.DefuseColor = new Vector3(0.5f, 0.45f, 0.55f);

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Player");
            Flame.LoadModel("SandP-PlayerFlame");

            for (int i = 0; i < 5; i++)
            {
                Shots[i] = new Shot(Game);
            }

            BaseRadar.LoadModel("cube");
            HealthModel = Load("cube");

            ThrustSound = LoadSoundEffect("Thrust");
            ThrustTimer.Amount = (float)ThrustSound.Duration.TotalSeconds;

            FireSound = LoadSoundEffect("PlayerShot");
            HitSound = LoadSoundEffect("PlayerHit");
            ChestPickupSound = LoadSoundEffect("ChestPickup");
            DeadSound = LoadSoundEffect("PlayerDead");
            OreSound = LoadSoundEffect("PickupChunk");
            DockSound = LoadSoundEffect("Dock");
            UnDockSound = LoadSoundEffect("UnDock");
            ShotHitSound = LoadSoundEffect("PlayerShotHit");
        }

        public override void BeginRun()
        {
            OreText.ProcessWords("ORE", new Vector3(-110, 400, 150), 2);
            OreText.Position.Z = 150;
            ChestCollectedText.ProcessWords("CHESTS", new Vector3(-400, 400, 150), 2);
            ChestCollectedText.Position.Z = 150;
            OreCollected.ProcessNumber(OreinHold, new Vector3(0, 400, 150), 2);
            OreCollected.Position.Z = 150;
            ChestCollected.ProcessNumber(Chests, new Vector3(-220, 400, 150), 2);
            ChestCollected.Position.Z = 150;
            MissilesLeftText.ProcessWords("MISSILES", new Vector3(110, 400, 150), 2);
            MissilesLeft.ProcessNumber(0, new Vector3(400, 400, 150), 2);

            Flame.AddAsChildOf(this, true, false);
            Flame.Position.X = -25;

            GenerateHealthBar();
            Active = false;

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                if (HitPoints < 1)
                {
                    if (BaseRef.OreOnBase < 15)
                    {
                        Dead();
                    }
                    else
                    {
                        BaseRef.NewShip();
                        Reset();
                    }
                }

                if (Hit)
                {
                    HitSound.Play();
                    HitPoints--;
                    GenerateHealthBar();
                    Hit = false;
                }

                if (Docked)
                {
                    Velocity = Vector3.Zero;
                    Acceleration = Vector3.Zero;
                    RotationVelocity = Vector3.Zero;
                    Flame.Visable = false;

                    if (DockTimer.Expired)
                    {
                        UnDockSound.Play();
                        BaseRef.LoadOre(OreinHold);
                        OreinHold = 0;
                        OreCollected.UpdateNumber(OreinHold);
                        Docked = false;
                    }
                }
                else
                {
                    CheckEdge();
                    GetInput();
                    CheckDocking();
                }

                base.Update(gameTime);
                Services.Camera.Position.X = Position.X;
                Services.Camera.Position.Y = Position.Y;
                OreText.Position.X = Position.X - 110;
                OreText.Position.Y = Position.Y + 400;
                OreText.UpdatePosition();
                ChestCollectedText.Position.X = Position.X - 400;
                ChestCollectedText.Position.Y = Position.Y + 400;
                ChestCollectedText.UpdatePosition();
                OreCollected.Position.X = Position.X;
                OreCollected.Position.Y = Position.Y + 400;
                OreCollected.UpdatePosition();
                ChestCollected.Position.X = Position.X - 220;
                ChestCollected.Position.Y = Position.Y + 400;
                ChestCollected.UpdatePosition();
                MissilesLeftText.Position.X = Position.X + 100;
                MissilesLeftText.Position.Y = Position.Y + 400;
                MissilesLeftText.UpdatePosition();
                MissilesLeft.Position.X = Position.X + 400;
                MissilesLeft.Position.Y = Position.Y + 400;
                MissilesLeft.UpdatePosition();

                if (!Docked)
                {
                    Vector3 offset = SetVelocity(AngleFromVectors(Position, BaseRef.Position), 40);
                    offset.Z = 250;
                    BaseRadar.Position = Position + offset;
                }

                BaseRef.Update(gameTime);
            }
        }

        public void Dead()
        {
            DeadSound.Play();
            Explosion.Spawn(Position, Radius * 0.25f);
            Active = false;
            Flame.Active = false;
            BaseRadar.Active = false;
            Services.Camera.Position.X = 0;
            Services.Camera.Position.Y = 0;
        }

        public void Reset()
        {
            HitPoints = 10;
            Position = Vector3.Zero;
        }

        public void NewGame()
        {
            Reset();
            Active = true;
            BaseRadar.Active = true;
            BaseRef.OreOnBase = 0;
            OreinHold = 0;
            OreCollected.UpdateNumber(OreinHold);
            Chests = 0;
            ChestCollected.UpdateNumber(Chests);
        }

        public void Bumped(Vector3 position, Vector3 velocity) //TODO: Make this part of PositionedObject class.
        {
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += velocity * 0.75f;
            Velocity += SetVelocityFromAngle(AngleFromVectors(position, Position), 75);
        }

        /// <summary>
        /// Return true if a shot hit object.
        /// </summary>
        /// <param name="target">Object to be tested against.</param>
        /// <returns>true if hit</returns>
        public bool CheckShotCollusions(PositionedObject target) //TODO: Use this in other games.
        {
            foreach (Shot shot in Shots)
            {
                if (shot.Active)
                {
                    if (shot.CirclesIntersect(target))
                    {
                        shot.HitTarget();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckMissileCollusions(PositionedObject target)
        {
            foreach(Missile missile in Missiles)
            {
                if (missile.Active)
                {
                    if (missile.CirclesIntersect(target))
                    {
                        missile.HitTarget();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool OrePickup()
        {
            if (OreinHold < 10)
            {
                OreSound.Play();
                OreinHold++;
                OreCollected.UpdateNumber(OreinHold);
                return true;
            }
            else
                HoldFull = true;

            return false;
        }

        public void ChestPickup()
        {
            ChestPickupSound.Play();
            Chests++;
            ChestCollected.UpdateNumber(Chests);
            UpdateMissileDisplay();
        }

        void UpdateMissileDisplay()
        {
            int numberOfMissiles = 0;
            int numberOfLiveMissiles = 0;

            foreach (Missile missile in Missiles)
            {
                if (missile.Active)
                    numberOfLiveMissiles++;
            }

            numberOfMissiles = Chests - numberOfLiveMissiles;

            MissilesLeft.UpdateNumber(numberOfMissiles);

            foreach (Mod missile in MissileBar)
            {
                missile.Active = false;
            }

            for (int mc = 0; mc < numberOfMissiles; mc++)
            {
                bool spawnNew = true;
                int freeOne = MissileBar.Count;

                for (int i = 0; i < MissileBar.Count; i++)
                {
                    if (!MissileBar[i].Active)
                    {
                        MissileBar[i].Active = true;
                        spawnNew = false;
                        break;
                    }
                }

                if (spawnNew)
                {
                    MissileBar.Add(new Missile(Game));
                    MissileBar.Last().AddAsChildOf(this, false, false);
                    MissileBar.Last().LoadModel("SandP-Missile");
                    MissileBar.Last().Scale = 4;
                }
            }

            float posX = 100;

            foreach (Mod missile in MissileBar)
            {
                if (missile.Active)
                {
                    missile.Position.X = posX;
                    missile.Position.Y = 400;
                    missile.Position.Z = 100;
                    posX += 20;
                    missile.Rotation.Z = MathHelper.PiOver2;
                }
            }
        }

        void FireMissile()
        {
            bool spawnNew = false;
            int freeOne = Missiles.Count;

            if (Missiles.Count < Chests)
            {
                spawnNew = true;
            }
            else
            {
                return;
            }

            for (int i = 0; i < Missiles.Count; i++)
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

            Missiles[freeOne].Spawn(Position, Rotation, MissileTarget, 10);
            UpdateMissileDisplay();
        }

        void CheckDocking()
        {
            if (OreinHold > 0)
            {
                if (CirclesIntersect(BaseRef))
                {
                    DockSound.Play();
                    Docked = true;
                    DockTimer.Reset(OreinHold);
                }
            }
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

        void GetInput()
        {
            KeyState = Keyboard.GetState();

            if (KeyState.IsKeyDown(Keys.Up))
                ThrustOn();
            else
            {
                Acceleration = Vector3.Zero;
                Flame.Visable = false;
            }

            if (KeyState.IsKeyDown(Keys.Right))
                RotateRight();
            else if (KeyState.IsKeyDown(Keys.Left))
                RotateLeft();
            else
                RotationVelocity = Vector3.Zero;

            if (!KeyStateOld.IsKeyDown(Keys.LeftControl))
            {
                if (KeyState.IsKeyDown(Keys.LeftControl))
                {
                    FireShot();
                }
            }

            if (!KeyStateOld.IsKeyDown(Keys.LeftShift))
            {
                if (KeyState.IsKeyDown(Keys.LeftShift))
                {
                    FireMissile();
                }
            }

            KeyStateOld = KeyState;
        }

        void ThrustOn()
        {
            Acceleration = SetVelocity3FromAngleZ(Rotation.Z, 300);
            Flame.Visable = true;

            if (ThrustTimer.Expired)
            {
                ThrustTimer.Reset();
                ThrustSound.Play();
            }
        }

        void RotateRight()
        {
            RotationVelocity.Z = -MathHelper.Pi;
        }

        void RotateLeft()
        {
            RotationVelocity.Z = MathHelper.Pi;
        }

        void FireShot()
        {
            foreach(Shot shot in Shots)
            {
                if (!shot.Active)
                {
                    FireSound.Play();
                    Vector3 offset = SetVelocity(Rotation.Z, 20);
                    Vector3 dir = SetVelocity(Rotation.Z, 500);

                    shot.Spawn(Position + offset, Velocity * 0.75f + dir, 1);
                    break;
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

    }
}
