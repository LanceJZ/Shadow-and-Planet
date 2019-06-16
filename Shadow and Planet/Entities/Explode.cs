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
    public class Explode : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        List<ExplodeParticle> Particles;
        bool _Active;

        public bool Active { get => _Active; set => _Active = value; }

        public Explode(Game game) : base(game)
        {
            Particles = new List<ExplodeParticle>();

            game.Components.Add(this);
            LoadContent();
            BeginRun();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {

        }

        public void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (_Active)
            {
                bool done = true;

                foreach(ExplodeParticle particle in Particles)
                {
                    if (particle.Active)
                    {
                        done = false;
                        break;
                    }
                }

                if (done)
                    _Active = false;

                base.Update(gameTime);
            }
        }

        public void Spawn(Vector3 position, float radius)
        {
            _Active = true;
            int count = (int)Services.RandomMinMax(10, 10 + radius * 2);

            if (count > Particles.Count)
            {
                int more = count - Particles.Count;

                for (int i = 0; i < more; i++)
                {
                    Particles.Add(new ExplodeParticle(Game));
                }
            }

            foreach (ExplodeParticle particle in Particles)
            {
                position += new Vector3(Services.RandomMinMax(-radius, radius),
                    Services.RandomMinMax(-radius, radius), 0);

                particle.Spawn(position);
            }
        }
    }
}
