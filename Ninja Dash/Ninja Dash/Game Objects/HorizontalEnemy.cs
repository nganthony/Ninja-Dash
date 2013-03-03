#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace Ninja_Dash
{
    class HorizontalEnemy
    {
        public enum Direction { Left, Right }

        Game curGame;

        Texture2D horizontalEnemyPlatform;
        Animation animation;

        public Vector2 Position;

        float maxLeftPosition;
        float maxRightPosition;

        Player player;

        float initialYPosition;

        Direction direction;

        public int Speed = 150;

        SpriteEffects flip;

        public Rectangle CollisionRectangle
        {
            get
            {
                Rectangle rect = new Rectangle((int)Position.X - animation.FrameWidth / 2,
                    (int)Position.Y - animation.FrameHeight / 2, animation.FrameWidth,
                    animation.FrameHeight);

                return rect;
            }
        }

        public bool Active;
        public bool CollidedWithPlayer;

        Random random;

        public HorizontalEnemy(Game game, Animation animation, Player player, Texture2D platform)
        {
            curGame = game;
            this.animation = animation;
            this.player = player;
            initialYPosition = player.Position.Y;
            horizontalEnemyPlatform = platform;

            Active = true;
            CollidedWithPlayer = false;
            random = new Random();
        }

        public void Initialize()
        {
            
            maxLeftPosition = player.marginLeft + 30;
            maxRightPosition = player.marginRight - 30;

            int spawnXPosition = random.Next((int)maxLeftPosition, (int)maxRightPosition);

            Position = new Vector2(spawnXPosition, player.Position.Y - player.marginVertical - player.verticalOffset);

            int randomDirection = random.Next(1, 10);

            if (randomDirection <= 5)
            {
                direction = Direction.Left;
            }
            else if (randomDirection > 5)
            {
                direction = Direction.Right;
            }
        }

        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime);

            UpdateStates(gameTime);

            if (Position.X <= maxLeftPosition)
            {
                direction = Direction.Right;
            }
            else if (Position.X >= maxRightPosition)
            {
                direction = Direction.Left;
            }

            if (initialYPosition - player.Position.Y > curGame.GraphicsDevice.Viewport.Height * 2)
            {
                Active = false;
            }
        }

        public void UpdateStates(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (direction)
            {
                case Direction.Left:
                    Position.X -= Speed * elapsed;
                    flip = SpriteEffects.None;
                    break;

                case Direction.Right:
                    Position.X += Speed * elapsed;
                    flip = SpriteEffects.FlipHorizontally;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(horizontalEnemyPlatform, new Vector2(0, Position.Y + animation.FrameHeight / 2), null, Color.White, 0.0f,
                    Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            if (!CollidedWithPlayer)
            {
                animation.Draw(spriteBatch, Position, flip);
            }
        }
    }
}
