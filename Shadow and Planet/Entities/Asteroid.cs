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
            LoadContent();
        }

        public override void Initialize()
        {
            Scale = 2;
            Radius = 100;

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("Asteroid");
            BeginRun();
        }

        public override void BeginRun()
        {
            Velocity = RandomVelocity(25);
            ResetHitpoints();

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
            if (Position.X > 6000)
                Position.X = -6000;

            if (Position.X < -6000)
                Position.X = 6000;

            if (Position.Y > 4000)
                Position.Y = -4000;

            if (Position.Y < -4000)
                Position.Y = 4000;
        }

        void CheckCollusion()
        {
            if (CirclesIntersect(PlayerRef))
            {
                PlayerRef.Bumped(Position, Velocity);
            }

            foreach (Pirate pirate in PiratesRef.Pirates)
            {
                if (pirate.Active)
                {
                    if (CirclesIntersect(pirate))
                    {
                        pirate.Bumped(Position, Velocity);
                    }

                    pirate.CheckMissileHit(this);
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
            Velocity += VelocityFromVectors(position, Position, 75);
        }

        void ResetHitpoints()
        {
            int patato = PlayerRef.OreinHold;
            HitPoints = (int)Services.RandomMinMax(20 + patato, 40 + patato);
        }
    }
}
