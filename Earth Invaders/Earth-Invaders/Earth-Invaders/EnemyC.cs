using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tile_Engine;

namespace Earth_Invaders
{
    public class EnemyC : Enemy
    {
        #region Declarations
        //Health bar
        Texture2D mHealthBar;
        //
        #endregion
        private Vector2 oldLocation;



        #region Constructor

        public EnemyC(ContentManager content, int cellX, int cellY)
        {

            //////////////////////////////////////////////////////////////////////////
            //Health bar
            //Load the HealthBar image from the disk into the Texture2D object
            mHealthBar = content.Load<Texture2D>(@"Textures\\Sprites\\HealthBar6") as Texture2D;
            //////////////////////////////////////////////////////////////////////////
            animations.Add("idle",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterC\Idle"),
                    118,
                    "idle"));//A=50  B=100  C=118
            animations["idle"].LoopAnimation = true;

            animations.Add("run",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterC\Run"),
                    145,
                    "run"));//A=63  B=150  C=145
            animations["run"].FrameLength = 0.1f;
            animations["run"].LoopAnimation = true;

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterC\Die"),
                    145,
                    "die"));//A=92  B=150  C=145
            animations["die"].LoopAnimation = false;

            frameWidth = 64;
            frameHeight = 64;
            CollisionRectangle = new Rectangle(12, 1, 40, 62);

            worldLocation = new Vector2(
                cellX * TileMap.TileWidth,
                cellY * TileMap.TileHeight);

            enabled = true;

            codeBasedBlocks = true;
            PlayAnimation("run");
        }
        #endregion

        #region Public Methods
        public void Attack(Vector2 target)
        {
            codeBasedBlocks = false;
            int x = -1, y = -1;
            velocity = new Vector2(0, 0);

            if (Math.Abs(target.Y - worldLocation.Y) < 60)
            {
                y = -1;

            }
            else if (Math.Abs(target.Y - worldLocation.Y) > 200)
            {
                y = 1;
            }


            if (Math.Abs(target.X - worldLocation.X) > 100)
            {
                if (target.X < WorldLocation.X)
                {
                    flipped = false;
                    facingLeft = true;
                    x = -1;
                }
                else if (target.X > WorldLocation.X)
                {
                    flipped = true;
                    facingLeft = false;
                    x = 1;
                }
                if (Math.Abs(oldLocation.X - WorldLocation.X) < 2)
                {
                    y = -1;
                }
                walkSpeed = 200;
            }
            else
            {
                walkSpeed = 120;


                if (Math.Abs(target.X - worldLocation.X) >= 99)
                {
                    x *= -1;
                    y *= -1;
                    flipped = !flipped;
                    facingLeft = !facingLeft;
                }
                else
                {
                    if (flipped && !facingLeft)
                    {
                        x = 1;
                    }
                    else
                    {
                        x = -1;
                    }
                }

            }
            direction = new Vector2(x, y);
            oldLocation = WorldLocation;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //Health bar
            //Draw the health for the health bar
            int width = (int)((50) * (healthCount / 5.0));
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), width, 10)),
                new Rectangle(0, 45, 10, 10), Color.Blue);
            //Draw the box around the health bar
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), 50, 16)),
                new Rectangle(0, 0, 50, 16), Color.White);
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            base.Draw(spriteBatch);


        }
        #endregion
    }

}
