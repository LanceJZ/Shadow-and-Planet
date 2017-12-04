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

    public class Base : Mod
    {
        Numbers OreAmount;
        Words OreText;

        public int OreOnBase = 0;

        public Base(Game game) : base(game) //TODO: Pirates will attack base from time to time.
        {
            OreAmount = new Numbers(game);
            OreText = new Words(game);
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);

            Radius = 25;
            Position.Z = -150;

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Base");
        }

        public override void BeginRun()
        {
            OreText.ProcessWords("BASE_ORE", Vector3.Zero, 2);
            OreText.Position.Z = 150;
            OreAmount.ProcessNumber(OreOnBase, Vector3.Zero, 2);
            OreAmount.Position.Z = 150;

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            OreText.Position.X = Services.Camera.Position.X - 200;
            OreText.Position.Y = Services.Camera.Position.Y - 400;
            OreText.UpdatePosition();
            OreAmount.Position.X = Services.Camera.Position.X;
            OreAmount.Position.Y = Services.Camera.Position.Y - 400;
            OreAmount.UpdatePosition();

            base.Update(gameTime);
        }

        public void LoadOre(int amount)
        {
            OreOnBase += amount;
            OreAmount.UpdateNumber(OreOnBase);
        }

        public void NewShip()
        {
            OreOnBase -= 15;
            OreAmount.UpdateNumber(OreOnBase);
        }
    }
}
