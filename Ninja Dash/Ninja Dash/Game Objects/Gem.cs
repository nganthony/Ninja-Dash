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
    class Gem
    {
        Animation gemAnimation;

        public Vector2 Position;

        public bool Active;

        Player player;

        float initialYPosition;

        Game curGame;

        public Rectangle CollisionRectangle
        {
            get
            {
                Rectangle rect;
                rect = new Rectangle((int)Position.X - gemAnimation.FrameWidth / 2, 
                    (int)Position.Y - gemAnimation.FrameHeight / 2,
                    gemAnimation.FrameWidth, gemAnimation.FrameHeight);

                return rect;
            }
        }

        public Gem(Game game, Animation animation, Vector2 position, Player player)
        {
            curGame = game;
            gemAnimation = animation;
            Position = position;
            this.player = player;
            Active = true;

            initialYPosition = player.Position.Y;
        }

        public void Update(GameTime gameTime)
        {
            gemAnimation.Update(gameTime);

            //Set enemy to inactive if it has travelled double the screen height
            if (initialYPosition - player.Position.Y > curGame.GraphicsDevice.Viewport.Height * 2)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            gemAnimation.Draw(spriteBatch, Position, SpriteEffects.None);
        }
    }
}
