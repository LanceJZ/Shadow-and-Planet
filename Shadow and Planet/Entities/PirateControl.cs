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

        public PirateControl(Game game, Player player) : base(game)
        {
            PlayerRef = player;
            Pirates = new List<Pirate>();

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
                PlayerRef.ActivatePirateRadar();

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

        }

        public override void Update(GameTime gameTime)
        {
            CheckOtherPirateCollusion();

            base.Update(gameTime);
        }

        void CheckOtherPirateCollusion()
        {
            for (int i = 0; i < Pirates.Count; i++)
            {
                if (Pirates[i].Hit)
                {
                    PlayerRef.DeactivatePirateRadar(i);
                    Pirates[i].Active = false;
                    Pirates[i].Hit = false;
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
