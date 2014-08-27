using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Tile_Engine;

namespace Earth_Invaders
{
    /// <summary>
    /// The enemy main class
    /// It contains the main characteristics of the enemies
    /// It is not instantiated directly , It is inherited by another classes(EnemyA, EnemyB, .. , EnemyF)
    /// each child class defines a type of enemies
    /// </summary>
    public class Enemy : Creature
    {
        #region Declarations

        protected Vector2 fallSpeed = new Vector2(0, 20);

        protected float walkSpeed = 100.0f;

        protected bool facingLeft = true;
        public bool Dead = false;

        protected double healthCount = 5;

        public bool playerSeen = false;

        public Vector2 direction = new Vector2(1, 0);

        private double elapsedTime = 0;

        public Stack<Vector2> Vel;
        public Stack<bool> Flip;
        public Stack<Vector2> Loc;
        public Stack<bool> Pseen;
        public Enemy()
        {
            Vel = new Stack<Vector2>(50000);
            Flip = new Stack<bool>(50000);
            Loc = new Stack<Vector2>(50000);
            Pseen = new Stack<bool>(50000);
        }
        ~Enemy()
        {
            Vel.Clear();
            Flip.Clear();
            Loc.Clear();
            Pseen.Clear();
        }
        #endregion

        #region Properties
        public double ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public float WalkSpeed
        {
            get { return walkSpeed; }
            set { walkSpeed = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// updates the enemy's position after specific time
        /// if time is reversed it updates the enemy with its previous states
        /// using stacks Vel, Flip, Loc, Pseen
        /// and if time is not reversed it updates the velocity with its horizontal and vertical components
        /// it also prevents stack overflow by clearing stack when they reach their limit
        /// </summary>
        /// <param name="gameTime">the time elapsed from the last call of this method</param>
        public override void Update(GameTime gameTime)
        {
            Vector2 oldLocation = worldLocation;
            if (Player.Time_Reverse)
            {
                if (Flip.Any())
                    flipped = Flip.Pop();
                facingLeft = !flipped;
                if (flipped)
                    direction = new Vector2(-1, 0);
                else
                    direction = new Vector2(1, 0);
                if (Vel.Any())
                    velocity = Vel.Pop() * direction;
                if (Loc.Any())
                    worldLocation = Loc.Pop();
                if (Pseen.Any())
                    playerSeen = Pseen.Pop();

            }
            else if (!Dead && !Player.Time_Reverse)
            {
                if (!playerSeen)
                {
                    codeBasedBlocks = true;
                    velocity = new Vector2(0, velocity.Y);
                    direction = new Vector2(1, 0);

                    flipped = true;

                    if (facingLeft)
                    {

                        direction = new Vector2(-1, 0);

                        flipped = false;
                    }

                }
                else if (worldLocation.Y <= 30)
                {
                    velocity.Y = 0;
                }

                direction *= walkSpeed;
                velocity += direction;

                if (!playerSeen)
                    velocity += fallSpeed;

                //
                if (Vel.Count() == 49999 || Flip.Count() == 49999 || Loc.Count() == 49999 || Pseen.Count() == 49999)
                {
                    Stack<Vector2> Vel2 = new Stack<Vector2>(5000);
                    Stack<bool> Flip2 = new Stack<bool>(5000);
                    Stack<Vector2> Loc2 = new Stack<Vector2>(5000);
                    Stack<bool> Pseen2 = new Stack<bool>(5000);

                    for (int i = 0; i < 5000; i++)
                    {
                        Vel2.Push(Vel.Pop());
                        Flip2.Push(Flip.Pop());
                        Loc2.Push(Loc.Pop());
                        Pseen2.Push(Pseen.Pop());
                    }
                    Vel.Clear();
                    Flip.Clear();
                    Loc.Clear();
                    Pseen.Clear();
                    for (int i = 0; i < 5000; i++)
                    {
                        Vel.Push(Vel2.Pop());
                        Flip.Push(Flip2.Pop());
                        Loc.Push(Loc2.Pop());
                        Pseen.Push(Pseen2.Pop());
                    }
                }
                //
                Vel.Push(velocity);
                Flip.Push(flipped);
                Loc.Push(worldLocation);
                Pseen.Push(playerSeen);
                //
            }

            base.Update(gameTime);



            if (!Dead)
            {
                if (oldLocation == worldLocation)
                {
                    facingLeft = !facingLeft;
                }
            }
            else
            {
                if (animations[currentAnimation].FinishedPlaying)
                {
                    enabled = false;
                }
            }

        }
        /// <summary>
        /// Kill the enemy by playing dying animation
        /// and setting "dead" field to true
        /// </summary>
        public override void Kill()
        {
            this.PlayAnimation("die");
            this.Dead = true;
        }
        /// <summary>
        /// detect collision between the enemy and fires that come from the player
        /// if collision detected it decreases the health of the enemy and remove the fire
        /// It also detects collision between the enemy and the player
        /// if collision of this type detected it decreases the player health
        /// </summary>
        public override void CheckCollision()
        {
            for (int i = LevelManager.fires.Count - 1; i >= 0; i--)
                if (this.CollisionRectangle.Intersects(LevelManager.fires[i].CollisionRectangle) && LevelManager.fires[i].source == "PLAYER")
                {
                    if (healthCount > 1)
                        healthCount--;
                    else
                    {
                        healthCount = 0;
                        this.Kill();
                    }
                    LevelManager.fires.RemoveAt(i);
                }
            /// the next commented code is the code which detects collision between player and enemies            
            if (LevelManager.player.CollisionRectangle.Intersects(
                this.CollisionRectangle))
            {
                if (LevelManager.player._health > 1)
                {
                    LevelManager.player._health--;

                }
                else
                {
                    LevelManager.player.Kill();
                }
            }
        }



        #endregion


    }
}
