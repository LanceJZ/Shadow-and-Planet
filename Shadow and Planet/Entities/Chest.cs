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

    public class Chest : Mod
    {
        Player PlayerRef;

        public Chest(Game game, Player player) : base(game)
        {
            PlayerRef = player;
        }

        public override void Initialize()
        {
            RotationVelocity = SetRandomVelocity(5);
            Radius = 20;

            base.Initialize();
            LoadContent();
        }

        public override void LoadContent()
        {
            LoadModel("SandP-Chest");
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
                if (CirclesIntersect(PlayerRef))
                {
                    Active = false;
                    PlayerRef.ChestPickup();
                }
            }

            base.Update(gameTime);
        }
    }
}
