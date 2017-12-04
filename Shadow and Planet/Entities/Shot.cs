using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using Engine;

namespace Shadow_and_Planet.Entities
{
    using Mod = Engine.AModel;

    public class Shot : Mod
    {
        Timer LifeTimer;

        public Shot(Game game) : base(game)
        {
            LifeTimer = new Timer(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            Active = false;
            Radius = 2;
            Scale = 2;

            LoadContent();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            LoadModel("cube");
        }

        public override void Update(GameTime gameTime)
        {
            CheckEdge();

            if (LifeTimer.Expired)
                Active = false;

            base.Update(gameTime);
        }

        public void Spawn(Vector3 postion, Vector3 direction, float timer)
        {
            Active = true;
            Position = postion;
            Velocity = direction;
            Vector3 acc = Acceleration;
            LifeTimer.Reset(timer);
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
