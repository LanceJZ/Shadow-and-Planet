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
        Numbers OreCollected;
        Numbers Score;
        Numbers Health;
        Words OreText;
        Words ScoreText;
        Words HealthText;

        KeyboardState KeyState;
        KeyboardState KeyStateOld;

        Shot[] Shots = new Shot[5];

        Mod Flame;

        public int OreinHold;
        public bool HoldFull;

        public Player(Game game) : base(game)
        {
            OreCollected = new Numbers(game);
            Score = new Numbers(game);
            Health = new Numbers(game);
            OreText = new Words(game);
            ScoreText = new Words(game);
            HealthText = new Words(game);
            Flame = new Mod(game);
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);

            Radius = 20;
            Scale = 1;

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
        }

        public override void BeginRun()
        {
            OreText.ProcessWords("ORE", Vector3.Zero, 2);
            OreText.Position.Z = 150;
            OreCollected.ProcessNumber(OreinHold, Vector3.Zero, 2);
            OreCollected.Position.Z = 150;

            Flame.AddAsChild(this, true, false);
            Flame.Position.X = -25;

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            Rotation.Y = 0;
            CheckEdge();
            GetInput();

            if (Position.X > 3000 || Position.X < -3000)
            {
                Velocity.X = (Velocity.X * 0.1f) * -1;
            }

            if (Position.Y > 2000 || Position.Y < -2000)
            {
                Velocity.Y = (Velocity.Y * 0.1f) * -1;
            }

            base.Update(gameTime);
            Services.Camera.Position.X = Position.X;
            Services.Camera.Position.Y = Position.Y;
            OreText.Position.X = Position.X - 150;
            OreText.Position.Y = Position.Y + 400;
            OreText.UpdatePosition();
            OreCollected.Position.X = Position.X;
            OreCollected.Position.Y = Position.Y + 400;
            OreCollected.UpdatePosition();
        }

        public void Bumped(Vector3 position, Vector3 velocity)
        {
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += velocity * 0.75f;
            Velocity += SetVelocityFromAngle(AngleFromVectors(position, Position), 75);
        }

        /// <summary>
        /// Return true if a shot hit object.
        /// </summary>
        /// <param name="po">Object to be tested against.</param>
        /// <returns>true if hit</returns>
        public bool CheckShotCollusions(PositionedObject po)
        {
            foreach (Shot shot in Shots)
            {
                if (shot.Active)
                {
                    if (shot.CirclesIntersect(po))
                    {
                        shot.Active = false;
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
                OreinHold++;
                OreCollected.UpdateNumber(OreinHold);
                return true;
            }
            else
                HoldFull = true;

            return false;
        }

        void CheckEdge()
        {
            if (Position.X > 3000)
                Position.X = -3000;

            if (Position.X < -3000)
                Position.X = 3000;

            if (Position.Y > 2000)
                Position.Y = -2000;

            if (Position.Y < -2000)
                Position.Y = 2000;
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

            KeyStateOld = KeyState;
        }

        void ThrustOn()
        {
            Acceleration = SetVelocity3FromAngleZ(Rotation.Z, 300);
            Flame.Visable = true;
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
                    Vector3 offset = SetVelocity(Rotation.Z, 20);
                    Vector3 dir = SetVelocity(Rotation.Z, 500);

                    shot.Spawn(Position + offset, Velocity * 0.75f + dir, 1);
                    break;
                }
            }
        }

    }
}
