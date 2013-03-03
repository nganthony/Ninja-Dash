#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Ninja_Dash
{
    class Player
    {
        #region Fields

        //States of the ninja player
        public enum State
        {
            RunLeft,
            RunRight,
            Jump,
            Idle,
            NinjaStarPowerUp,
            FlyingObjectPowerUp,
            HorizontalEnemyPowerUp,
            Hit
        }

        Game curGame;
        Viewport viewport;
        GameplayScreen gameplayScreen;

        //Player animations
        Animation runAnimation;
        Animation jumpAnimation;
        Animation fallAnimation;
        Animation idleFallAnimation;
        Animation rocketshipAnimation;
        Animation centralAnimation;
        Animation shieldAnimation;

        //A store containing all the animations used in the game
        AnimationStore animationStore;

        //Used for flipping the player sprite
        SpriteEffects flip;

        //Offset vertical position of player
        public float marginVertical = 600;

        //Additional vertical padding
        public float verticalOffset = 100;  

        //Collision box surrounding the player for collision detection
        Rectangle collisionRectangle;

        //State indicating whether the player has a shield on
        public bool ShieldOn;

        //Elapsed time since ninja star power up
        public TimeSpan elapsedPowerUpTime = TimeSpan.Zero;

        //The time which the ninja star power up should start to end
        TimeSpan rocketPowerUpEndTime = TimeSpan.FromSeconds(2.0);

        //State indicating if the rocketship sound has played
        public bool LiftOffSoundPlayed = false;

        //Holds the state of the player
        public State PlayerState;

        //Holds the previous state of the player
        public State PreviousState;

        //Holds the direction in which the player is heading
        public State DirectionState;

        //Margins of the screen
        public float marginLeft;
        public float marginRight;

        //Jump motion values
        Vector2 jumpVelocity = new Vector2(730, 860);
        float jumpGravity = 800;

        //Fall motion values
        Vector2 fallVelocity = new Vector2(50, 0);
        const float fallGravity = 620;

        //Horizontal enemy power up variables
        int horizontalPowerUpCount = 0;
        public Vector2 horizontalPowerUpSpeed = new Vector2(750, 450);
        int numberOfJumps = 10;

        //Holds the amount of time elapsed since start of motion
        float flightTime = 0;

        //Holds where the initial position of the player before the motion starts
        public Vector2 StartMotionPosition;

        //Camera speeds
        public uint runSpeed = 700;
        public const uint fallSpeed = 600;
        public const uint ninjaStarPowerUpSpeed = 1050;

        //Dictionary of counters of items collected
        public Dictionary<string, int> ItemsCollected;

        //Counter of gems collected, this can reset back to 0 when 50 gems have been collected
        public int GemsCollected = 0;

        //Counter of total gems collected for one game
        public int AccumulatedGems = 0;

        //Counter of shields used for one game
        public int AccumulatedShieldsUsed = 0;

        //Counter of total number of power ups used for one game
        public int AccumulatedPowerUpsUsed = 0;

        public bool FinishedTransition = false;

        TimeSpan elapsedHitTime = TimeSpan.Zero;
        //Time to initiate fall animation, based on how long the idle fall animation lasts for
        TimeSpan initiateFallAnimationTime = TimeSpan.FromSeconds(0.48);

        #endregion

        #region Properties

        //Position of player
        public Vector2 Position;

        public int Width
        {
            get { return centralAnimation.ScaledWidth; }
        }

        public int Height
        {
            get { return centralAnimation.ScaledHeight; }
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                collisionRectangle = new Rectangle((int)Position.X - Width / 2, 
                    (int)Position.Y - Height / 2, Width, Height);

                //Make collision rectangle smaller so that it is more lenient with hit detection
                collisionRectangle.Inflate(-20, -20);

                return collisionRectangle;
            }
        }

        #endregion

        #region Initialize

        public Player(Game game, AnimationStore animationStore, GameplayScreen gameplayScreen)
        {
            curGame = (NinjaDashGame)game;
            this.animationStore = animationStore;
            this.gameplayScreen = gameplayScreen;

            ItemsCollected = new Dictionary<string, int>();

            //Add items collected in-game to ItemsCollected dictionary
            ItemsCollected.Add("NinjaStars", 0);
            ItemsCollected.Add("FlyingObjects", 0);
            ItemsCollected.Add("HorizontalEnemies", 0);
        }

        public void Initialize()
        {
            viewport = curGame.GraphicsDevice.Viewport;

            //Set the screen margins for the player
            const float ViewMargin = 0.20f;
            float marginWidth = viewport.Width * ViewMargin;
            marginLeft = marginWidth;
            marginRight = viewport.Width - marginWidth;

            //Initial starting position
            Position = new Vector2(marginLeft, viewport.Height);

            //Initialize animations
            runAnimation = new Animation(animationStore["PlayerRun"]);
            jumpAnimation = new Animation (animationStore["PlayerJump"]);
            fallAnimation = new Animation(animationStore["PlayerFall"]);
            idleFallAnimation = new Animation(animationStore["PlayerIdleFall"]);
            rocketshipAnimation = new Animation(animationStore["Rocketship"]);
            shieldAnimation = new Animation(animationStore["Shield"]);

            //Set the initial central animation to run animation
            centralAnimation = runAnimation;

            //Initialize starting states
            ShieldOn = false;
            PlayerState = State.RunLeft;
            DirectionState = State.RunLeft;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            //Update info on the player based on its state
            UpdatePlayerState(gameTime);

            UpdateShield(gameTime);

            //Update player animation
            centralAnimation.Update(gameTime);

            //Clamp the position of the player to the screen margins
            Position.X = MathHelper.Clamp(Position.X, marginLeft, marginRight);
        }

        public void UpdatePlayerState(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (PlayerState)
            {
                case State.RunLeft:
                    //Switch to player run animation
                    centralAnimation = runAnimation;
                    flip = SpriteEffects.None;
                    Position.Y -= (runSpeed * elapsed);
                    break;

                case State.RunRight:
                    //Switch to player run animation
                    centralAnimation = runAnimation;
                    flip = SpriteEffects.FlipHorizontally;
                    Position.Y -= (runSpeed * elapsed);
                    break;

                case State.Jump:
                    //Initiate player jump with velocity and gravity
                    UpdatePlayerJump(gameTime, jumpAnimation, jumpVelocity, jumpGravity);
                    break;

                case State.Idle:
                    centralAnimation = jumpAnimation;

                    //Flip the sprite animation based on where the player is located
                    if (DirectionState == State.RunRight)
                    {
                        flip = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        flip = SpriteEffects.None;
                    }

                    //Check the camera position relative to player position
                    if (Position.Y - gameplayScreen.cameraPosition > marginVertical)
                    {
                        //Set to run right if player is on right wall
                        if (DirectionState == State.RunRight)
                        {
                            PlayerState = State.RunRight;
                        }
                        //Set to run left if player is on the left wall
                        else if (DirectionState == State.RunLeft)
                        {
                            PlayerState = State.RunLeft;
                        }

                        //Reset values
                        flightTime = 0;
                        ItemsCollected["NinjaStars"] = 0;
                        elapsedPowerUpTime = TimeSpan.Zero;
                        LiftOffSoundPlayed = false;
                        AccumulatedPowerUpsUsed++;
                    }
                    break;

                case State.NinjaStarPowerUp:
                    //Set central animation to rocket ship animation
                    centralAnimation = rocketshipAnimation;

                    //Updates the player in its ninja star power up state
                    UpdateNinjaStarPowerUp(gameTime);
                    break;

                case State.FlyingObjectPowerUp:
                    centralAnimation = runAnimation;
                    break;

                case State.HorizontalEnemyPowerUp:
                    centralAnimation = jumpAnimation;

                    //Update the player in its horizontal enemy power up state
                    UpdateHorizontalEnemyPowerUp(gameTime);
                    break;
                    
                case State.Hit:
                    //Start keeping track of how much time has elapsed since player was hit
                    elapsedHitTime += gameTime.ElapsedGameTime;

                    if (elapsedHitTime < initiateFallAnimationTime)
                    {
                        centralAnimation = idleFallAnimation;
                    }
                    else
                    {
                        centralAnimation = fallAnimation;
                    }

                    int direction = (DirectionState == State.RunLeft) ? 1 : -1;

                    UpdatePlayerTrajectoryMotion(direction, gameTime, fallVelocity, fallGravity, 
                        StartMotionPosition.X, StartMotionPosition.Y);
                    break;
            }
        }

        private void UpdateHorizontalEnemyPowerUp(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Check if the player has reached the max number of power up jumps
            if (horizontalPowerUpCount < numberOfJumps)
            {
                //Depending on which the direction the player originally jumped in,
                //move the player left initially if player was jumping to left.
                //If the player was jumping right, move the player to the right.
                if (DirectionState == State.RunLeft)
                {
                    Position.X -= horizontalPowerUpSpeed.X * elapsed;
                }
                else
                {
                    Position.X += horizontalPowerUpSpeed.X * elapsed;
                }

                //Update player in y-axis
                Position.Y -= horizontalPowerUpSpeed.Y * elapsed;

                //Check if player has exceeded left margin
                if (Position.X < marginLeft)
                {
                    flip = SpriteEffects.FlipHorizontally;

                    //Play sound
                    if (horizontalPowerUpCount != numberOfJumps - 1)
                    {
                        AudioManager.PlaySound("Swoosh");
                    }

                    //Switch direction
                    horizontalPowerUpSpeed.X *= -1;
                    horizontalPowerUpCount++;
                }
                //Check if player has exceeded right margin
                else if (Position.X > marginRight)
                {
                    flip = SpriteEffects.None;

                    //Play sound
                    if (horizontalPowerUpCount != numberOfJumps - 1)
                    {
                        AudioManager.PlaySound("Swoosh");
                    }

                    //Switch direction
                    horizontalPowerUpSpeed.X *= -1;
                    horizontalPowerUpCount++;
                }
            }
            else
            {
                //Reset values
                horizontalPowerUpCount = 0;
                flightTime = 0;
                ItemsCollected["HorizontalEnemies"] = 0;
                AccumulatedPowerUpsUsed++;

                //Put the player in correct state based on number of jumps and
                //the direction which the player was initially heading
                if (DirectionState == State.RunRight)
                {
                    if (numberOfJumps % 2 == 0)
                    {
                        PlayerState = State.RunLeft;
                    }
                    else
                    {
                        PlayerState = State.RunRight;
                    }
                }
                else
                {
                    if (numberOfJumps % 2 == 0)
                    {
                        PlayerState = State.RunRight;
                    }
                    else
                    {
                        PlayerState = State.RunLeft;
                    }
                }
            }
        }

        private void UpdateNinjaStarPowerUp(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Check if the player is in power up mode
            if (PlayerState == State.NinjaStarPowerUp)
            {
                //Accelerate the player upwards
                Position.Y -= ninjaStarPowerUpSpeed * elapsed;

                //Start tracking how long the power up has lasted for
                elapsedPowerUpTime += gameTime.ElapsedGameTime;

                if (elapsedPowerUpTime < rocketPowerUpEndTime)
                {
                    //Control the speed of the walls
                    gameplayScreen.leftWall.ScrollSpeed = ninjaStarPowerUpSpeed;
                    gameplayScreen.rightWall.ScrollSpeed = ninjaStarPowerUpSpeed;
                }
                else
                {
                    //Check if the player is in between the walls
                    if (Position.X < marginRight && Position.X > marginLeft)
                    {
                        //Move left if the player was originally going left
                        if (DirectionState == State.RunLeft)
                        {
                            Position.X -= 210 * elapsed;
                        }
                        //Move right if the player was originally going right
                        else if (DirectionState == State.RunRight)
                        {
                            Position.X += 210 * elapsed;
                        }

                        //Slow down the power up
                        Position.Y += 300 * elapsed;
                    }
                    //The player has reached one of the walls
                    else
                    {
                        //Slow down the wall to player run speed
                        gameplayScreen.leftWall.ScrollSpeed = runSpeed;
                        gameplayScreen.rightWall.ScrollSpeed = runSpeed;

                        //Set player to idle to allow camera to catch up to player
                        PlayerState = State.Idle;
                    }
                }
            }
        }

        private void UpdateShield(GameTime gameTime)
        {
            if (ShieldOn)
            {
                shieldAnimation.Update(gameTime);
            }

            if (GemsCollected >= 50)
            {
                ShieldOn = true;
                AudioManager.PlaySound("ShieldOn");
                GemsCollected -= 50;
                AccumulatedShieldsUsed++;
            }
        }

        private void UpdateFlyingObjectPowerUp(GameTime gameTime)
        {
            //TODO: Player will do a sinusoidal movement upwards. Direction of movement will be based on DirectionState.
        }

        private void UpdatePlayerJump(GameTime gameTime, Animation animation, Vector2 jumpVelocity, float jumpGravity)
        {
            //Switch to player jump animation
            centralAnimation = animation;

            if (PreviousState == State.RunLeft)
            {
                flip = SpriteEffects.FlipHorizontally;
                DirectionState = State.RunRight;

                //Update jump motion
                UpdatePlayerTrajectoryMotion(1, gameTime, jumpVelocity,
                    jumpGravity, StartMotionPosition.X, StartMotionPosition.Y);

                //Player has reached right wall
                if (Position.X > marginRight)
                {
                    PlayerState = State.RunRight;

                    //Clear flight time
                    flightTime = 0;
                }
            }
            else if (PreviousState == State.RunRight)
            {
                flip = SpriteEffects.None;
                DirectionState = State.RunLeft;

                //Update jump motion
                UpdatePlayerTrajectoryMotion(-1, gameTime, jumpVelocity,
                    jumpGravity, StartMotionPosition.X, StartMotionPosition.Y);

                //Player has reached the left margin
                if (Position.X < marginLeft)
                {
                    PlayerState = State.RunLeft;

                    //Clear flight time
                    flightTime = 0;
                }
            }
        }
        
        private void UpdatePlayerTrajectoryMotion(int direction, GameTime gameTime, Vector2 velocity, float gravity, float startXPosition, float startYPosition)
        {
            //Gather the total time since motion was initiated
            flightTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Use projectile formulas to perform motion
            Position.X = startXPosition + (direction) * velocity.X * flightTime;
            Position.Y = startYPosition - (velocity.Y * flightTime) + (float)((0.5) * gravity * Math.Pow(flightTime, 2));
        }

        public void IncrementItemCollected(string itemCollected)
        {
            //Increment the number of items until it reaches the max capacity
            if (ItemsCollected[itemCollected] < 3)
            {
                //Increment the counter for item collected
                ItemsCollected[itemCollected]++;
            }

            //Holds all the keys of the dictionary except for the item that was collected
            List<string> itemCollectionStrings = new List<string>();

            //Traverse through the dictionary of items collected
            foreach (KeyValuePair<string, int> entry in ItemsCollected)
            {
                if (entry.Key != itemCollected)
                {
                    //Add the keys of the items that were not collected
                    itemCollectionStrings.Add(entry.Key);
                }
            }

            //Set the counter of the rest of the items to zero
            for (int i = itemCollectionStrings.Count - 1; i >= 0; i--)
            {
                ItemsCollected[itemCollectionStrings[i]] = 0;
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw player animation based on the state of the player
            centralAnimation.Draw(spriteBatch, Position, flip);

            DrawShield(spriteBatch);
        }

        private void DrawShield(SpriteBatch spriteBatch)
        {
            if (ShieldOn)
            {
                if (PlayerState != Player.State.NinjaStarPowerUp)
                {
                    shieldAnimation.Draw(spriteBatch, Position, SpriteEffects.None);
                }
            }
        }

        #endregion
    }
}
