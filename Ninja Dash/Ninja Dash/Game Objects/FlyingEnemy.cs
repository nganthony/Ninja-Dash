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
    class FlyingEnemy: FlyingObject
    {
        Vector2 leftPosition;
        Vector2 rightPosition;

        static Vector2 flyingEnemyVelocity = new Vector2(350, 0);

        //Random number generator
        Random random;

        public FlyingEnemy(Game game, SpriteBatch spriteBatch, Animation spriteAnimation, Player relativePlayer)
            : base(game, spriteBatch, spriteAnimation)
        {
            random = new Random();

            leftPosition = new Vector2(0, relativePlayer.Position.Y - 700);
            rightPosition = new Vector2(game.GraphicsDevice.Viewport.Width, relativePlayer.Position.Y - 700);

            int randomNumber = random.Next(1, 10);

            if (randomNumber <= 5)
            {
                Position = leftPosition;
                Flip = SpriteEffects.FlipHorizontally;
                direction = Direction.Right;
            }

            else
            {
                Position = rightPosition;
                Flip = SpriteEffects.None;
                direction = Direction.Left;
            }

            Velocity = flyingEnemyVelocity;
        }
    }
}
