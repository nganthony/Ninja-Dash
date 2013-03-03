#region File Description
//-----------------------------------------------------------------------------
// Copyright (C) Anthony Ng. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Ninja_Dash
{
    /// <summary>
    /// Stores a collection of animations.
    /// </summary>
    public class AnimationStore
    {
        #region Fields

        [ContentSerializer]
        public Dictionary<string, Animation> Animations { set; private get; }

        #endregion

        #region Properties

        /// <summary>
        /// Returns an animation from the store which has the specified alias.
        /// </summary>
        /// <param name="animationAlias">Alias of the desired animation.</param>
        /// <returns>An animation object matching the specified alias.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the store does not contain an animation with the 
        /// specified alias.</exception>
        public Animation this[string animationAlias]
        {
            get
            {
                return Animations[animationAlias];
            }
        }

        #endregion

        #region Public Methods

        public AnimationStore()
        {
            Animations = new Dictionary<string, Animation>();
        }

        public void Initialize(ContentManager contentManager)
        {
            AddAnimations();

            foreach (Animation animation in Animations.Values)
            {
                animation.LoadSheet(contentManager);
            }
        }

        private void AddAnimations()
        {
            Animation playerJumpAnimation = new Animation("Textures/Player/NinjaJump", 1, true, 170, 250, 250, 0.37f);
            Animation playerRunAnimation = new Animation("Textures/Player/RunNinja", 6, true, 70, 250, 250, 0.37f);
            Animation playerFallAnimation = new Animation("Textures/Player/Fall", 4, true, 80, 250, 250, 0.37f);
            Animation playerIdleFallAnimation = new Animation("Textures/Player/IdleFall", 6, false, 100, 250, 250, 0.37f);
            Animation enemyRunAnimation = new Animation("Textures/Enemy/EnemyNinja", 1, true, 60, 250, 250, 0.30f);
            Animation ninjaStarAnimation = new Animation("Textures/NinjaStar", 5, true, 20, 30, 30, 1.5f);
            Animation rocketShipAnimation = new Animation("Textures/Rocketship", 1, true, 30, 100, 150, 1.5f);
            Animation gemAnimation = new Animation("Textures/Gem", 1, true, 30, 32, 32, 1.35f);
            Animation horizontalEnemyRunAnimation = new Animation("Textures/Enemy/Run2", 10, true, 60, 64, 64, 1.5f);
            Animation shieldAnimation = new Animation("Textures/Shield", 1, true, 60, 80, 80, 1.3f);
            Animation kunaiAnimation = new Animation("Textures/Kunai", 1, true, 60, 70, 70, 1.0f);

            //Hud
            Animation hudEnemyAnimation = new Animation("Textures/Enemy/Run3", 10, true, 60, 64, 64, 1.0f);
            Animation hudNinjaStarAnimation = new Animation("Textures/NinjaStar", 5, true, 20, 30, 30, 1.3f);
            Animation hudHorizontalEnemyAnimation = new Animation("Textures/Enemy/Run2", 10, true, 60, 64, 64, 1.0f);
            Animation hudKunaiAnimation = new Animation("Textures/Kunai", 1, true, 60, 70, 70, 0.7f);

            //Collision Animations
            Animation obstacleCollisionAnimation = new Animation("Textures/ObstacleCollision", 12, false, 60, 134, 134, 1.0f);

            Animations.Add("PlayerJump", playerJumpAnimation);
            Animations.Add("PlayerRun", playerRunAnimation);
            Animations.Add("PlayerFall", playerFallAnimation);
            Animations.Add("PlayerIdleFall", playerIdleFallAnimation);
            Animations.Add("EnemyRun", enemyRunAnimation);
            Animations.Add("NinjaStar", ninjaStarAnimation);
            Animations.Add("Rocketship", rocketShipAnimation);
            Animations.Add("Gem", gemAnimation);
            Animations.Add("HorizontalEnemy", horizontalEnemyRunAnimation);
            Animations.Add("Shield", shieldAnimation);
            Animations.Add("HudEnemy", hudEnemyAnimation);
            Animations.Add("HudHorizontalEnemy", hudHorizontalEnemyAnimation);
            Animations.Add("HudNinjaStar", hudNinjaStarAnimation);
            Animations.Add("ObstacleCollision", obstacleCollisionAnimation);
            Animations.Add("Kunai", kunaiAnimation);
            Animations.Add("HudKunai", hudKunaiAnimation);
        }

        #endregion
    }
}
