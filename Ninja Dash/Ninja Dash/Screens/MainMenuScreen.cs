#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace Ninja_Dash
{
    class MainMenuScreen: MenuScreen
    {

        #region Fields

        Viewport viewport;

        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        //Textures
        Texture2D backgroundTexture;
        Texture2D stringTexture;

        //Position of textures
        Vector2 stringPosition;
        Vector2 backgroundPosition;

        //Button textures
        Texture2D buttonHighscoresTexture;
        Texture2D buttonSoundOnTexture;
        Texture2D buttonSoundOffTexture;
        Texture2D buttonHelpTexture;
        Texture2D buttonRateAppTexture;

        //Buttons
        Button buttonHighscores;
        Button buttonSound;
        Button buttonHelp;
        Button buttonRateApp;

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

        public bool showingHighScoreScreen = false;

        public static bool IsSoundOn = true;

        #endregion

        #region Initialization

        public MainMenuScreen(GameplayScreen gameplayScreen)
        {
            IsPopup = true;
            gameStarted = false;

            this.gameplayScreen = gameplayScreen;

            if(!settings.Contains("IsSoundOn"))
            {
                settings.Add("IsSoundOn", IsSoundOn);
                settings.Save();
            }
        }

        public override void LoadContent()
        {
            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            //Main menu texture to be rendered on the screen
            backgroundTexture = Load<Texture2D>("Textures/MenuTitle");
            stringTexture = Load<Texture2D>("Textures/TapToAmbush");
            stringPosition = new Vector2(viewport.Width / 2, 480);
            backgroundPosition = Vector2.Zero;

            //Initialize button textures
            buttonHighscoresTexture = Load<Texture2D>("Textures/Buttons/ButtonHighscores");
            buttonSoundOnTexture = Load<Texture2D>("Textures/Buttons/ButtonSoundOn");
            buttonSoundOffTexture = Load<Texture2D>("Textures/Buttons/ButtonSoundOff");
            buttonHelpTexture = Load<Texture2D>("Textures/Buttons/ButtonHelp");
            buttonRateAppTexture = Load<Texture2D>("Textures/Buttons/ButtonRateApp");

            float yButtonPosition = 650;

            //Initialize high scores button
            buttonHighscores = new Button();
            buttonHighscores.Initialize(buttonHighscoresTexture, new Vector2((viewport.Width / 2) - 100, yButtonPosition), 0.9f);
            buttonHighscores.Selected += new EventHandler(buttonHighscores_Selected);

            //Initialize sound button
            buttonSound = new Button();
            buttonSound.Initialize(buttonSoundOnTexture, new Vector2((viewport.Width / 2), yButtonPosition), 0.9f);
            buttonSound.Selected +=new EventHandler(buttonSound_Selected);

            settings.TryGetValue<bool>("IsSoundOn", out IsSoundOn);

            if (IsSoundOn)
            {
                buttonSound.ButtonTexture = buttonSoundOnTexture;
            }
            else
            {
                buttonSound.ButtonTexture = buttonSoundOffTexture;
            }

            //Initialize help button
            buttonHelp = new Button();
            buttonHelp.Initialize(buttonHelpTexture, new Vector2((viewport.Width / 2) + 100, yButtonPosition), 0.9f);
            buttonHelp.Selected += new EventHandler(buttonHelp_Selected);

            buttonRateApp = new Button();
            buttonRateApp.Initialize(buttonRateAppTexture, new Vector2(viewport.Width / 2, yButtonPosition + 90), 1.0f);
            buttonRateApp.Selected += new EventHandler(buttonRateApp_Selected);

            //Add menu buttons to list
            MenuButtons.Add(buttonHighscores);
            MenuButtons.Add(buttonSound);
            MenuButtons.Add(buttonHelp);
            MenuButtons.Add(buttonRateApp);

            base.LoadContent();
        }

        void buttonRateApp_Selected(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        void buttonHelp_Selected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new TutorialScreen(), null);
        }

        void buttonSound_Selected(object sender, EventArgs e)
        {
            settings.TryGetValue<bool>("IsSoundOn", out IsSoundOn);

            if (IsSoundOn)
            {
                IsSoundOn = false;
                buttonSound.ButtonTexture = buttonSoundOffTexture;
                AudioManager.StopMusic();
            }
            else
            {
                IsSoundOn = true;
                buttonSound.ButtonTexture = buttonSoundOnTexture;
                AudioManager.PlayMusic("GameplayMusic");
            }

            settings.Remove("IsSoundOn");
            settings.Add("IsSoundOn", IsSoundOn);
            settings.Save();
        }

        void buttonHighscores_Selected(object sender, EventArgs e)
        {
            showingHighScoreScreen = true;
            ScreenManager.AddScreen(new HighScoreScreen(this), null);
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

            if (!showingHighScoreScreen)
            {
                //Draw main menu background
                spriteBatch.Draw(backgroundTexture, backgroundPosition, null, Color.White, 0f,
                   Vector2.Zero, 1f, SpriteEffects.None, 1f);

                if (!gameStarted)
                {
                    spriteBatch.Draw(stringTexture, stringPosition, null, new Color(255, 255, 255, (byte)MathHelper.Clamp(255, 0, 255)) * mAlphaFactor, 0f,
                       new Vector2(stringTexture.Width / 2, stringTexture.Height / 2), 1f, SpriteEffects.None, 1f);
                }
            }

            spriteBatch.End();

            //Do not show the menu buttons if high score screen is showing
            if (!showingHighScoreScreen)
            {
                base.Draw(gameTime);
            }
        }

        #endregion
    }
}
