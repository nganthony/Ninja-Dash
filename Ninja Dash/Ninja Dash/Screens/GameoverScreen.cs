using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ninja_Dash
{
    class GameoverScreen: MenuScreen
    {
        #region Fields

        Viewport viewport;

        //Font used to draw the text on the screen
        SpriteFont gameOverFont;

        //Textures
        Texture2D gameOverTexture;
        Texture2D youClimbedTexture;
        Texture2D highestClimbTexture;
        Texture2D gemsCollectedTexture;
        Texture2D powerUpsUsedTexture;
        Texture2D shieldsUsedTexture;

        //Button textures
        Texture2D buttonReplayTexture;
        Texture2D buttonBackTexture;

        //Position of textures
        Vector2 gameOverTexturePosition;
        Vector2 youClimbedTexturePosition;
        Vector2 highestClimbTexturePosition;
        Vector2 gemsCollectedTexturePosition;
        Vector2 powerUpsUsedTexturePosition;
        Vector2 shieldsUsedTexturePosition;

        //Buttons
        Button buttonReplay;
        Button buttonBack;

        //Final player score for the game instance
        public int FinalPlayerScore
        {
            get;
            set;
        }

        //Total gems collected for the game instance
        public int TotalGemsCollected
        {
            get;
            set;
        }

        //Total power ups used for the game instance
        public int TotalPowerUpsUsed
        {
            get;
            set;
        }

        //Total shields used for the game instance
        public int TotalShieldsUsed
        {
            get;
            set;
        }

        #endregion

        #region Initialization

        public GameoverScreen()
        {
            //Set game over screen as a popup
            IsPopup = true;
        }

        public override void LoadContent()
        {
            //Initialize viewport
            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            //Initialize game over font
            gameOverFont = Load<SpriteFont>("Fonts/GameOverFont");

            //Initialize textures
            gameOverTexture = Load<Texture2D>("Textures/GameOverText");
            youClimbedTexture = Load<Texture2D>("Textures/YouClimbedText");
            highestClimbTexture = Load<Texture2D>("Textures/HighestClimbText");
            gemsCollectedTexture = Load<Texture2D>("Textures/GemsCollectedText");
            powerUpsUsedTexture = Load<Texture2D>("Textures/PowerUpsUsedText");
            shieldsUsedTexture = Load<Texture2D>("Textures/ShieldsUsedText");

            //Initialize button textures
            buttonReplayTexture = Load<Texture2D>("Textures/Buttons/ButtonReplay");
            buttonBackTexture = Load<Texture2D>("Textures/Buttons/ButtonBack");

            //Starting x-coordinate of textures
            float xPosition = (viewport.Width / 2) + 50;

            //Initialize position of textures
            youClimbedTexturePosition = new Vector2(xPosition, 320);
            highestClimbTexturePosition = new Vector2(xPosition, 400);
            gemsCollectedTexturePosition = new Vector2(xPosition, 440);
            powerUpsUsedTexturePosition = new Vector2(xPosition, 480);
            shieldsUsedTexturePosition = new Vector2(xPosition, 520);

            //Initialize back button
            buttonBack = new Button();
            buttonBack.Initialize(buttonBackTexture, new Vector2((viewport.Width / 2) - 60, 620), 1.0f);
            buttonBack.Selected += new EventHandler(buttonBack_Selected);

            //Initialize replay button
            buttonReplay = new Button();
            buttonReplay.Initialize(buttonReplayTexture, new Vector2((viewport.Width / 2) + 60, 620), 1.0f);
            buttonReplay.Selected += new EventHandler(buttonReplay_Selected);

            //Add buttons to the menu button list
            MenuButtons.Add(buttonReplay);
            MenuButtons.Add(buttonBack);

            base.LoadContent();
        }

        #endregion

        #region Button Event Handlers

        //Replay button event handler
        public void buttonReplay_Selected(object sender, EventArgs e)
        {
            //Create a new gameplay screen
            GameplayScreen gameplayScreen = new GameplayScreen();
            gameplayScreen.GameStarted = true;

            //Load the gameplay screen by exiting all existing screens and displaying gameplay screen
            LoadingScreen.Load(ScreenManager, false, null, gameplayScreen);
        }

        //Back button event handler
        public void buttonBack_Selected(object sender, EventArgs e)
        {
            //Go to main menu screen
            GoToMainMenu();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Handle Input

        //Event handler for hardware back button
        protected override void  OnCancel(PlayerIndex playerIndex)
        {
            GoToMainMenu();
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //Spacing between final score texture and final score text
            Vector2 scoreTextSpacing = new Vector2(10, 0);

            //Spacing between other textures and text
            Vector2 textSpacing = new Vector2(10, -3);

            //Shadow offset that is rendered behind the main text to create a shadow effect
            Vector2 shadowOffset = new Vector2(3, 3);

            //Use a power curve for transitioning the game over screen
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            gameOverTexturePosition = new Vector2(viewport.Width / 2, 220);
            gameOverTexturePosition.Y -= transitionOffset * 250;

            spriteBatch.Begin();

            //Game over text
            spriteBatch.Draw(gameOverTexture, gameOverTexturePosition, null, Color.White, 0.0f,
                   new Vector2(gameOverTexture.Width / 2, gameOverTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            //You climbed text
            spriteBatch.Draw(youClimbedTexture, youClimbedTexturePosition, null, Color.White, 0.0f,
                   new Vector2(youClimbedTexture.Width, youClimbedTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameOverFont, FinalPlayerScore.ToString() + " m", youClimbedTexturePosition + scoreTextSpacing + shadowOffset, Color.Black,
                0.0f, new Vector2(0, youClimbedTexture.Height), 1.0f, SpriteEffects.None, 0);

            spriteBatch.DrawString(gameOverFont, FinalPlayerScore.ToString() + " m", youClimbedTexturePosition + scoreTextSpacing, new Color(138, 206, 204),
                0.0f, new Vector2(0, youClimbedTexture.Height), 1.0f, SpriteEffects.None, 0);

            //Highest climb text
            spriteBatch.Draw(highestClimbTexture, highestClimbTexturePosition, null, Color.White, 0.0f,
                   new Vector2(highestClimbTexture.Width, highestClimbTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameOverFont, HighScoreScreen.highScore[0].Value.ToString() + " m", highestClimbTexturePosition + textSpacing + shadowOffset, Color.Black,
                0.0f, new Vector2(0, highestClimbTexture.Height), 0.8f, SpriteEffects.None, 0);

            spriteBatch.DrawString(gameOverFont, HighScoreScreen.highScore[0].Value.ToString() + " m", highestClimbTexturePosition + textSpacing, Color.White,
                0.0f, new Vector2(0, highestClimbTexture.Height), 0.8f, SpriteEffects.None, 0);

            //Gems collected text
            spriteBatch.Draw(gemsCollectedTexture, gemsCollectedTexturePosition, null, Color.White, 0.0f,
                   new Vector2(gemsCollectedTexture.Width, gemsCollectedTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameOverFont, TotalGemsCollected.ToString(), gemsCollectedTexturePosition + textSpacing + shadowOffset, Color.Black,
                0.0f, new Vector2(0, gemsCollectedTexture.Height), 0.8f, SpriteEffects.None, 0);

            spriteBatch.DrawString(gameOverFont, TotalGemsCollected.ToString(), gemsCollectedTexturePosition + textSpacing, Color.White,
                0.0f, new Vector2(0, gemsCollectedTexture.Height), 0.8f, SpriteEffects.None, 0);

            //Power ups used text
            spriteBatch.Draw(powerUpsUsedTexture, powerUpsUsedTexturePosition, null, Color.White, 0.0f,
                   new Vector2(powerUpsUsedTexture.Width, powerUpsUsedTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameOverFont, TotalPowerUpsUsed.ToString(), powerUpsUsedTexturePosition + textSpacing + shadowOffset, Color.Black,
                0.0f, new Vector2(0, powerUpsUsedTexture.Height), 0.8f, SpriteEffects.None, 0);

            spriteBatch.DrawString(gameOverFont, TotalPowerUpsUsed.ToString(), powerUpsUsedTexturePosition + textSpacing, Color.White,
                0.0f, new Vector2(0, powerUpsUsedTexture.Height), 0.8f, SpriteEffects.None, 0);

            //Shields used text
            spriteBatch.Draw(shieldsUsedTexture, shieldsUsedTexturePosition, null, Color.White, 0.0f,
                   new Vector2(shieldsUsedTexture.Width, shieldsUsedTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameOverFont, TotalShieldsUsed.ToString(), shieldsUsedTexturePosition + textSpacing + shadowOffset, Color.Black,
                0.0f, new Vector2(0, shieldsUsedTexture.Height), 0.8f, SpriteEffects.None, 0);

            spriteBatch.DrawString(gameOverFont, TotalShieldsUsed.ToString(), shieldsUsedTexturePosition + textSpacing, Color.White,
                0.0f, new Vector2(0, shieldsUsedTexture.Height), 0.8f, SpriteEffects.None, 0);

            spriteBatch.End();

            //If the screen is transitioning off, fade the screen to black
            if (ScreenState == ScreenState.TransitionOff)
            {
                ScreenManager.FadeBackBufferToBlack(TransitionPosition);
            }

            base.Draw(gameTime);
        }

        #endregion

        #region Misc. Methods

        public void GoToMainMenu()
        {
            GameplayScreen gameplayScreen = new GameplayScreen();

            LoadingScreen.Load(ScreenManager, false, null, gameplayScreen,
                                                            new MainMenuScreen(gameplayScreen));
        }

        #endregion
    }
}
