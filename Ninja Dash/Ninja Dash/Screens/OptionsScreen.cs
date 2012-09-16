using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO.IsolatedStorage;
using System.IO;
using GameStateManagement;

namespace Ninja_Dash
{
    class OptionsScreen : MenuScreen
    {
        #region Fields

        //Menu entry for sound on/off
        MenuEntry soundOptionMenuEntry;

        //Menu entry to view credits
        MenuEntry creditsOptionMenuEntry;

        //Menu entry to go back to the previous screen
        MenuEntry backOptionMenuEntry;

        //Text file to store the sound state (on/off)
        static readonly string SoundFilename = "sound.txt";

        //Is the sound on?
        static bool soundOn;

        #endregion

        #region Intialize

        public OptionsScreen()
        {
            //Read the sound state from the text file and determine the value of soundOn
            OptionsScreen.soundOn = IsSoundOn();

            soundOptionMenuEntry = new MenuEntry("");
            creditsOptionMenuEntry = new MenuEntry("CREDITS");
            backOptionMenuEntry = new MenuEntry("BACK");

            //Add methods to menu entry event handlers
            soundOptionMenuEntry.Selected += SoundOptionSelected;
            creditsOptionMenuEntry.Selected += CreditsOptionSelected;
            backOptionMenuEntry.Selected += BackOptionSelected;

            //Write the sound state to the text file
            OptionsScreen.SaveSound();
        }

        #endregion

        #region Other Methods

        //Method that is added to the sound menu entry event handler
        public void SoundOptionSelected(object sender, EventArgs e)
        {
            //If the user clicks the sound button when the sound is on, the sound will turn off
            if (IsSoundOn())
            {
                soundOn = false;
                AudioManager.StopMusic();
                OptionsScreen.SaveSound();
            }

            //If the user clicks the sound button when the sound is off, the sound will turn on
            else
            {
                soundOn = true;
                //AudioManager.PlayMusic("Funky Game");
                OptionsScreen.SaveSound();
            }
        }

        //Method that is added to credits menu entry event handler
        public void CreditsOptionSelected(object sender, EventArgs e)
        {
            //Switch to credit screen
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new CreditsScreen(), null);
        }

        //Method that is added to back menu entry event handler
        public void BackOptionSelected(object sender, EventArgs e)
        {
            //Switch to the previous screen
            //ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        //Overrides the back button functionality on the phone
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            //Return to previous screen
            //ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        #endregion

        #region Update

        //Updates the text shown on the "SOUND: " menu entry depending on the sound state
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (soundOn)
            {
                soundOptionMenuEntry.Text = "SOUND: ON";
            }

            else
            {
                soundOptionMenuEntry.Text = "SOUND: OFF";
            }

        }

        #endregion

        #region Sound Loading/Saving

        public static void SaveSound()
        {
            // Get the place to store the data
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create the file to save the data
                using (IsolatedStorageFileStream isfs = isf.CreateFile(OptionsScreen.SoundFilename))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        isfs.Flush();
                        writer.WriteLine(soundOn.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Loads the sound state from a text file.  
        /// </summary>
        public static bool IsSoundOn()
        {
            //Set the initialize sound state to be on
            string textSoundOn = "True";

            // Get the place the data stored
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Try to open the file
                if (isf.FileExists(OptionsScreen.SoundFilename))
                {
                    using (IsolatedStorageFileStream isfs =
                        isf.OpenFile(OptionsScreen.SoundFilename, FileMode.Open))
                    {
                        // Get the stream to read the data
                        using (StreamReader reader = new StreamReader(isfs))
                        {
                            textSoundOn = reader.ReadLine();
                        }
                    }
                }
            }

            if (textSoundOn == "True")
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        #endregion
    }
}
