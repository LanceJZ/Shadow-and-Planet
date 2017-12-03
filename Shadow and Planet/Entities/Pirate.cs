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
        List<Missile> Missiles;
        Player PlayerRef;
        Timer ChaseTimer;
        Timer BumpTimer;
        Timer FireTimer;

        Vector3 NewHeading = Vector3.Zero;

        int HitPoints;

        public Pirate(Game game, Player player) : base(game)
        {
            PlayerRef = player;
            ChaseTimer = new Timer(game, 10);
            BumpTimer = new Timer(game);
            FireTimer = new Timer(game);
            Missiles = new List<Missile>();
        }

        public override void Initialize()
        {
            Radius = 40;
            Scale = 2;
            Reset();
            base.Initialize();
            LoadContent();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Pirate");
            BeginRun();
        }

        public override void BeginRun()
        {

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
                    if (ChaseTimer.Expired)
                        ChasePlayer();

                    RotationVelocity.Z = AimAtTarget(NewHeading, Rotation.Z, MathHelper.PiOver4);
                    Velocity = SetVelocityFromAngle(Rotation.Z, 100);

                    if (FireTimer.Expired)
                    {
                        FireTimer.Reset(Services.RandomMinMax(3, 10));
                        FireMissile();
                    }

                    if (BumpTimer.Enabled)
                    {
                        Rotation.X = 0;
                        Rotation.Y = 0;
                        RotationVelocity.X = 0;
                        RotationVelocity.Y = 0;
                        BumpTimer.Enabled = false;
                    }
                }
            }

            base.Update(gameTime);
        }

        public void Bumped(Vector3 position, Vector3 velocity)
        {
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

        void FireMissile()
        {

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
                HitPoints--;

                if (HitPoints < 1)
                {
                    Hit = true;
                }
            }
        }

        void Reset()
        {
            HitPoints = (int)Services.RandomMinMax(5, 15);
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
        }
    }
}
