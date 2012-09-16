using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameStateManagement;

namespace Ninja_Dash
{
    class TutorialScreen : GameScreen
    {
        #region Fields

        //Texture for tutorial screen
        Texture2D tutorialTexture;

        #endregion

        #region Initialize

        public TutorialScreen()
        {
            EnabledGestures = GestureType.Tap;
        }

        public override void LoadContent()
        {
            //tutorialTexture = Load<Texture2D>("Textures/tutorialscreen");
        }

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            //Go back to the previous screen if the player presses the back button
            if (input.IsPauseGame(null))
            {
                Exit();
            }

            // Return to the main menu when a tap gesture is recognized
            if (input.Gestures.Count > 0)
            {
                GestureSample sample = input.Gestures[0];
                if (sample.GestureType == GestureType.Tap)
                {
                    Exit();

                    input.Gestures.Clear();
                }
            }
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Exit this screen.
        /// </summary>
        private void Exit()
        {
            ScreenManager.RemoveScreen(this);
            //ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            //Draw the tutorial texture onto the screen
            //ScreenManager.SpriteBatch.Draw(tutorialTexture, Vector2.Zero, new Color(255, 255, 255, TransitionAlpha));

            ScreenManager.SpriteBatch.End();
        }

        #endregion
    }
}
