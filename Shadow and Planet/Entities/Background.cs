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
    using Mod = Engine.AModel;

    public class Background : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        Engine.AModel[] Stars = new Engine.AModel[400];
        Engine.AModel[] StarsBack = new Engine.AModel[40];

        Vector3[] StarsOrg;

        public Background(Game game) : base(game)
        {
            StarsOrg = new Vector3[StarsBack.Length];

            game.Components.Add(this);
        }

        public override void Initialize()
        {

            float spinSpeed = 7.5f;

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i] = new Engine.AModel(Game);
            }

            for (int i = 0; i < StarsBack.Length; i++)
            {
                StarsBack[i] = new Engine.AModel(Game);
            }

            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].Position = new Vector3(Services.RandomMinMax(-6000, 6000), Services.RandomMinMax(-4000, 4000), -200);
                Stars[i].RotationVelocity = new Vector3(Services.RandomMinMax(-spinSpeed, spinSpeed),
                    Services.RandomMinMax(-spinSpeed, spinSpeed), Services.RandomMinMax(-spinSpeed, spinSpeed));
            }

            for (int i = 0; i < StarsBack.Length; i++)
            {
                StarsBack[i].Position = new Vector3(Services.RandomMinMax(-600, 600), Services.RandomMinMax(-450, 450), -900);
                StarsBack[i].RotationVelocity = new Vector3(Services.RandomMinMax(-spinSpeed, spinSpeed),
                    Services.RandomMinMax(-spinSpeed, spinSpeed), Services.RandomMinMax(-spinSpeed, spinSpeed));
                StarsBack[i].Scale = Services.RandomNumber.Next(1, 3);
                StarsOrg[i] = StarsBack[i].Position;
            }

            base.Initialize();
            Services.AddBeginable(this);
            Services.AddLoadable(this);
        }

        public void BeginRun()
        {

        }

        public void LoadContent()
        {
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].LoadModel("cube");
            }

            for (int i = 0; i < StarsBack.Length; i++)
            {
                StarsBack[i].LoadModel("cube");
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < StarsBack.Length; i++)
            {
                StarsBack[i].Position = StarsOrg[i] + Services.Camera.Position;
            }

            base.Update(gameTime);
        }
    }
}
