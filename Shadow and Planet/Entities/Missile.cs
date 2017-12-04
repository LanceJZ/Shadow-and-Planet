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

    public class Missile : Mod
    {
        PositionedObject TargetRef;
        Timer LifeTimer;

        public Missile(Game game) : base(game)
        {
            LifeTimer = new Timer(game);
            LoadContent();
        }

        public override void Initialize()
        {
            Active = false;
            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Missile");
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
                RotationVelocity.Z = AimAtTarget(TargetRef.Position, Rotation.Z, MathHelper.PiOver4 *0.25f);
                Velocity = SetVelocityFromAngle(Rotation.Z, 200);

                if (LifeTimer.Expired)
                {
                    Active = false;
                }

                CheckCollusions();
            }

            base.Update(gameTime);
        }

        public void Spawn(Vector3 postion, Vector3 rotation, PositionedObject target, float timer)
        {
            Active = true;
            Position = postion;
            Rotation.Z = rotation.Z;
            TargetRef = target;
            LifeTimer.Reset(timer);
        }

        void CheckCollusions()
        {
            if (CirclesIntersect(TargetRef))
            {
                TargetRef.Hit = true;
                Active = false;
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
    }
}
