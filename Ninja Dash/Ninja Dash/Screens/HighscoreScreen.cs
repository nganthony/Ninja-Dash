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

        SpriteFont highScoreFont;

        Dictionary<int, string> numberPlaceMapping;


        #endregion

        #region Initialzations


        /// <summary>
        /// Creates a new highscore screen instance.
        /// </summary>
        public HighScoreScreen()
        {
            EnabledGestures = GestureType.Tap;

            IsPopup = true;

            numberPlaceMapping = new Dictionary<int, string>();
            initializeMapping();
        }

        /// <summary>
        /// Load screen resources
        /// </summary>
        public override void LoadContent()
        {
            highScoreFont = Load<SpriteFont>("Fonts/MenuFont");

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
            ScreenManager.FadeBackBufferToBlack(0.8f);

            ScreenManager.SpriteBatch.Begin();

            // Draw the highscores table
            for (int i = 0; i < highScore.Count; i++)
            {
                if (!string.IsNullOrEmpty(highScore[i].Key))
                {
                    Vector2 numberPosition = new Vector2(80, i * 72 + 100);
                    // Draw place number
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, GetPlaceString(i),
                        numberPosition + new Vector2(4, 4), Color.Black);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, GetPlaceString(i),
                        numberPosition, Color.White);

                    //Note: Uncomment this to display names for highscores
                    Vector2 namePosition = new Vector2(320, i * 72 + 100);
                    // Draw Name
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Key,
                        namePosition + new Vector2(4, 4), Color.Black);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Key,
                        namePosition, Color.White);

                    Vector2 scorePosition = new Vector2(320, i * 72 + 100);
                    // Draw score
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Value.ToString(),
                        scorePosition + new Vector2(4, 4), Color.Black);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont, highScore[i].Value.ToString(),
                        scorePosition, Color.White);

                }
            }

            ScreenManager.SpriteBatch.End();

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
