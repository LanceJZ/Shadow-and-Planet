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

    public class Chunk : Mod
    {
        Player PlayerRef;

        public Chunk(Game game, Player player) : base(game)
        {
            PlayerRef = player;
        }

        public override void Initialize()
        {
            Radius = 10;
            Scale = 2;

            base.Initialize();
            LoadContent();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Chunk");
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

                if (CheckForCollusion())
                {
                    Active = false;
                }
            }

            base.Update(gameTime);
        }

        public void Bumped(Vector3 position, Vector3 velocity)
        {
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += velocity * 0.75f;
            Velocity += VelocityFromVectors(position, Position, 75);
            SetRotation();
        }

        public void Spawn(Vector3 position)
        {
            Active = true;
            Position = position;
            Velocity = RandomVelocity(150);
            SetRotation();
        }

        void SetRotation()
        {
            RotationVelocity = new Vector3(Services.RandomMinMax(-1, 1), Services.RandomMinMax(-1, 1), 0);
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

        bool CheckForCollusion()
        {
            if (CirclesIntersect(PlayerRef))
            {
                if (PlayerRef.OrePickup())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
