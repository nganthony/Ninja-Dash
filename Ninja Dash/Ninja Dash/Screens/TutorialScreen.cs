#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameStateManagement;

namespace Ninja_Dash
{
    class TutorialScreen : MenuScreen
    {
        #region Fields

        Viewport viewport;

        //Texture for tutorial screen
        Texture2D tutorialTexture;
        Texture2D buttonBackTexture;

        //Buttons
        Button buttonBack;

        #endregion

        #region Initialize

        public TutorialScreen()
        {
            EnabledGestures = GestureType.Tap;
        }

        public override void LoadContent()
        {
            //Initialize viewport
            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            tutorialTexture = Load<Texture2D>("Textures/TutorialScreen");
            buttonBackTexture = Load<Texture2D>("Textures/Buttons/ButtonBack");

            //Initialize back button
            buttonBack = new Button();
            buttonBack.Initialize(buttonBackTexture, new Vector2(50, viewport.Height - 50), 1.0f);
            buttonBack.Selected += new EventHandler(buttonBack_Selected);

            //Add Menu Buttons
            MenuButtons.Add(buttonBack);
        }

        #endregion

        //Back button event handler
        public void buttonBack_Selected(object sender, EventArgs e)
        {
            Exit();
        }

        #region Handle Input

        //Event handler for hardware back button
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            Exit();
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Exit this screen.
        /// </summary>
        private void Exit()
        {
            ScreenManager.RemoveScreen(this);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            //Draw the tutorial texture onto the screen
            ScreenManager.SpriteBatch.Draw(tutorialTexture, Vector2.Zero, Color.White);

            ScreenManager.SpriteBatch.End();

            //Create a fade effect when the screen is transitioning on
            ScreenManager.FadeBackBufferToBlack(TransitionPosition);

            base.Draw(gameTime);
        }

        #endregion
    }
}
