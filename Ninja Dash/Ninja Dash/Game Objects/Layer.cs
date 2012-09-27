#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ninja_Dash
{
    class Layer
    {
        #region Fields

        //State determining if the layer will scroll horizontally or vertically
        public enum ScrollState
        {
            Horizontal,
            Vertical
        }

        Game curGame;
        SpriteBatch spriteBatch;

        //Layer texture to be rendered on the screen
        Texture2D layerTexture;

        //Position of layer
        public Vector2 Position;

        //Direction which the layer will scroll
        ScrollState direction;

        //Speed at which layer will scroll
        float scrollSpeed;

        public bool Active;

        #endregion

        #region Initialize

        public Layer(Game game, SpriteBatch spriteBatch, Texture2D texture, 
            Vector2 startPosition, float scrollSpeed, ScrollState direction)
        {
            curGame = (NinjaDashGame)game;
            layerTexture = texture;
            Position = startPosition;
            this.scrollSpeed = scrollSpeed;
            this.spriteBatch = spriteBatch;
            this.direction = direction;

            Active = true;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Active)
            {
                //Depending on which direction the layer will scroll, update position of layer
                if (direction == ScrollState.Horizontal)
                {
                    Position.X += scrollSpeed * elapsed;

                    if (Position.X > curGame.GraphicsDevice.Viewport.Width)
                    {
                        Position.X = -curGame.GraphicsDevice.Viewport.Width;
                    }
                    else if (Position.X < -curGame.GraphicsDevice.Viewport.Width)
                    {
                        Position.X = curGame.GraphicsDevice.Viewport.Width;
                    }
                }
                else if (direction == ScrollState.Vertical)
                {
                    Position.Y += scrollSpeed * elapsed;
                }
            }
        }

        #endregion

        #region Draw

        public void Draw()
        {
            if (Active)
            {
                //Render layer on to screen
                spriteBatch.Draw(layerTexture, Position, Color.White);
            }
        }

        #endregion
    }
}
