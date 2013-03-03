#region File Description
//-----------------------------------------------------------------------------
// HighscoreScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameStateManagement;

#endregion

namespace Ninja_Dash
{
    class HighScoreScreen : MenuScreen
    {
        #region Fields

        Viewport viewport;

        static readonly string HighScoreFilename = "highscores.txt";

        const int highscorePlaces = 5;
        public static List<KeyValuePair<string, int>> highScore = new List<KeyValuePair<string, int>>(highscorePlaces)
        {
            new KeyValuePair<string,int>
                ("----",0),
            new KeyValuePair<string,int>
                ("----",0),
            new KeyValuePair<string,int>
                ("----",0),
            new KeyValuePair<string,int>
                ("----",0),
            new KeyValuePair<string,int>
                ("----",0),
        };

        //Textures
        Texture2D highscoreTexture;
        Texture2D buttonBackTexture;

        //Fonts
        SpriteFont highScoreFont;

        //Buttons
        Button buttonBack;

        Dictionary<int, string> numberPlaceMapping;

        //Main menu screen the high score screen is associated with
        MainMenuScreen mainMenuScreen;

        #endregion

        #region Button Event Handlers

        //Back button event handler
        public void buttonBack_Selected(object sender, EventArgs e)
        {
            GoToMainMenuScreen();
        }

        //Event handler for hardware back button
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            GoToMainMenuScreen();
        }

        //Goes back to the main menu screen
        public void GoToMainMenuScreen()
        {
            ScreenManager.RemoveScreen(this);
            mainMenuScreen.showingHighScoreScreen = false;
        }

        #endregion

        #region Initialzations


        /// <summary>
        /// Creates a new highscore screen instance.
        /// </summary>
        public HighScoreScreen(MainMenuScreen mainMenuScreen)
        {
            EnabledGestures = GestureType.Tap;

            IsPopup = true;
            this.mainMenuScreen = mainMenuScreen;
            numberPlaceMapping = new Dictionary<int, string>();
            initializeMapping();
        }

        /// <summary>
        /// Load screen resources
        /// </summary>
        public override void LoadContent()
        {
            //Initialize viewport
            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            highscoreTexture = Load<Texture2D>("Textures/HighscoreText");
            buttonBackTexture = Load<Texture2D>("Textures/Buttons/ButtonBack");
            highScoreFont = Load<SpriteFont>("Fonts/MenuFont");

            LoadHighscores();

            //Initialize back button
            buttonBack = new Button();
            buttonBack.Initialize(buttonBackTexture, new Vector2((viewport.Width / 2), 700), 1.0f);
            buttonBack.Selected += new EventHandler(buttonBack_Selected);

            //Add Menu Buttons
            MenuButtons.Add(buttonBack);

            base.LoadContent();
        }


        #endregion

        #region Render


        /// <summary>
        /// Renders the screen.
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        public override void Draw(GameTime gameTime)
        {
            Vector2 highscoreTexturePosition = new Vector2(240, 200);

            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(highscoreTexture, highscoreTexturePosition, null, Color.White, 0.0f,
                   new Vector2(highscoreTexture.Width / 2, highscoreTexture.Height), 1.0f, SpriteEffects.None, 0.0f);

            // Draw the highscores table
            for (int i = 0; i < highScore.Count; i++)
            {
                if (!string.IsNullOrEmpty(highScore[i].Key))
                {
                    Vector2 numberPosition = new Vector2(70, i * 72 + 250);
                    // Draw place number
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, GetPlaceString(i),
                        numberPosition + new Vector2(4, 4), Color.Black);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, GetPlaceString(i),
                        numberPosition, Color.White);

                    //Note: Uncomment this to display names for highscores
                    Vector2 namePosition = new Vector2(190, i * 72 + 250);
                    // Draw Name
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Key,
                        namePosition + new Vector2(4, 4), Color.Black);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Key,
                        namePosition, Color.White);

                    Vector2 scorePosition = new Vector2(350, i * 72 + 250);
                    // Draw score
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Value.ToString(),
                        scorePosition + new Vector2(4, 4), Color.Black);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Value.ToString(),
                        scorePosition, Color.White);

                }
            }

            ScreenManager.SpriteBatch.End();

            //Create a fade effect when the screen is transitioning on
            ScreenManager.FadeBackBufferToBlack(TransitionPosition); 

            base.Draw(gameTime);
        }


        #endregion

        #region Highscore loading/saving logic


        /// <summary>
        /// Check if a score belongs on the high score table.
        /// </summary>
        /// <returns></returns>
        public static bool IsInHighscores(int score)
        {
            // If the score is better than the worst score in the table
            return score > highScore[highscorePlaces - 1].Value;
        }

        /// <summary>
        /// Put high score on highscores table.
        /// </summary>
        /// <param name="name">Player's name.</param>
        /// <param name="score">The player's score.</param>
        public static void PutHighScore(string playerName, int score)
        {
            if (IsInHighscores(score))
            {
                highScore[highscorePlaces - 1] = new KeyValuePair<string, int>(playerName, score);
                OrderGameScore();
                SaveHighscore();
            }
        }

        /// <summary>
        /// Order the high scores table.
        /// </summary>
        private static void OrderGameScore()
        {
            highScore.Sort(CompareScores);
        }

        /// <summary>
        /// Comparison method used to compare two highscore entries.
        /// </summary>
        /// <param name="score1">First highscore entry.</param>
        /// <param name="score2">Second highscore entry.</param>
        /// <returns>1 if the first highscore is smaller than the second, 0 if both
        /// are equal and -1 otherwise.</returns>
        private static int CompareScores(KeyValuePair<string, int> score1,
            KeyValuePair<string, int> score2)
        {
            if (score1.Value < score2.Value)
            {
                return 1;
            }

            if (score1.Value == score2.Value)
            {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Saves the current highscore to a text file. 
        /// </summary>
        public static void SaveHighscore()
        {
            // Get the place to store the data
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create the file to save the data
                using (IsolatedStorageFileStream isfs = isf.CreateFile(HighScoreScreen.HighScoreFilename))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        for (int i = 0; i < highScore.Count; i++)
                        {
                            // Write the scores
                            writer.WriteLine(highScore[i].Key);
                            writer.WriteLine(highScore[i].Value.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the high score from a text file.  
        /// </summary>
        public static void LoadHighscores()
        {
            // Get the place the data stored
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Try to open the file
                if (isf.FileExists(HighScoreScreen.HighScoreFilename))
                {
                    using (IsolatedStorageFileStream isfs =
                        isf.OpenFile(HighScoreScreen.HighScoreFilename, FileMode.Open))
                    {
                        // Get the stream to read the data
                        using (StreamReader reader = new StreamReader(isfs))
                        {
                            // Read the highscores
                            int i = 0;
                            while (!reader.EndOfStream)
                            {
                                string name = reader.ReadLine();
                                string score = reader.ReadLine();
                                highScore[i++] = new KeyValuePair<string, int>(name, int.Parse(score));
                            }
                        }
                    }
                }
            }

            OrderGameScore();
        }

        private string GetPlaceString(int number)
        {
            return numberPlaceMapping[number];
        }

        private void initializeMapping()
        {
            numberPlaceMapping.Add(0, "1ST");
            numberPlaceMapping.Add(1, "2ND");
            numberPlaceMapping.Add(2, "3RD");
            numberPlaceMapping.Add(3, "4TH");
            numberPlaceMapping.Add(4, "5TH");
        }


        #endregion
    }
}
