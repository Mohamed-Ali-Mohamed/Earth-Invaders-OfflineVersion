using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tile_Engine;

namespace Earth_Invaders
{
    /// <summary>
    /// GameObject is the class of any moving object in the game (player, Enemy, Fire, .. etc)
    /// It describes the main characteristics of each object in the game
    /// such as (world location and velocity and collision rectangle and animation)
    /// </summary>
    public class GameObject
    {
        #region Declarations
        protected Vector2 worldLocation;
        protected Vector2 velocity;
        protected int frameWidth;
        protected int frameHeight;

        protected bool enabled;
        protected bool flipped = false;
        protected bool onGround;

        protected Rectangle collisionRectangle;
        protected int collideWidth;
        protected int collideHeight;
        protected bool codeBasedBlocks = true;

        protected float drawDepth = 0.85f;
        protected Dictionary<string, AnimationStrip> animations =
            new Dictionary<string, AnimationStrip>();
        protected string currentAnimation;
        #endregion

        #region Properties
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }

        public Vector2 WorldCenter
        {
            get
            {
                return new Vector2(
                  (int)worldLocation.X + (int)(frameWidth / 2),
                  (int)worldLocation.Y + (int)(frameHeight / 2));
            }
        }

        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle(
                    (int)worldLocation.X,
                    (int)worldLocation.Y,
                    frameWidth,
                    frameHeight);
            }
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle(
                    (int)worldLocation.X + collisionRectangle.X,
                    (int)WorldRectangle.Y + collisionRectangle.Y,
                    collisionRectangle.Width,
                    collisionRectangle.Height);
            }
            set { collisionRectangle = value; }
        }

        public bool Flipped
        {
            get { return flipped; }
            set { flipped = value; }
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// update the animation of game object
        /// it checks if the current animation has been finished
        /// if finished it plays the next animation
        /// else it updates the current animation
        /// </summary>
        /// <param name="gameTime"> the time elapsed from the last call</param>
        private void updateAnimation(GameTime gameTime)
        {
            if (animations.ContainsKey(currentAnimation))
            {
                if (animations[currentAnimation].FinishedPlaying)
                {
                    PlayAnimation(animations[currentAnimation].NextAnimation);
                }
                else
                {
                    animations[currentAnimation].Update(gameTime);
                }
            }
        }
        #endregion

        #region Map-Based Collision Detection Methods
        /// <summary>
        /// test horizontal component of movement of game objects
        /// it tests the collision rectangle of the game object
        /// after the horizontal movement and checks if the new position
        /// is passable or not and then returns the right move amount
        /// </summary>
        /// <param name="moveAmount">move amount of the object before testing</param>
        /// <returns> returns the move amount after testing the horizontal component</returns>
        private Vector2 horizontalCollisionTest(Vector2 moveAmount)
        {
            if (moveAmount.X == 0)
                return moveAmount;

            Rectangle afterMoveRect = CollisionRectangle;
            afterMoveRect.Offset((int)moveAmount.X, 0);
            Vector2 corner1, corner2;

            if (moveAmount.X < 0)
            {
                corner1 = new Vector2(afterMoveRect.Left,
                                      afterMoveRect.Top + 1);
                corner2 = new Vector2(afterMoveRect.Left,
                                      afterMoveRect.Bottom - 1);
            }
            else
            {
                corner1 = new Vector2(afterMoveRect.Right,
                                      afterMoveRect.Top + 1);
                corner2 = new Vector2(afterMoveRect.Right,
                                      afterMoveRect.Bottom - 1);
            }

            Vector2 mapCell1 = TileMap.GetCellByPixel(corner1);
            Vector2 mapCell2 = TileMap.GetCellByPixel(corner2);

            if (!TileMap.CellIsPassable(mapCell1) ||
                !TileMap.CellIsPassable(mapCell2))
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }

            if (codeBasedBlocks)
            {
                if (TileMap.CellCodeValue(mapCell1) == "BLOCK" ||
                    TileMap.CellCodeValue(mapCell2) == "BLOCK")
                {
                    moveAmount.X = 0;
                    velocity.X = 0;
                }
            }

            return moveAmount;
        }
        /// <summary>
        /// test vertical component of movement of game object
        /// it tests the collision rectangle of the game object
        /// after the vertical movement and checks if the new position
        /// is passable or not and then returns the right move amount
        /// </summary>
        /// <param name="moveAmount">move amount of the object before testing</param>
        /// <returns> returns the move amount after testing the vertical component</returns>
        private Vector2 verticalCollisionTest(Vector2 moveAmount)
        {
            if (moveAmount.Y == 0)
                return moveAmount;

            Rectangle afterMoveRect = CollisionRectangle;
            afterMoveRect.Offset((int)moveAmount.X, (int)moveAmount.Y);
            Vector2 corner1, corner2;

            if (moveAmount.Y < 0)
            {
                corner1 = new Vector2(afterMoveRect.Left + 1,
                                      afterMoveRect.Top);
                corner2 = new Vector2(afterMoveRect.Right - 1,
                                      afterMoveRect.Top);
            }
            else
            {
                corner1 = new Vector2(afterMoveRect.Left + 1,
                                      afterMoveRect.Bottom);
                corner2 = new Vector2(afterMoveRect.Right - 1,
                                      afterMoveRect.Bottom);
            }

            Vector2 mapCell1 = TileMap.GetCellByPixel(corner1);
            Vector2 mapCell2 = TileMap.GetCellByPixel(corner2);

            if (!TileMap.CellIsPassable(mapCell1) ||
                !TileMap.CellIsPassable(mapCell2))
            {
                if (moveAmount.Y > 0)
                    onGround = true;
                moveAmount.Y = 0;
                velocity.Y = 0;
            }

            if (codeBasedBlocks)
            {
                if (TileMap.CellCodeValue(mapCell1) == "BLOCK" ||
                    TileMap.CellCodeValue(mapCell2) == "BLOCK")
                {
                    if (moveAmount.Y > 0)
                        onGround = true;
                    moveAmount.Y = 0;
                    velocity.Y = 0;
                }
            }

            return moveAmount;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// plays the current animation of the game object
        /// it takes the animation name and uses it as a key to get 
        /// the animation from animations dictionary then plays it
        /// </summary>
        /// <param name="name">the name of the animation to be played</param>
        public void PlayAnimation(string name)
        {
            if (!(name == null) && animations.ContainsKey(name))
            {
                currentAnimation = name;
                animations[name].Play();
            }
        }
        /// <summary>
        /// Updates the game object position 
        /// after making collision tests
        /// and ensure that the game object doesn't exceed the world
        /// </summary>
        /// <param name="gameTime">the time elapsed from the last call of this method</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!enabled)
                return;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            updateAnimation(gameTime);

            if (velocity.Y != 0)
            {
                onGround = false;
            }

            Vector2 moveAmount = velocity * elapsed;

            moveAmount = horizontalCollisionTest(moveAmount);
            moveAmount = verticalCollisionTest(moveAmount);

            Vector2 newPosition = worldLocation + moveAmount;

            newPosition = new Vector2(
                MathHelper.Clamp(newPosition.X, 0,
                  Camera.WorldRectangle.Width - frameWidth),
                MathHelper.Clamp(newPosition.Y, 2 * (-TileMap.TileHeight),
                  Camera.WorldRectangle.Height - frameHeight));

            worldLocation = newPosition;
        }
        /// <summary>
        /// draw the game object using its sprite sheet
        /// and its current animation
        /// </summary>
        /// <param name="spriteBatch">sprite sheets of the current game object</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!enabled)
                return;

            if (animations.ContainsKey(currentAnimation))
            {

                SpriteEffects effect = SpriteEffects.None;

                if (flipped)
                {
                    effect = SpriteEffects.FlipHorizontally;
                }

                spriteBatch.Draw(
                    animations[currentAnimation].Texture,
                    Camera.WorldToScreen(WorldRectangle),
                    animations[currentAnimation].FrameRectangle,
                    Color.White, 0.0f, Vector2.Zero, effect, drawDepth);
            }
        }



        #endregion

    }
}
