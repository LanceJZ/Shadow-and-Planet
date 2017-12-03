using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using Engine;

namespace Shadow_and_Planet.Entities
{
    using Mod = Engine.AModel;

    public class PirateControl : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        Player PlayerRef;
        public List<Pirate> Pirates;
        List<Mod> PirateRadar;

        XnaModel PirateRadarModel;

        public PirateControl(Game game, Player player) : base(game)
        {
            PlayerRef = player;
            Pirates = new List<Pirate>();
            PirateRadar = new List<Mod>();

            game.Components.Add(this);
        }

        public override void Initialize()
        {
            Services.AddBeginable(this);
            Services.AddLoadable(this);

            base.Initialize();
        }

        public void BeginRun()
        {
            for (int i = 0; i < 4; i++)
            {
                Pirates.Add(new Pirate(Game, PlayerRef));
                ActivatePirateRadar();

                if (Services.RandomMinMax(0, 10) > 5)
                {
                    Pirates.Last().Position = new Vector3(Services.RandomMinMax(-3000, -2000),
                        Services.RandomMinMax(-2000, -1000), 0);
                }
                else
                {
                    Pirates.Last().Position = new Vector3(Services.RandomMinMax(3000, 2000),
                        Services.RandomMinMax(2000, 1000), 0);
                }
            }
        }

        public void LoadContent()
        {
            PirateRadarModel = PlayerRef.Load("cube - pirate");
        }

        public override void Update(GameTime gameTime)
        {
            CheckOtherPirateCollusion();

            base.Update(gameTime);
        }

        void ActivatePirateRadar()
        {
            PirateRadar.Add(new Mod(Game));
            PirateRadar.Last().SetModel(PirateRadarModel);
            PirateRadar.Last().DefuseColor = new Vector3(1, 0, 0);
            PirateRadar.Last().Scale = 4;
        }

        void DeactivatePirateRadar(int number)
        {
            PirateRadar[number].Active = false;
        }

        void CheckOtherPirateCollusion()
        {
            for (int i = 0; i < Pirates.Count; i++)
            {
                if (Pirates[i].Hit)
                {
                    DeactivatePirateRadar(i);
                    Pirates[i].Active = false;
                    Pirates[i].Hit = false;
                }

                if (Pirates[i].Active)
                {
                    Vector3 offset = PlayerRef.
                        SetVelocity(PlayerRef.AngleFromVectors(PlayerRef.Position, Pirates[i].Position), 60);
                    offset.Z = 250;
                    PirateRadar[i].Position = PlayerRef.Position + offset;
                }
            }

            foreach (Pirate pirateA in Pirates)
            {
                foreach (Pirate pirateB in Pirates)
                {
                    if (pirateA != pirateB)
                    {
                        if (pirateA.Active && pirateB.Active)
                        {
                            if (pirateA.CirclesIntersect(pirateB))
                            {
                                pirateA.Bumped(pirateB.Position, pirateB.Velocity);
                                pirateB.Bumped(pirateA.Position, pirateA.Velocity);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
