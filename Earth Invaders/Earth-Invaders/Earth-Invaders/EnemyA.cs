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
    public class EnemyA : Enemy
    {

        #region Declarations
        Vector2 oldLocation;
        //Health bar
        Texture2D mHealthBar;
        //

        float maxX;
        float minY, maxY;
        Random rand;
        float x, y;
        #endregion
        public Vector2 OldLocation
        {
            get { return oldLocation; }
        }
        #region Constructor
        public EnemyA(ContentManager content, int cellX, int cellY)
        {
            healthCount = 10;
            rand = new Random();
            ///////////////////////////////////////////////////////////////////////
            bool flag = true;
            while (flag)
            {
                maxX = 30 + rand.Next(100);
                for (int i = 0; i < LevelManager.maxXList.Count; i++)
                {
                    if (maxX == LevelManager.maxXList[i])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag == false)
                    flag = true;
                else
                {
                    flag = false;
                    LevelManager.maxXList.Add(maxX);
                }
            }
            flag = true;
            while (flag)
            {
                maxY = 100 + rand.Next(200);
                for (int i = 0; i < LevelManager.maxYList.Count; i++)
                {
                    if (maxY == LevelManager.maxYList[i])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag == false)
                    flag = true;
                else
                {
                    flag = false;
                    LevelManager.maxYList.Add(maxY);
                }
            }
            flag = true;
            while (flag)
            {
                minY = 30 + rand.Next(50);

                for (int i = 0; i < LevelManager.minYList.Count; i++)
                {
                    if (minY == LevelManager.minYList[i])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag == false)
                    flag = true;
                else
                {
                    LevelManager.minYList.Add(minY);
                    flag = false;
                }
            }

            //////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            //Health bar
            //Load the HealthBar image from the disk into the Texture2D object
            mHealthBar = content.Load<Texture2D>(@"Textures\\Sprites\\HealthBar6") as Texture2D;
            //////////////////////////////////////////////////////////////////////////
            animations.Add("idle",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterA\Idle"),
                    50,
                    "idle"));//A=50  B=100  C=118
            animations["idle"].LoopAnimation = true;

            animations.Add("run",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterA\Run"),
                    63,
                    "run"));//A=63  B=150  C=145
            animations["run"].FrameLength = 0.1f;
            animations["run"].LoopAnimation = true;

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterA\Die"),
                    92,
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
            bool flag = false;
            codeBasedBlocks = false;

            velocity = new Vector2(0, 0);

            if (!flag)
            {
                x = direction.X / (Math.Abs(direction.X));
                y = direction.Y / (Math.Abs(direction.Y));

                if (Math.Abs(target.Y - worldLocation.Y) < minY && worldLocation.Y > 10)
                {
                    y = -1;

                }
                else if (Math.Abs(target.Y - worldLocation.Y) > maxY)
                {
                    y = 1;
                }



                if (Math.Abs(target.X - worldLocation.X) > maxX)
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

                    walkSpeed = 200;
                }
                else
                {
                    walkSpeed = 120;


                    if (Math.Abs(target.X - worldLocation.X) >= maxX - 1)
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
                if (Math.Abs(oldLocation.X - WorldLocation.X) < 2)
                {
                    y = -1;
                }
                direction = new Vector2(x, y);
            }

            oldLocation = WorldLocation;

        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //Health bar
            //Draw the health for the health bar
            int width = (int)((50) * (healthCount / 10.0));
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), width, 10)),
                new Rectangle(0, 45, 10, 10), Color.BlueViolet);
            //Draw the box around the health bar
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), 50, 16)),
                new Rectangle(0, 0, 50, 16), Color.White);
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            base.Draw(spriteBatch);


        }
        #endregion
    }

}
