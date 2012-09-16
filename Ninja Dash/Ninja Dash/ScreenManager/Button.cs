#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
#endregion

namespace GameStateManagement
{
    public class Button
    {
        #region Fields

        //Texture of button
        Texture2D buttonTexture;

        //Position of button
        Vector2 buttonPosition;

        //Color of button
        Color color = Color.White;

        float scale;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        //The event handler to be invoked when the button has been selected
        public event EventHandler Selected;

        //Invokes the event handler
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
            {
                Selected(this, null);
            }
        }

        public int ButtonWidth
        {
            get { return buttonTexture.Width; }
        }

        public int ButtonHeight
        {
            get { return buttonTexture.Height; }
        }

        public Vector2 Position
        {
            get { return buttonPosition; }
            set { buttonPosition = value; }
        }

        //Rectangle that bounds the button (used for touch detection)
        public Rectangle ButtonRect
        {
            get
            {
                Rectangle buttonRect;

                buttonRect = new Rectangle((int)buttonPosition.X - ButtonWidth / 2, 
                    (int) buttonPosition.Y - ButtonHeight / 2,
                    buttonTexture.Width, buttonTexture.Height);

                //Give the rectangle a bit more padding
                buttonRect.Inflate(30, 30);

                return buttonRect;
            }
        }

        #endregion

        #region Initialize

        //Initializes the button texture and button position
        public void Initialize(Texture2D texture, Vector2 position, float scale)
        {
            buttonTexture = texture;
            buttonPosition = position;
            this.scale = scale;
        }

        #endregion

        #region Draw
        
        //Renders the button on to the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, buttonPosition, null, color, 0.0f,
                new Vector2(ButtonWidth / 2, ButtonHeight / 2), scale, SpriteEffects.None, 1.0f);
        }

        #endregion
    }
}
