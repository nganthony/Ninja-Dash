#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ninja_Dash
{
    class FlyingObject
    {
        public enum Direction { Left, Right }

        Game curGame;
        SpriteBatch spriteBatch;

        protected Animation spriteAnimation;
        public Vector2 Position;
        public Vector2 Velocity;
        protected SpriteEffects Flip;

        protected Direction direction;

        public bool Active;

        public Rectangle CollisionRectangle
        {
            get
            {
                Rectangle rect = new Rectangle((int)Position.X - spriteAnimation.ScaledWidth / 2, 
                    (int)Position.Y - spriteAnimation.ScaledHeight / 2, 
                    spriteAnimation.ScaledWidth, spriteAnimation.ScaledHeight);

                return rect;
            }
        }

        public FlyingObject(Game game, SpriteBatch spriteBatch, Animation spriteAnimation, 
            Vector2 position, SpriteEffects flip, Vector2 velocity, Direction direction)
        {
            curGame = game;
            this.spriteBatch = spriteBatch;
            this.spriteAnimation = spriteAnimation;
            Position = position;
            Flip = flip;
            this.Velocity = velocity;
            this.direction = direction;

            Active = true;
        }

        public FlyingObject(Game game, SpriteBatch spriteBatch, Animation spriteAnimation)
        {
            curGame = game;
            this.spriteBatch = spriteBatch;
            this.spriteAnimation = spriteAnimation;

            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteAnimation.Update(gameTime);

            if (direction == Direction.Right)
                Position.X += Velocity.X * elapsed;
            else if (direction == Direction.Left)
                Position.X -= Velocity.X * elapsed;

            Position.Y += Velocity.Y * elapsed;

            if (Position.X > curGame.GraphicsDevice.Viewport.Width || Position.X < 0)
            {
                Active = false;
            }
        }

        public void Draw()
        {
            spriteAnimation.Draw(spriteBatch, Position, Flip);
        }
    }
}
