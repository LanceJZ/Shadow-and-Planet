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

    public class Asteroid : Mod
    {
        Player PlayerRef;
        PirateControl PiratesRef;

        int HitPoints;

        public Asteroid(Game game, Player player, PirateControl pirate) : base(game)
        {
            PlayerRef = player;
            PiratesRef = pirate;
        }

        public override void Initialize()
        {
            Scale = 2;
            Radius = 100;
            ResetHitpoints();

            base.Initialize();

            LoadContent();
        }

        public override void LoadContent()
        {
            LoadModel("Asteroid");
            BeginRun();
        }

        public override void BeginRun()
        {
            Velocity = new Vector3(Services.RandomMinMax(-10, 10),
                Services.RandomMinMax(-10, 10), 0);

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            CheckCollusion();
            CheckEdge();

            base.Update(gameTime);
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

        void CheckCollusion()
        {
            if (CirclesIntersect(PlayerRef))
            {
                PlayerRef.Bumped(Position, Velocity);
            }

            foreach (Pirate pirate in PiratesRef.Pirates)
            {
                if (CirclesIntersect(pirate))
                {
                    pirate.Bumped(Position, Velocity);
                }
            }

            if (!Hit)
            {
                if (PlayerRef.CheckShotCollusions(this))
                {
                    HitPoints -= 1;

                    if (HitPoints < 1)
                    {
                        ResetHitpoints();
                        Hit = true;
                    }
                }
            }
        }

        public void Bumped(Vector3 position, Vector3 velocity)
        {
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += velocity * 0.75f;
            Velocity += SetVelocityFromAngle(AngleFromVectors(position, Position), 75);
        }

        void ResetHitpoints()
        {
            HitPoints = (int)Services.RandomMinMax(10, 20);
        }
    }
}
