#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Ninja_Dash
{
    class Enemy
    {
        #region Fields

        Viewport viewport;
        Game curGame;
        SpriteBatch spriteBatch;
        SpriteEffects Flip;

        Animation enemyAnimation;

        Player player;

        //Random number generator
        Random random;

        //Active state of the enemy
        public bool Active;

        //Enemy spawn positions
        protected Vector2 Position;
        protected Vector2 leftPosition;
        protected Vector2 rightPosition;

        //Initial Y position player was in during spawn
        //so that the enemy could be spawned relative to that position
        float initialYPosition;

        //Collision box surrounding the enemy
        public Rectangle CollisionRectangle
        {
            get
            {
                Rectangle rect = new Rectangle((int)Position.X - enemyAnimation.FrameWidth / 2, 
                    (int)Position.Y - enemyAnimation.FrameHeight / 2,
                    enemyAnimation.FrameWidth, enemyAnimation.FrameHeight);

                return rect;
            }
        }

        #endregion

        #region Initialize

        public Enemy(Game game, Animation enemyAnimation, Player player, SpriteBatch spriteBatch)
        {
            curGame = (NinjaDashGame)game;
            viewport = curGame.GraphicsDevice.Viewport;
            random = new Random();
            this.enemyAnimation = enemyAnimation;
            this.player = player;
            this.spriteBatch = spriteBatch;
            Active = true;
        }

        public virtual void Initialize()
        {
            //Set the positions which enemy can spawn
            leftPosition = new Vector2(player.marginLeft, player.Position.Y - player.marginVertical - player.verticalOffset);
            rightPosition = new Vector2(player.marginRight, player.Position.Y - player.marginVertical - player.verticalOffset);

            //Store the current Y position of player
            initialYPosition = player.Position.Y;

            int randomNumber = random.Next(1, 10);

            //Generate a random position for enemy spawn
            if (randomNumber <= 5)
            {
                Position = leftPosition;
                Flip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                Position = rightPosition;
                Flip = SpriteEffects.None;
            }
        }

        #endregion

        #region Update

        public virtual void  Update(GameTime gameTime)
        {
            //Update enemy animation
            enemyAnimation.Update(gameTime);

            //Set enemy to inactive if it has travelled double the screen height
            if (initialYPosition - player.Position.Y > viewport.Height * 2)
            {
                Active = false;
            }
        }

        #endregion

        #region Draw

        public virtual void Draw()
        {
            //Render enemy animation on to screen
            enemyAnimation.Draw(spriteBatch, Position, Flip);
        }

        #endregion
    }
}
