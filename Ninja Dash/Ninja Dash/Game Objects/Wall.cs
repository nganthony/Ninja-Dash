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
    class Wall
    {
        Game curGame;
        Viewport viewport;
        SpriteBatch spriteBatch;

        Texture2D wallTexture;

        Vector2 wall1Position;
        Vector2 wall2Position;
        Vector2 restartPosition;

        SpriteEffects flip;

        public float xCoordinate;

        public bool FinishedTransition = false;

        float scrollSpeed;
        public float horizontalScrollSpeed = 100;

        public int Width
        {
            get { return wallTexture.Width; }
        }

        public int Height
        {
            get { return wallTexture.Height; }
        }

        public float ScrollSpeed
        {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        public Wall(Game game, SpriteBatch spriteBatch, Texture2D texture, float xCoordinate, float scrollSpeed, SpriteEffects flip) 
        {
            curGame = game;
            this.spriteBatch = spriteBatch;
            wallTexture = texture;

            this.scrollSpeed = scrollSpeed;
            this.xCoordinate = xCoordinate;
            this.flip = flip;
        }

        public void Initialize()
        {
            viewport = curGame.GraphicsDevice.Viewport;

            wall1Position = new Vector2(xCoordinate, -viewport.Height);
            wall2Position = new Vector2(xCoordinate, 0);

            restartPosition = new Vector2(xCoordinate, -viewport.Height);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            wall1Position.X = xCoordinate;
            wall2Position.X = xCoordinate;

            if (FinishedTransition)
            {
                wall1Position.Y += scrollSpeed * elapsed;
                wall2Position.Y += scrollSpeed * elapsed;
            }

            if (wall1Position.Y > viewport.Height)
            {
                wall1Position = restartPosition;
            }

            if (wall2Position.Y > viewport.Height)
            {
                wall2Position = restartPosition;
            }
        }

        public void Draw()
        {
            spriteBatch.Draw(wallTexture, wall1Position, null, Color.White, 0.0f,
                    Vector2.Zero, 1.0f, flip, 0.0f);

            spriteBatch.Draw(wallTexture, wall2Position, null, Color.White, 0.0f,
                    Vector2.Zero, 1.0f, flip, 0.0f);
        }
    }
}
