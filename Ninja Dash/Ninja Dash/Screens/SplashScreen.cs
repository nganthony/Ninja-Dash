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
    class SplashScreen: GameScreen
    {
        Texture2D splashScreenTexture;

        TimeSpan totalTime = TimeSpan.Zero;
        TimeSpan removeSplashScreenTime = TimeSpan.FromSeconds(3);

        public override void LoadContent()
        {
            splashScreenTexture = Load<Texture2D>("Textures/SplashScreen");
            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            totalTime += gameTime.ElapsedGameTime;

            if (totalTime > removeSplashScreenTime)
            {
                ScreenManager.RemoveScreen(this);
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(splashScreenTexture, Vector2.Zero, Color.White);

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
