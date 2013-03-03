#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Globalization;
using Microsoft.Xna.Framework.GamerServices;

namespace Ninja_Dash
{
    //Camera States
    public enum ScrollDirection {Up, Down}

    public enum SpawnGemState { VerticalLeft, VerticalRight, Horizontal }

    class GameplayScreen: MenuScreen
    {
        #region Fields

        Viewport viewport;

        //The main ninja player
        Player player;

        //Textures
        Texture2D wallTexture;
        Texture2D obstacleTexture;
        Texture2D castleLayerTexture1;
        Texture2D staticLayerTexture;
        Texture2D mistLayerTexture1;
        Texture2D mistLayerTexture2;
        Texture2D mountainLayerTexture1;
        Texture2D mountainLayerTexture2;
        Texture2D horizontalEnemyPlatformTexture;
        Texture2D pauseTextTexture;
        Texture2D pauseButtonTexture;

        //Store containing all the animations used in the game
        AnimationStore animationStore;

        //Background layers
        Layer staticLayer;
        Layer castleLayer;
        Layer mistLayer1;
        Layer mistLayer2;
        Layer mountainLayer1;
        Layer mountainLayer2;

        //Wall
        public Wall leftWall;
        public Wall rightWall;

        //Hud
        List<Animation> hudHorizontalEnemyAnimations;
        List<Animation> hudFlyingObjectAnimations;
        List<Animation> hudNinjaStarsAnimations;
        Animation hudGemAnimation;
        SpriteFont hudGemFont;

        //Obstacles
        List<Obstacle> obstacles;
        List<Enemy> enemies;
        List<EnemyWithObject> enemiesWithObject;
        List<FlyingEnemy> flyingEnemies;
        List<Gem> gems;
        List<HorizontalEnemy> horizontalEnemies;

        List<Animation> collisionAnimations;

        //Number of obstacles to spawn at a time
        int numberOfObstacles;

        //The seperation distance between subsequent obstacles 
        int obstacleTimeDifference;

        //Player score
        int score;
        SpriteFont scoreFont;

        //Random number generator
        Random random;

        //Time interval which items spawn
        float spawnTimeInterval;

        //Time since previous item spawn
        float previousSpawnTime;

        public float cameraPosition;

        public bool GameStarted;
        bool gameOver;

        //The elapsed time since the game was over
        TimeSpan gameOverTime;

        //The time in which the game over screen should appear
        TimeSpan initiateGameOver;

        //Number of objects the enemy should throw
        EnemyWithObject.NumberOfObjects numOfObjects;

        //Time it takes for each update
        float elapsedTime = 0;
        double elapsedGameplayTime = 0;

        bool FinishedWallTransition = false;

        Button pauseButton;
        bool gamePaused = false;

        #endregion

        #region Initialize

        public GameplayScreen()
        {
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            //Initialize interval spawn time to 2 seconds
            spawnTimeInterval = 2000;
            previousSpawnTime = 0;

            //Initialize collections
            obstacles = new List<Obstacle>();
            enemies = new List<Enemy>();
            enemiesWithObject = new List<EnemyWithObject>();
            flyingEnemies = new List<FlyingEnemy>();
            gems = new List<Gem>();
            horizontalEnemies = new List<HorizontalEnemy>();
            collisionAnimations = new List<Animation>();

            // Initialize random number generator
            random = new Random();

            //Set the initial player score to zero
            score = 0;

            GameStarted = false;
            gameOver = false;

            gameOverTime = TimeSpan.Zero;

            //Three seconds for the game over screen to be displayed after game over
            initiateGameOver = TimeSpan.FromSeconds(3.0);

            //Set the number of obstacles to be spawned at a time to be one
            numberOfObstacles = 1;

            //Set the distance between subsequent obstacles to 200 pixels
            obstacleTimeDifference = 0;

            numOfObjects = EnemyWithObject.NumberOfObjects.One;
        }

        public override void LoadContent()
        {
            viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            //Animation store initialization
            animationStore = new AnimationStore();
            animationStore.Initialize(ScreenManager.Game.Content);

            //Ninja player initialization
            player = new Player(ScreenManager.Game, animationStore, this);
            player.Initialize();

            //Set the initial camera position to height of screen
            cameraPosition = 0;

            LoadAssets();

            pauseButton = new Button();
            pauseButton.Initialize(pauseButtonTexture, new Vector2(30, 30), 1.3f);
            pauseButton.Selected += new EventHandler(pauseButton_Selected);
            MenuButtons.Add(pauseButton);

            base.LoadContent();
        }

        void pauseButton_Selected(object sender, EventArgs e)
        {
            if (gamePaused)
            {
                gamePaused = false;
            }
            else
            {
                gamePaused = true;
            }
        }

        public void LoadAssets()
        {
            //Font initialization
            scoreFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/ScoreFont");

            //Load textures to be used in GameplayScreen
            LoadTextures();

            //Initialize layers
            CreateLayerComponent();

            //Initialize wall
            CreateWallComponent();

            //Initialize hud
            CreateHudComponent();
        }

        private void CreateLayerComponent()
        {
            //Initialize castle layer
            castleLayer = new Layer(ScreenManager.Game, ScreenManager.SpriteBatch,
                castleLayerTexture1, Vector2.Zero, 12f, Layer.ScrollState.Vertical);

            //Initialize static layer
            staticLayer = new Layer(ScreenManager.Game, ScreenManager.SpriteBatch,
                staticLayerTexture, Vector2.Zero, 0.0f, Layer.ScrollState.Vertical);

            //Initialize mist layer 1
            mistLayer1 = new Layer(ScreenManager.Game, ScreenManager.SpriteBatch,
                mistLayerTexture1, Vector2.Zero, 50f, Layer.ScrollState.Horizontal);

            //Initialize mist layer 2
            mistLayer2 = new Layer(ScreenManager.Game, ScreenManager.SpriteBatch,
                mistLayerTexture2, Vector2.Zero, -60f, Layer.ScrollState.Horizontal);

            //Initialize mountain layer
            mountainLayer1 = new Layer(ScreenManager.Game, ScreenManager.SpriteBatch,
                mountainLayerTexture1, Vector2.Zero, 9f, Layer.ScrollState.Vertical);

            mountainLayer2 = new Layer(ScreenManager.Game, ScreenManager.SpriteBatch,
                mountainLayerTexture2, Vector2.Zero, 7f, Layer.ScrollState.Vertical);
        }

        private void CreateWallComponent()
        {
            //Initialize left wall
            leftWall = new Wall(ScreenManager.Game, ScreenManager.SpriteBatch, wallTexture,
                -wallTexture.Width, player.runSpeed, SpriteEffects.None);
            leftWall.Initialize();

            //Initialize right wall
            rightWall = new Wall(ScreenManager.Game, ScreenManager.SpriteBatch, wallTexture,
                viewport.Width, player.runSpeed, SpriteEffects.FlipHorizontally);
            rightWall.Initialize();
        }

        private void CreateHudComponent()
        {
            hudGemAnimation = new Animation(animationStore["Gem"]);
            hudGemFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GemFont");

            //Create lists of hud animations
            hudHorizontalEnemyAnimations = new List<Animation>();
            hudFlyingObjectAnimations = new List<Animation>();
            hudNinjaStarsAnimations = new List<Animation>();

            //Add 3 hud animations for each list
            for (int i = 0; i < 3; i++)
            {
                hudHorizontalEnemyAnimations.Add(new Animation(animationStore["HudHorizontalEnemy"]));
                hudFlyingObjectAnimations.Add(new Animation(animationStore["HudKunai"]));
                hudNinjaStarsAnimations.Add(new Animation(animationStore["HudNinjaStar"]));
            }
        }

        private void LoadTextures()
        {
            //Texture initialization
            obstacleTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/Branch");
            wallTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/BrickWall");
            castleLayerTexture1 = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/BackgroundCastleLayer1");
            staticLayerTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/BackgroundStatic1");
            mistLayerTexture1 = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/BackgroundMist1");
            mistLayerTexture2 = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/BackgroundMist4");
            mountainLayerTexture1 = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/BackgroundMountainLayer1");
            mountainLayerTexture2 = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/BackgroundMountainLayer2");
            horizontalEnemyPlatformTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/HorizontalEnemyPlatform");
            pauseButtonTexture = Load<Texture2D>("Textures/pause");
            pauseTextTexture = Load<Texture2D>("Textures/PauseText");
        }

        #endregion

        #region Game Loop Update

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateMistBackgrounds(gameTime);

            if (GameStarted)
            {
                if (!gamePaused)
                {
                    elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (FinishedWallTransition)
                    {
                        //Update ninja player
                        player.Update(gameTime);
                    }


                    UpdatePlayerHud(gameTime);
                    UpdateObstacles(gameTime);
                    UpdateEnemies(gameTime);
                    UpdateEnemiesWithObject(gameTime);
                    UpdateFlyingEnemies(gameTime);
                    UpdateGems(gameTime);
                    UpdateHorizontalEnemies(gameTime);
                    UpdateCollisionAnimations(gameTime);
                    UpdateCollisions();

                    if (player.PlayerState != Player.State.Hit)
                    {
                        UpdateCamera(gameTime);
                        UpdateWalls(gameTime);

                        if (FinishedWallTransition)
                        {
                            elapsedGameplayTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                            UpdateBackground(gameTime);
                        }

                        if (player.PlayerState != Player.State.NinjaStarPowerUp &&
                            player.PlayerState != Player.State.Idle &&
                            player.PlayerState != Player.State.HorizontalEnemyPowerUp)
                        {
                            if (FinishedWallTransition)
                            {
                                UpdateSpawnItems(gameTime);
                            }
                        }

                        UpdateSpawnTimeInterval();
                    }
                    else
                    {
                        if (!gameOver)
                        {
                            InitiateGameOverScreen(gameTime);
                        }
                    }
                }

                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
        }

        #endregion

        #region Update Misc. Methods

        private void UpdateCamera(GameTime gameTime)
        {
            if (player.PlayerState == Player.State.RunLeft ||
                player.PlayerState == Player.State.RunRight ||
                player.PlayerState == Player.State.Jump)
            {
                if (player.Position.Y <= player.marginVertical)
                {
                    player.FinishedTransition = true;
                    ScrollCamera(player.runSpeed, ScrollDirection.Up);
                }
            }
            else if (player.PlayerState == Player.State.Idle)
            {
                ScrollCamera(660, ScrollDirection.Up);
            }
            else if (player.PlayerState == Player.State.Hit)
            {
                ScrollCamera(Player.fallSpeed, ScrollDirection.Down);
            }
            else if (player.PlayerState == Player.State.NinjaStarPowerUp)
            {
                //Start increasing the speed of the camera after some time has passed
                if (player.elapsedPowerUpTime > TimeSpan.FromSeconds(0.25))
                {
                    ScrollCamera(660, ScrollDirection.Up);
                }

                if (player.elapsedPowerUpTime > TimeSpan.FromSeconds(1.3))
                {
                    if (!player.LiftOffSoundPlayed)
                    {
                        AudioManager.PlaySound("LiftOff");
                        player.LiftOffSoundPlayed = true;
                    }
                }
            }
            else if (player.PlayerState == Player.State.HorizontalEnemyPowerUp)
            {
                ScrollCamera((uint)player.horizontalPowerUpSpeed.Y, ScrollDirection.Up);
            }
        }

        private void UpdatePlayerHud(GameTime gameTime)
        {
            hudGemAnimation.Update(gameTime);

            for (int i = 0; i < 3; i++)
            {
                hudHorizontalEnemyAnimations[i].Update(gameTime);
                hudNinjaStarsAnimations[i].Update(gameTime);
                hudFlyingObjectAnimations[i].Update(gameTime);
            }
        }

        #endregion

        #region Update Spawn Methods

        //Updates the interval at which the items spawn based on player score
        private void UpdateSpawnTimeInterval()
        {
            if (score > 400 && score < 800)
            {
                spawnTimeInterval = 1500;
            }
            else if (score >= 800 && score <= 1500)
            {
                spawnTimeInterval = 1200;
                numberOfObstacles = 2;
                obstacleTimeDifference = 250;
            }
            else if (score > 1500 && score <= 2500)
            {
                spawnTimeInterval = 1000;
                numOfObjects = EnemyWithObject.NumberOfObjects.Two;
            }
            else if (score > 2500 && score <= 3500)
            {
                spawnTimeInterval = 800;
                numberOfObstacles = 3;
            }
            else if (score > 3500 && score <= 4500)
            {
                spawnTimeInterval = 700;
            }
            else if (score > 3500 && score <= 4000)
            {
                spawnTimeInterval = 600;
            }
            else if (score > 4000 && score <= 5000)
            {
                spawnTimeInterval = 700;
            }
            else if (score > 5000 && score <= 6000)
            {
                spawnTimeInterval = 600;
            }
            else if (score > 6000 && score <= 7000)
            {
                spawnTimeInterval = 500;
            }
            else if (score > 7000)
            {
                spawnTimeInterval = 400;
            }

            if (score > 4000) 
            {
                GenerateRandomNumberOfObstacles();
            }
        }

        //Generates a random number of obstacles at a time
        private void GenerateRandomNumberOfObstacles()
        {
            int randNumber = random.Next(1, 9);

            if (randNumber <= 3)
            {
                numberOfObstacles = 2;
            }
            else if (randNumber > 3 && randNumber <= 6)
            {
                numberOfObstacles = 3;
            }
            else
            {
                numberOfObstacles = 4;
            }
        }

        //Spawns a randomly generated item at a certain interval
        private void UpdateSpawnItems(GameTime gameTime)
        {
            // Keep track of time which last item spawned
            previousSpawnTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Spawn item when it has reached the interval time
            if (previousSpawnTime >= spawnTimeInterval)
            {
                SpawnRandomItem();
            }
        }

        private void SpawnRandomItem()
        {
            //Generates a random number to select an item to spawn
            int randomNumber = random.Next(1, 18);

            //Reset time
            previousSpawnTime = 0;

            //If the player has retrieved 3 flying objects, stop regular
            //spawning and instead spawn a batch of gems
            if (player.ItemsCollected["FlyingObjects"] >= 3)
            {
                //Spawn gems in a 6 by 6 matrix
                SpawnGemLines(6, 6);

                //Reset flying objects collected
                player.ItemsCollected["FlyingObjects"] = 0;
                player.AccumulatedPowerUpsUsed++;
            }

            else if (randomNumber <= 3)
            {
                SpawnEnemyWithObject();
            }

            else if (randomNumber >= 4 && randomNumber <= 6)
            {
                int rand = random.Next(1, 10);

                if (rand <= 5)
                    SpawnObstacles(Obstacle.SpawnSide.Left, numberOfObstacles, obstacleTimeDifference);
                else
                    SpawnObstacles(Obstacle.SpawnSide.Right, numberOfObstacles, obstacleTimeDifference);
            }

            else if (randomNumber >= 7 && randomNumber <= 9)
            {
                SpawnEnemy();
            }

            else if (randomNumber >= 10 && randomNumber <= 12)
            {
                SpawnFlyingEnemy();
            }

            else if (randomNumber >= 13 && randomNumber <= 15)
            {
                int rand = random.Next(1, 15);

                //Randomly choose the gem spawn state
                if (rand <= 5)
                {
                    SpawnGemsInLine(SpawnGemState.VerticalLeft, 5);
                }
                else if (rand > 5 && rand <= 10)
                {
                    SpawnGemsInLine(SpawnGemState.VerticalRight, 5);
                }
                else if (rand > 10 && rand <= 15)
                {
                    SpawnGemsInLine(SpawnGemState.Horizontal, 6);
                }
            }

            else if (randomNumber >= 16 && randomNumber <= 18)
            {
                HorizontalEnemy horizontalEnemy = new HorizontalEnemy(ScreenManager.Game,
                    new Animation(animationStore["HorizontalEnemy"]), player, horizontalEnemyPlatformTexture);

                horizontalEnemy.Initialize();

                horizontalEnemies.Add(horizontalEnemy);
            }
        }

        //Spawns a single or group of obstacles
        private void SpawnObstacles(Obstacle.SpawnSide startSpawnSide, int numberOfObstacles, float distanceSeperation)
        {
            float seperation = distanceSeperation;
            Obstacle.SpawnSide side = startSpawnSide;

            for (int i = 0; i < numberOfObstacles; i++)
            {
                Obstacle obstacle = new Obstacle(ScreenManager.Game);
                obstacles.Add(obstacle);

                obstacle.Initialize(obstacleTexture, player, side);

                if (numberOfObstacles > 1)
                {
                    //Time to delay the next spawn if more than two obstacles are spawned
                    //The delay time is determined by the amount of time the obstacle has the travel
                    //by doing the following calculation: (distance / velocity) / # frames per second
                    float delayTime = (distanceSeperation / player.runSpeed) * 1000;

                    previousSpawnTime -= delayTime;

                    obstacle.Position -= new Vector2(0, seperation);

                    if (side == Obstacle.SpawnSide.Right)
                        side = Obstacle.SpawnSide.Left;
                    else
                        side = Obstacle.SpawnSide.Right;

                    seperation += distanceSeperation;
                }
            }
        }

        //Spawns gems in a vertical or horizontal line
        private void SpawnGemsInLine(SpawnGemState gemState, uint numberOfGems)
        {
            //Position the line of gems will start
            Vector2 startPosition = Vector2.Zero;

            //Amount of horizontal spacing between gems
            float horizontalSpacing = (viewport.Width - leftWall.Width * 2) / (numberOfGems + 1);

            //Amount of vertical spacing between gems
            float verticalSpacing = 20;

            //Chooses how the gems will be spawned
            switch (gemState)
            {
                case SpawnGemState.VerticalLeft:
                    startPosition = new Vector2(player.marginLeft, player.Position.Y - player.marginVertical - player.verticalOffset);
                    break;

                case SpawnGemState.VerticalRight:
                    startPosition = new Vector2(player.marginRight, player.Position.Y - player.marginVertical - player.verticalOffset);
                    break;

                case SpawnGemState.Horizontal:
                    startPosition = new Vector2(leftWall.Width + horizontalSpacing, player.Position.Y - player.marginVertical - player.verticalOffset);
                    break;
            }

            //Delay the next spawn time if we are spawning in a vertical line
            if (gemState == SpawnGemState.VerticalLeft || gemState == SpawnGemState.VerticalRight)
            {
                float delayNextSpawnTime = ((verticalSpacing + animationStore["Gem"].FrameHeight) / player.runSpeed) * 1000 * numberOfGems;
                previousSpawnTime -= delayNextSpawnTime;
            }

            //Initialize gems based on the gem spawn state
            for (int i = 0; i < numberOfGems; i++)
            {
                Gem gem = null;

                if (gemState == SpawnGemState.VerticalLeft || gemState == SpawnGemState.VerticalRight)
                {
                    gem = new Gem(ScreenManager.Game, new Animation(animationStore["Gem"]),
                        startPosition - new Vector2(0, (animationStore["Gem"].FrameHeight + verticalSpacing) * i),
                        player);
                }
                else if (gemState == SpawnGemState.Horizontal)
                {
                    //Space gems out evenly
                    gem = new Gem(ScreenManager.Game, new Animation(animationStore["Gem"]),
                        startPosition + new Vector2(horizontalSpacing * i, 0), player);
                }

                //Add gem to list
                gems.Add(gem);
            }
        }

        private void SpawnGemLines(int numberOfLines, int numberOfGemsPerLine)
        {
            //Amount of horizontal spacing between gems
            float horizontalSpacing = (viewport.Width - leftWall.Width * 2) / (numberOfGemsPerLine + 1);

            //Amount of vertical spacing between gems
            float verticalSpacing = 80;

            float delayNextSpawnTime = ((verticalSpacing + animationStore["Gem"].FrameHeight) / player.runSpeed) * 1000 * numberOfLines;
            previousSpawnTime -= delayNextSpawnTime;

            Vector2 startPosition = new Vector2(leftWall.Width + horizontalSpacing, player.Position.Y - player.marginVertical - player.verticalOffset);

            //Initialize gems based on the gem spawn state
            for (int i = 0; i < numberOfLines; i++)
            {
                for (int j = 0; j < numberOfGemsPerLine; j++)
                {
                    Gem gem = null;

                    //Space gems out evenly
                    gem = new Gem(ScreenManager.Game, new Animation(animationStore["Gem"]),
                         startPosition + new Vector2(horizontalSpacing * j, -(verticalSpacing * i)), player);

                    //Add gem to list
                    gems.Add(gem);
                }
            }
        }

        private void SpawnEnemyWithObject()
        {
            EnemyWithObject enemy;

            enemy = new EnemyWithObject(ScreenManager.Game, player, ScreenManager.SpriteBatch,
                numOfObjects, animationStore);

            enemy.Initialize();
            enemiesWithObject.Add(enemy);
        }

        private void SpawnEnemy()
        {
            Enemy enemy = new Enemy(ScreenManager.Game, new Animation(animationStore["EnemyRun"]),
                player, ScreenManager.SpriteBatch);

            enemy.Initialize();
            enemies.Add(enemy);
        }

        private void SpawnFlyingEnemy()
        {
            FlyingEnemy flyingEnemy = new FlyingEnemy(ScreenManager.Game, ScreenManager.SpriteBatch,
                new Animation(animationStore["Kunai"]), player);

            flyingEnemies.Add(flyingEnemy);
            AudioManager.PlaySound("Shuriken_Throw");
        }

        #endregion

        #region Update Asset Methods

        private void UpdateObstacles(GameTime gameTime)
        {
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                //Update items
                obstacles[i].Update(gameTime);

                //Remove items if inactive
                if (!obstacles[i].Active)
                {
                    obstacles.RemoveAt(i);
                }
            }
        }

        private void UpdateGems(GameTime gameTime)
        {
            for (int i = gems.Count - 1; i >= 0; i--)
            {
                gems[i].Update(gameTime);

                if (!gems[i].Active)
                {
                    gems.RemoveAt(i);
                }
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                //Update items
                enemies[i].Update(gameTime);

                //Remove items if inactive
                if (!enemies[i].Active)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateEnemiesWithObject(GameTime gameTime)
        {
            for (int i = enemiesWithObject.Count - 1; i >= 0; i--)
            {
                // Update items
                enemiesWithObject[i].Update(gameTime);

                for (int j = enemiesWithObject[i].flyingObjects.Count - 1; j >= 0; j--)
                {
                    if (!enemiesWithObject[i].flyingObjects[j].Active)
                    {
                        enemiesWithObject[i].flyingObjects.RemoveAt(j);
                    }
                }

                //Remove items if inactive
                if (!enemiesWithObject[i].Active)
                {
                    enemiesWithObject.RemoveAt(i);
                }
            }
        }

        private void UpdateFlyingEnemies(GameTime gameTime)
        {
            for (int i = flyingEnemies.Count - 1; i >= 0; i--)
            {
                // Update items
                flyingEnemies[i].Update(gameTime);

                //Remove items if inactive
                if (!flyingEnemies[i].Active)
                {
                    flyingEnemies.RemoveAt(i);
                }
            }
        }

        private void UpdateHorizontalEnemies(GameTime gameTime)
        {
            for (int i = horizontalEnemies.Count - 1; i >= 0; i--)
            {
                // Update horizontal enemy
                horizontalEnemies[i].Update(gameTime);

                //Remove if inactive
                if (!horizontalEnemies[i].Active)
                {
                    horizontalEnemies.RemoveAt(i);
                }
            }
        }

        private void UpdateBackground(GameTime gameTime)
        {
            castleLayer.Update(gameTime);
            mountainLayer1.Update(gameTime);
            mountainLayer2.Update(gameTime);
        }

        private void UpdateWalls(GameTime gameTime)
        {
            //Create a slide in transition of left wall
            if (leftWall.xCoordinate < 0)
            {
                leftWall.xCoordinate += (float)(leftWall.horizontalScrollSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                //Set the wall position to exactly the edge of the 
                //left screen incase the wall moves a bit past the edge.
                leftWall.xCoordinate = 0;

                //Left wall has finished transitioning
                leftWall.FinishedTransition = true;
            }

            //Update left wall
            leftWall.Update(gameTime);

            //Create a slide in transition of right wall
            if (rightWall.xCoordinate > viewport.Width - rightWall.Width)
            {
                rightWall.xCoordinate -= (float)(rightWall.horizontalScrollSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                //Set the wall position to exactly the edge of the 
                //right screen incase the wall moves a bit past the edge.
                rightWall.xCoordinate = viewport.Width - rightWall.Width;

                //Right wall has finished transitioning
                rightWall.FinishedTransition = true;
            }

            //Update right wall
            rightWall.Update(gameTime);

            FinishedWallTransition = leftWall.FinishedTransition && rightWall.FinishedTransition;
        }

        private void UpdateMistBackgrounds(GameTime gameTime)
        {
            mistLayer1.Update(gameTime);
            mistLayer2.Update(gameTime);
        }

        #endregion  

        #region Update Collision Methods

        private void UpdateCollisions()
        {
            if (player.PlayerState != Player.State.Hit)
            {
                UpdateObstacleCollisions();
                UpdateEnemyCollisions();
                UpdateEnemyWithObjectCollisions();
                UpdateFlyingEnemyCollisions();
                UpdateGemCollisions();
                UpdateHorizontalEnemyCollisions();
            }
        }

        private void UpdateObstacleCollisions()
        {
            //Obstacle collision handling
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                if (player.CollisionRectangle.Intersects(obstacles[i].CollisionRectangle))
                {
                    Animation obstacleCollisionAnimation = new Animation(animationStore["ObstacleCollision"]);
                    obstacleCollisionAnimation.Position = obstacles[i].Position;
                    PlayCollisionAnimation(obstacleCollisionAnimation);

                    InitiateHitCollision();
                    obstacles[i].Active = false;
                }
            }
        }

        private void UpdateEnemyCollisions()
        {
            //Enemy collision handling
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (player.CollisionRectangle.Intersects(enemies[i].CollisionRectangle))
                {
                    InitiateHitCollision();
                    enemies[i].Active = false;
                }
            }
        }

        private void UpdateEnemyWithObjectCollisions()
        {
            //Enemies with object collision handling
            for (int i = enemiesWithObject.Count - 1; i >= 0; i--)
            {
                //If the player has collided with the enemy throwing the object, initiate hit collision events
                if (player.CollisionRectangle.Intersects(enemiesWithObject[i].CollisionRectangle))
                {
                    if(!enemiesWithObject[i].CollidedWithPlayer)
                    InitiateHitCollision();
                    enemiesWithObject[i].CollidedWithPlayer = true;
                }

                for (int j = enemiesWithObject[i].flyingObjects.Count - 1; j >= 0; j--)
                {
                    if (player.CollisionRectangle.Intersects(enemiesWithObject[i].flyingObjects[j].CollisionRectangle))
                    {
                        enemiesWithObject[i].flyingObjects[j].Active = false;

                        //If the player was running initiate hit collision events
                        if (player.PlayerState == Player.State.RunLeft || player.PlayerState == Player.State.RunRight)
                        {
                            InitiateHitCollision();
                        }
                        else if (player.PlayerState == Player.State.Jump)
                        {
                            AudioManager.PlaySound("Shuriken_Metal_Hit");
                            player.IncrementItemCollected("NinjaStars");

                            //If player has collected three stars initiate power up
                            if (player.ItemsCollected["NinjaStars"] >= 3)
                            {
                                player.PlayerState = Player.State.NinjaStarPowerUp;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateFlyingEnemyCollisions()
        {
            for (int i = 0; i < flyingEnemies.Count; i++)
            {
                if (player.CollisionRectangle.Intersects(flyingEnemies[i].CollisionRectangle))
                {
                    if (player.PlayerState == Player.State.Jump)
                    {
                        AudioManager.PlaySound("Shuriken_Metal_Hit");
                        flyingEnemies[i].Active = false;
                        player.IncrementItemCollected("FlyingObjects");
                    }
                    else if (player.PlayerState == Player.State.RunLeft || player.PlayerState == Player.State.RunRight)
                    {
                        InitiateHitCollision();
                        flyingEnemies[i].Active = false;
                    }
                }
            }
        }

        private void UpdateGemCollisions()
        {
            for (int i = 0; i < gems.Count; i++)
            {
                if(player.CollisionRectangle.Intersects(gems[i].CollisionRectangle))
                {
                    AudioManager.PlaySound("GemCollected");
                    gems[i].Active = false;

                    //Add to gems in storage so far (can reset to 0 if 50 have been collected)
                    player.GemsCollected++;

                    //Add to total gems collected so far
                    player.AccumulatedGems++;
                }
            }
        }

        private void UpdateHorizontalEnemyCollisions()
        {
            for (int i = 0; i < horizontalEnemies.Count; i++)
            {
                if (player.CollisionRectangle.Intersects(horizontalEnemies[i].CollisionRectangle))
                {
                    if (player.PlayerState == Player.State.RunLeft || player.PlayerState == Player.State.RunRight)
                    {
                        if (!horizontalEnemies[i].CollidedWithPlayer)
                        {
                            InitiateHitCollision();
                            horizontalEnemies[i].CollidedWithPlayer = true;
                        }
                    }
                    else if (player.PlayerState == Player.State.Jump)
                    {
                        if (!horizontalEnemies[i].CollidedWithPlayer)
                        {
                            horizontalEnemies[i].CollidedWithPlayer = true;
                            player.IncrementItemCollected("HorizontalEnemies");
                            AudioManager.PlaySound("HorizontalEnemyHit");
                        }
                    }

                    if (player.ItemsCollected["HorizontalEnemies"] >= 3)
                    {
                        player.PlayerState = Player.State.HorizontalEnemyPowerUp;
                    }
                }
            }
        }

        //Updates all the animations in the collision animation collection
        private void UpdateCollisionAnimations(GameTime gameTime)
        {
            for (int i = collisionAnimations.Count - 1; i >= 0; i--)
            {
                //Update animation
                collisionAnimations[i].Update(gameTime);

                //Remove from collection if inactive
                if (collisionAnimations[i].Active == false)
                {
                    collisionAnimations.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Initiate Event Methods

        private void InitiateHitCollision()
        {
            //player.PreviousState = player.PlayerState;
            if (player.PlayerState != Player.State.NinjaStarPowerUp && 
                player.PlayerState != Player.State.Idle &&
                player.PlayerState != Player.State.HorizontalEnemyPowerUp && 
                player.ShieldOn != true)
            {
                player.PlayerState = Player.State.Hit;
                player.StartMotionPosition = player.Position;
                AudioManager.PlaySound("PlayerFall");
            }

            if (player.ShieldOn == true &&
                player.PlayerState != Player.State.NinjaStarPowerUp &&
                player.PlayerState != Player.State.Idle &&
                player.PlayerState != Player.State.HorizontalEnemyPowerUp)
            {
                player.ShieldOn = false;
            }
        }

        private void InitiateGameOverScreen(GameTime gameTime)
        {
            //Keep track of how much time has elapsed since game over
            gameOverTime += gameTime.ElapsedGameTime;

            //Time to remove the game screen and show the game over screen
            if (gameOverTime > initiateGameOver)
            {
                // If is in high score, get the player's name
                if (HighScoreScreen.IsInHighscores(score))
                {
                    if (!Guide.IsVisible)
                    {
                        Guide.BeginShowKeyboardInput(PlayerIndex.One,
                            "YOU GOT A HIGH SCORE!", "What is your name (max 6 characters)?", "Player",
                                AfterPlayerEnterName, null);
                    }
                }
                else
                {
                    ShowGameOverScreen();
                }

                AudioManager.PlaySound("Game_Over");
                gameOver = true;
            }
        }

        private void ShowGameOverScreen()
        {
            GameoverScreen gameOverScreen = new GameoverScreen();

            //Keep track of the final score
            gameOverScreen.FinalPlayerScore = score;

            //Keep track of total gems collected
            gameOverScreen.TotalGemsCollected = player.AccumulatedGems;

            gameOverScreen.TotalPowerUpsUsed = player.AccumulatedPowerUpsUsed;

            gameOverScreen.TotalShieldsUsed = player.AccumulatedShieldsUsed;

            //Remove the gameplay screen and add the gameover screen
            ScreenManager.AddScreen(gameOverScreen, null);
        }

        private void AfterPlayerEnterName(IAsyncResult result)
        {
            // Get the name entered by the user
            string playerName = Guide.EndShowKeyboardInput(result);

            if (!string.IsNullOrEmpty(playerName))
            {
                // Ensure that it is valid
                if (playerName != null && playerName.Length > 6)
                {
                    playerName = playerName.Substring(0, 6);
                }

                // Puts it in high score
                HighScoreScreen.PutHighScore(playerName, score);
            }

            ShowGameOverScreen();
        }

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            bool buttonTouched = false;

            // User presses the back button
            if (input.IsPauseGame(null))
            {
                if (!gamePaused)
                {
                    gamePaused = true;
                }
                else
                {
                    gamePaused = false;
                }
            }

            if (player.FinishedTransition)
            {
                if (input.TouchState.Count > 0 && input.TouchState[0].State == TouchLocationState.Pressed)
                {
                    if (gamePaused)
                    {
                        //Resume game
                        gamePaused = false;
                        return;
                    }

                    if (pauseButton.ButtonRect.Contains((int)input.TouchState[0].Position.X, (int)input.TouchState[0].Position.Y))
                    {
                        //Pause game
                        buttonTouched = true;
                        pauseButton.OnSelectEntry();
                    }

                    if (!buttonTouched && !gamePaused)
                    {
                        // Check if the ninja is not currently jumping, falling, and using a power up
                        if (player.PlayerState != Player.State.Jump && player.PlayerState != Player.State.Hit &&
                            player.PlayerState != Player.State.NinjaStarPowerUp && player.PlayerState != Player.State.Idle &&
                            player.PlayerState != Player.State.HorizontalEnemyPowerUp)
                        {
                            player.StartMotionPosition = player.Position;

                            // Save the previous state to know where the ninja should jump (left/right)
                            player.PreviousState = player.PlayerState;

                            // Make the current state of the ninja to jump
                            player.PlayerState = Player.State.Jump;

                            AudioManager.PlaySound("PlayerJump");
                        }
                    }
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            DrawBackground();
            spriteBatch.End();

            Matrix cameraTransform = Matrix.CreateTranslation(0.0f, -cameraPosition, 0.0f);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            DrawObstacles(spriteBatch);
            DrawGems(spriteBatch);
            DrawEnemies();
            DrawFlyingEnemies();
            DrawEnemiesWithObject();
            DrawHorizontalEnemies(spriteBatch);
            DrawCollisionAnimations(spriteBatch);

            if (GameStarted)
            {
                // Draw the ninja player
                player.Draw(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin();

            if (FinishedWallTransition)
            {
                if (!gameOver)
                {
                    DrawPlayerScore(spriteBatch);
                }

                if (gamePaused)
                {
                    spriteBatch.Draw(pauseTextTexture, Vector2.Zero, Color.White);
                }

                DrawPlayerHud(spriteBatch);
            }

            spriteBatch.End();

            if (FinishedWallTransition)
            {
                base.Draw(gameTime);
            }
        }

        #endregion

        #region Draw Helper Methods

        void DrawPlayerScore(SpriteBatch spriteBatch)
        {
            score = (int)(elapsedGameplayTime / 30);
            Vector2 scoreTextPosition = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 scoreTextOrigin = scoreFont.MeasureString(score.ToString()) / 2;
            Vector2 shadowOffset = new Vector2(5, 5);
            float scoreTextScale = 1.75f;

            spriteBatch.DrawString(scoreFont, score.ToString(), scoreTextPosition + shadowOffset, Color.Black,
                0.0f, scoreTextOrigin, scoreTextScale, SpriteEffects.None, 0);

            spriteBatch.DrawString(scoreFont, score.ToString(), scoreTextPosition, Color.White,
                0.0f, scoreTextOrigin, scoreTextScale, SpriteEffects.None, 0);
        }

        void DrawPlayerHud(SpriteBatch spriteBatch)
        {
            Vector2 gemPosition = new Vector2(50, ScreenManager.Game.GraphicsDevice.Viewport.Height - 50);

            Vector2 gemTextPosition = gemPosition + new Vector2((hudGemAnimation.FrameWidth / 2) + 25, 5);
            Vector2 gemTextOrigin = hudGemFont.MeasureString(player.GemsCollected.ToString()) / 2;
            Vector2 shadowOffset = new Vector2(5, 5);

            hudGemAnimation.Draw(spriteBatch, gemPosition, SpriteEffects.None);

            spriteBatch.DrawString(hudGemFont, player.GemsCollected.ToString(), gemTextPosition + shadowOffset, Color.Black,
                0.0f, gemTextOrigin, 1.0f, SpriteEffects.None, 0);

            spriteBatch.DrawString(hudGemFont, player.GemsCollected.ToString(), gemTextPosition, Color.White,
                0.0f, gemTextOrigin, 1.0f, SpriteEffects.None, 0);

            Vector2 hudItemCollectedStartPosition = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width - 200,
                ScreenManager.Game.GraphicsDevice.Viewport.Height - 50);

            int spacing = 60;

            if (player.ItemsCollected["NinjaStars"] > 0)
            {
                for (int i = 0; i < player.ItemsCollected["NinjaStars"]; i++)
                {
                    hudNinjaStarsAnimations[i].Draw(spriteBatch, 
                        hudItemCollectedStartPosition + new Vector2(spacing*i, 0), SpriteEffects.None);
                }
            }

            if (player.ItemsCollected["HorizontalEnemies"] > 0)
            {
                for (int i = 0; i < player.ItemsCollected["HorizontalEnemies"]; i++)
                {
                    hudHorizontalEnemyAnimations[i].Draw(spriteBatch,
                        hudItemCollectedStartPosition + new Vector2(spacing * i, 0), SpriteEffects.None);
                }
            }

            if (player.ItemsCollected["FlyingObjects"] > 0)
            {
                for (int i = 0; i < player.ItemsCollected["FlyingObjects"]; i++)
                {
                    hudFlyingObjectAnimations[i].Draw(spriteBatch,
                        hudItemCollectedStartPosition + new Vector2(spacing * i, 0), SpriteEffects.None);
                }
            }
        }

        void DrawBackground()
        {
            staticLayer.Draw();
            //mistLayer2.Draw();
            mountainLayer2.Draw();
            //mistLayer1.Draw();
            mountainLayer1.Draw();
            castleLayer.Draw();

            if (GameStarted)
            {
                leftWall.Draw();
                rightWall.Draw();
            }
            
        }

        void DrawObstacles(SpriteBatch spriteBatch)
        {      
            foreach (Obstacle obstacle in obstacles)
            {
                obstacle.Draw(spriteBatch);
            }  
        }

        void DrawGems(SpriteBatch spriteBatch)
        {
            foreach (Gem gem in gems)
            {
                gem.Draw(spriteBatch);
            }
        }

        void DrawEnemies()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw();
            } 
        }

        void DrawEnemiesWithObject()
        {
            foreach (EnemyWithObject enemyWithObject in enemiesWithObject)
            {
                enemyWithObject.Draw();
            }
        }

        void DrawFlyingEnemies()
        {
            foreach (FlyingEnemy flyingEnemy in flyingEnemies)
            {
                flyingEnemy.Draw();
                
            }
        }

        void DrawHorizontalEnemies(SpriteBatch spriteBatch)
        {
            foreach (HorizontalEnemy horizontalEnemy in horizontalEnemies)
            {
                horizontalEnemy.Draw(spriteBatch);
            }
        }

        private void DrawCollisionAnimations(SpriteBatch spriteBatch)
        {
            foreach (Animation collisionAnimation in collisionAnimations)
            {
                collisionAnimation.Draw(spriteBatch);
            }
        }

        #endregion

        #region Misc. Methods

        public void ScrollCamera(uint scrollSpeed, ScrollDirection direction)
        {
            if (direction == ScrollDirection.Up)
            {
                cameraPosition += -(scrollSpeed * elapsedTime);
            }
            else if (direction == ScrollDirection.Down)
            {
                cameraPosition += (scrollSpeed * elapsedTime);
            }
        }

        private void PlayCollisionAnimation(Animation animation)
        {
            collisionAnimations.Add(animation);
        }

        #endregion
    }
}
