#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ninja_Dash
{
    class EnemyWithObject: Enemy
    {
        public enum NumberOfObjects { One, Two }

        Game curGame;
        SpriteBatch spriteBatch;
        SpriteEffects flip;

        AnimationStore animationStore;

        Player relativePlayer;

        TimeSpan totalTimeElapsed;
        TimeSpan timeDelay;

        NumberOfObjects numberOfObjects;

        public FlyingObject ninjaStar1;
        public FlyingObject ninjaStar2;

        bool soundEffectPlayed;

        public EnemyWithObject(Game game, Player player, SpriteBatch spriteBatch, NumberOfObjects numberOfObjects, AnimationStore animationStore)
            : base(game, new Animation(animationStore["EnemyRun"]), player, spriteBatch)
        {
            curGame = game;
            this.spriteBatch = spriteBatch;
            relativePlayer = player;

            this.numberOfObjects = numberOfObjects;
            this.animationStore = animationStore;

            totalTimeElapsed = TimeSpan.Zero;
            timeDelay = TimeSpan.FromSeconds(0.45);

            soundEffectPlayed = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            FlyingObject.Direction objectDirection;

            if (Position == leftPosition)
            {
                objectDirection = FlyingObject.Direction.Right;
                flip = SpriteEffects.None;
            }
            else
            {
                objectDirection = FlyingObject.Direction.Left;
                flip = SpriteEffects.FlipHorizontally;
            }

            ninjaStar1 = new FlyingObject(curGame, spriteBatch, new Animation(animationStore["NinjaStar"]),
                       Position, flip, Vector2.Zero, objectDirection);

            ninjaStar2 = new FlyingObject(curGame, spriteBatch, new Animation(animationStore["NinjaStar"]),
                       Position, flip, Vector2.Zero, objectDirection);

            switch (numberOfObjects)
            {
                case NumberOfObjects.One:
                    ninjaStar1.Velocity = new Vector2(150, 0);
                    ninjaStar2.Active = false;
                    break;

                case NumberOfObjects.Two:
                    ninjaStar1.Velocity = new Vector2(150, 0);
                    ninjaStar2.Velocity = new Vector2(150, -115);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            totalTimeElapsed += gameTime.ElapsedGameTime;

            if (totalTimeElapsed < timeDelay)
            {
                ninjaStar1.Position = Position;
                ninjaStar2.Position = Position;
            }
            else
            {
                if (!soundEffectPlayed)
                {
                    AudioManager.PlaySound("Shuriken_Throw");
                    soundEffectPlayed = true;
                }

                if (ninjaStar1.Active)
                {
                    ninjaStar1.Update(gameTime);
                }

                if (ninjaStar2.Active)
                {
                    ninjaStar2.Update(gameTime);
                }
            }
            
            base.Update(gameTime);
        }

        public override void Draw()
        {
            if (ninjaStar1.Active)
            {
                ninjaStar1.Draw();
            }

            if (ninjaStar2.Active)
            {
                ninjaStar2.Draw();
            }

            base.Draw();
        }
    }
}
