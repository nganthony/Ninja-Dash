using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Ninja_Dash
{
    public class Animation
    {
        #region Fields

        public Vector2 Position;

        //Sprite sheet for animation
        Texture2D spriteSheet;

        //Location of the sprite sheet
        string spriteSheetPath;

        //Source rectangle to display the frame
        Rectangle frameRectangle;

        //Number of frames the animation has
        int numberOfFrames;

        //The frame that the animation is currently on
        int frameIndex;

        //The width of the frame
        public int FrameWidth;

        //The height of the frame
        public int FrameHeight;

        //Should we keep on looping the animation?
        bool IsRepeat;

        //Is the animation currently playing?
        public bool Active;

        //The amount of time to switch to the next frame
        int frameTime;

        //The elapsed time since the last frame was displayed
        int elapsedTime;

        public float Scale;

        #endregion

        #region Intialize

        public Animation(string spriteSheetPath, int numberOfFrames, bool IsRepeat, 
            int frameTime, int frameWidth, int frameHeight, float scale)
        {
            this.spriteSheetPath = spriteSheetPath;
            this.numberOfFrames = numberOfFrames;
            this.IsRepeat = IsRepeat;
            this.frameTime = frameTime;
            this.Scale = scale;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;

            Active = true;
            elapsedTime = 0;
            frameIndex = 0;
        }

        //Copy constructor
        public Animation(Animation animation): 
            this(animation.spriteSheetPath, animation.numberOfFrames, animation.IsRepeat, 
            animation.frameTime, animation.FrameWidth, animation.FrameHeight, animation.Scale)
        {
            spriteSheet = animation.spriteSheet;
        }


        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            //If the animation is not active, do not update
            if (Active == false)
                return;

            //The amount of time since the last frame was displayed
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime >= frameTime)
            {
                //Switch frames if the time since last frame is more than frame time
                frameIndex++;

                //Reset the time since last frame to zero since we just switched frames
                elapsedTime = 0;

                //If the current frame is equal to the total number of frames
                //decide whether we should stop the animation or continue again
                //from the first frame
                if (frameIndex == numberOfFrames)
                {
                    if (IsRepeat)
                    {
                        frameIndex = 0;
                    }

                    else
                    {
                        Active = false;
                    }
                }
            }

            //Change the frame by multiplying the frame width/ height by a factor (frame index)
            frameRectangle = new Rectangle(FrameWidth * frameIndex, 0, FrameWidth, FrameHeight);
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects flip)
        {
            //Draw each frame only if the animation is in active state
            if (Active)
            {
                spriteBatch.Draw(spriteSheet, position, frameRectangle, Color.White, 0.0f,
                    new Vector2(FrameWidth/2, FrameHeight/2), Scale, flip, 0.0f);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(spriteSheet, Position, frameRectangle, Color.White, 0.0f,
                   new Vector2(FrameWidth / 2, FrameHeight / 2), Scale, SpriteEffects.None, 0.0f);
            }
        }
        #endregion

        #region Public Methods

        public void LoadSheet(ContentManager contentManager)
        {
            if (!String.IsNullOrEmpty(spriteSheetPath))
            {
                spriteSheet = contentManager.Load<Texture2D>(spriteSheetPath);
            }
        }

        #endregion
    }
}
