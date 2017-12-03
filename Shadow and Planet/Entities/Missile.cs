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

        public Missile(Game game, Player player) : base(game)
        {
            TargetRef = player;
            LifeTimer = new Timer(game);
        }

        public override void Initialize()
        {
            Active = false;
            base.Initialize();
            LoadContent();
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
            }

            base.Update(gameTime);
        }

        public void Spawn(Vector3 postion, PositionedObject target, float timer)
        {
            Active = true;
            Position = postion;
            TargetRef = target;
            LifeTimer.Reset(timer);
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
    }
}
