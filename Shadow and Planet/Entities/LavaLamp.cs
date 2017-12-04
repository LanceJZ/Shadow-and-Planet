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

    public class LavaLamp : Mod
    {


        public LavaLamp(Game game) : base(game)
        {

        }

        public override void Initialize()
        {
            RotationVelocity = SetRandomVelocity(5);

            base.Initialize();
            LoadContent();
        }

        public override void LoadContent()
        {
            LoadModel("LavaLamp");
            BeginRun();
        }

        public override void BeginRun()
        {

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
