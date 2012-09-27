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
    class Obstacle
    {
        public enum SpawnSide
        {
            Left,
            Right
        }

        #region Fields

        Viewport viewport;
        Game curGame;

        //Allows obstacles to spawn relative to the position of the main player
        Player player;

        Texture2D texture;
        public Vector2 Position;
        SpriteEffects flip;

        //Positions for obstacle to start in
        public Vector2 leftPosition;
        public Vector2 rightPosition;

        //Initial y-position the player was in 
        //immediately after the obstacle spawned
        float initialYPosition;

        //Active state
        public bool Active;

        //Rectangle surrounding the object used for collision handling
        public Rectangle CollisionRectangle
        {
            get
            {
                Rectangle rect = new Rectangle((int)Position.X, 
                    (int)Position.Y, texture.Width, texture.Height);

                return rect;
            }
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        #endregion

        #region Initialization

        public Obstacle(Game game)
        {
            curGame = (NinjaDashGame)game;
        }

        public void Initialize(Texture2D obstacleTexture, Player player, SpawnSide side)
        {
            viewport = curGame.GraphicsDevice.Viewport;

            texture = obstacleTexture;
            this.player = player;

            //Initially set the object to active
            Active = true;

            //Set starting positions of the object
            leftPosition = new Vector2(0, player.Position.Y - player.marginVertical - player.verticalOffset);
            rightPosition = new Vector2(viewport.Width - texture.Width, player.Position.Y - player.marginVertical - player.verticalOffset);

            if (side == SpawnSide.Left)
            {
                flip = SpriteEffects.None;
                Position = leftPosition;
            }
            else if (side == SpawnSide.Right)
            {
                flip = SpriteEffects.FlipHorizontally;
                Position = rightPosition;
            }

            initialYPosition = player.Position.Y;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            //Set obstacle to inactive when it exceeds the screen
            if (initialYPosition - player.Position.Y > viewport.Height + 1000)
            {
                Active = false;
            }
        }

        #endregion

        #region Render

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f,
                    Vector2.Zero, 1.0f, flip, 0.0f);
        }

        #endregion
    }
}
