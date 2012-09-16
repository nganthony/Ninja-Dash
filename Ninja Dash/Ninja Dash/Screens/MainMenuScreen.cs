using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;

namespace Ninja_Dash
{
    class MainMenuScreen: MenuScreen
    {

        #region Fields

        Viewport viewport;

        //Textures
        Texture2D backgroundTexture;
        Texture2D stringTexture;

        //Position of textures
        Vector2 stringPosition;
        Vector2 backgroundPosition;

        //Button textures
        Texture2D buttonHighscoresTexture;
        Texture2D buttonOptionsTexture;

        //Buttons
        Button buttonHighscores;
        Button buttonOptions;

        //Alpha variables for fade in, fade out text
        float mAlphaFactor = 0.0f;
        float mFadeIncrement = 0.10f;
        double mFadeDelay = .035;

        //Speed to scroll out background
        const float backgroundScrollSpeed = 30.0f;

        //Holds whether the player has started the game
        bool gameStarted;
        float timeElapsedSinceTap = 0;
        float timeToStartGame = 0.9f;

        GameplayScreen gameplayScreen;

        #endregion

        #region Initialization

        public MainMenuScreen(GameplayScreen gameplayScreen)
        {
            IsPopup = true;
            gameStarted = false;

            this.gameplayScreen = gameplayScreen;
        }

        public override void LoadContent()
        {
            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            //Main menu texture to be rendered on the screen
            backgroundTexture = Load<Texture2D>("Textures/MainMenuBackground2");
            stringTexture = Load<Texture2D>("Textures/TapToAmbush");
            stringPosition = new Vector2(viewport.Width / 2, 480);
            backgroundPosition = Vector2.Zero;

            buttonHighscoresTexture = Load<Texture2D>("Textures/Buttons/ButtonHighscores");
            buttonOptionsTexture = Load<Texture2D>("Textures/Buttons/ButtonOptions");

            float yButtonPosition = 700;

            buttonHighscores = new Button();
            buttonHighscores.Initialize(buttonHighscoresTexture, new Vector2((viewport.Width / 2) - 50, yButtonPosition), 0.9f);
            buttonHighscores.Selected += new EventHandler(buttonHighscores_Selected);

            buttonOptions = new Button();
            buttonOptions.Initialize(buttonOptionsTexture, new Vector2((viewport.Width / 2) + 50, yButtonPosition), 0.9f);
            buttonOptions.Selected += new EventHandler(buttonOptions_Selected);

            MenuButtons.Add(buttonHighscores);
            MenuButtons.Add(buttonOptions);

            base.LoadContent();
        }

        void buttonOptions_Selected(object sender, EventArgs e)
        {

        }

        void buttonHighscores_Selected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new HighScoreScreen(), null);
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateTextFade(gameTime);

            if (gameStarted)
            {
                timeElapsedSinceTap += (float)gameTime.ElapsedGameTime.TotalSeconds;
                UpdateBackgroundPosition(gameTime);

                if (timeElapsedSinceTap > timeToStartGame)
                {
                    ScreenManager.RemoveScreen(this);
                    gameplayScreen.GameStarted = true;
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void UpdateBackgroundPosition(GameTime gameTime)
        {
            backgroundPosition.Y -= (float)(backgroundScrollSpeed * Math.Pow(timeElapsedSinceTap, 2));
        }

        private void UpdateTextFade(GameTime gameTime)
        {
            //Decrement the delay by the number of seconds that have elapsed since
            //the last time that the Update method was called
            mFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            //If the Fade delays has dropped below zero, then it is time to 
            //fade in/fade out the image a little bit more.
            if (mFadeDelay <= 0)
            {
                //Reset the Fade delay
                mFadeDelay = .035;

                //Increment/Decrement the fade value for the image
                mAlphaFactor += mFadeIncrement;

                //If the AlphaValue is equal or above the max Alpha value or
                //has dropped below or equal to the min Alpha value, then 
                //reverse the fade
                if (mAlphaFactor >= 1 || mAlphaFactor <= 0)
                {
                    mFadeIncrement *= -1;
                }

                if (mAlphaFactor >= 1)
                {
                    mFadeDelay = 0.5;
                }
            }
        }

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    if (!buttonSelected)
                    {
                        gameStarted = true;
                    }
                }
            }
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        #endregion

        #region Draw

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //Create a fade effect when the screen is transitioning on
            ScreenManager.FadeBackBufferToBlack(TransitionPosition);

            spriteBatch.Begin();

            //Draw main menu background
            spriteBatch.Draw(backgroundTexture, backgroundPosition, null, Color.White, 0f,
               Vector2.Zero, 1f, SpriteEffects.None, 1f);

            if (!gameStarted)
            {
                spriteBatch.Draw(stringTexture, stringPosition, null, new Color(255, 255, 255, (byte)MathHelper.Clamp(255, 0, 255)) * mAlphaFactor, 0f,
                   new Vector2(stringTexture.Width / 2, stringTexture.Height / 2), 1f, SpriteEffects.None, 1f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
