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
    public class EnemyD : Enemy
    {
        #region Declarations
        //Health bar
        Texture2D mHealthBar;
        //
        #endregion
        private Vector2 oldLocation;



        #region Constructor

        public EnemyD(ContentManager content, int cellX, int cellY)
        {
            fallSpeed.Y = 0;
            walkSpeed = 0;
            healthCount = 100;
            //////////////////////////////////////////////////////////////////////////
            //Health bar
            //Load the HealthBar image from the disk into the Texture2D object
            mHealthBar = content.Load<Texture2D>(@"Textures\\Sprites\\HealthBar6") as Texture2D;
            //////////////////////////////////////////////////////////////////////////
            animations.Add("idle",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterD\Idle"),
                    296,
                    "idle"));//A=50  B=100  C=118
            animations["idle"].LoopAnimation = true;

            animations.Add("run",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterD\Run"),
                    296,
                    "run"));//A=63  B=150  C=145
            animations["run"].FrameLength = 0.1f;
            animations["run"].LoopAnimation = true;

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterD\Die"),
                    296,
                    "die"));//A=92  B=150  C=145
            animations["die"].LoopAnimation = false;

            frameWidth = 296;
            frameHeight = 73;
            CollisionRectangle = new Rectangle(1, 1, 295, 72);

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
            float x = -1, y = 0;
            velocity = new Vector2(0, 0);


            if (Math.Abs(target.X - WorldCenter.X) > 5)
            {
                if (target.X < WorldCenter.X)
                {
                    x = -1;
                }
                else if (target.X > WorldCenter.X)
                {
                    x = 1;
                }

                walkSpeed = 280;
            }
            else
            {
                x = 0; y = 0;
                walkSpeed = 0;
            }
            
            direction = new Vector2(x, y);
            oldLocation = WorldLocation;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //Health bar
            //Draw the health for the health bar
            int width = (int)((200) * (healthCount / 100.0));
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 95, (int)(WorldCenter.Y - 35), width, 10)),
                new Rectangle(0, 45, 10, 10), Color.Yellow);
            //Draw the box around the health bar
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 95, (int)(WorldCenter.Y - 35), 200, 16)),
                new Rectangle(0, 0, 50, 16), Color.White);
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            base.Draw(spriteBatch);


        }
        #endregion
    }

}
