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

    public class ExplodeParticle : Mod
    {
        Timer LifeTimer;

        public ExplodeParticle(Game game) : base(game)
        {
            LifeTimer = new Timer(game);

            LoadContent();
            BeginRun();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("Cube");
        }

        public override void BeginRun()
        {

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            if (LifeTimer.Expired)
                Active = false;

            base.Update(gameTime);
        }

        public void Spawn(Vector3 position)
        {
            Velocity = RandomVelocity(50);
            Position = position;
            Active = true;
            Scale = Services.RandomMinMax(1, 2);
            LifeTimer.Reset(Services.RandomMinMax(0.1f, 1));
        }
    }
}
