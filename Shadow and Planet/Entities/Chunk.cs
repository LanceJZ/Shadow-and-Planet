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
        public Player PlayerRef;

        public Chunk(Game game) : base(game)
        {

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
