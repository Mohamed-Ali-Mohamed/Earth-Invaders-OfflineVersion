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
    public class EnemyE : Enemy
    {

        #region Declarations
        //Health bar
        Texture2D mHealthBar;
        //
        #endregion

        #region Constructor
        public EnemyE(ContentManager content, int cellX, int cellY)
        {
            healthCount = 15;
            //////////////////////////////////////////////////////////////////////////
            //Health bar
            //Load the HealthBar image from the disk into the Texture2D object
            mHealthBar = content.Load<Texture2D>(@"Textures\\Sprites\\HealthBar6") as Texture2D;
            //////////////////////////////////////////////////////////////////////////
            animations.Add("idle",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterE\Idle"),
                    158,
                    "idle"));//A=50  B=100  C=118
            animations["idle"].LoopAnimation = true;

            animations.Add("run",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterE\Run"),
                    158,
                    "run"));//A=63  B=150  C=145
            animations["run"].FrameLength = 0.1f;
            animations["run"].LoopAnimation = true;

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(
                        @"Textures\Sprites\MonsterE\Die"),
                    158,
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //Health bar
            //Draw the health for the health bar
            int width = (int)((50) * (healthCount / 15.0));
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), width, 10)),
                new Rectangle(0, 45, 10, 10), Color.Crimson);
            //Draw the box around the health bar
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), 50, 16)),
                new Rectangle(0, 0, 50, 16), Color.White);
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            base.Draw(spriteBatch);


        }
        #endregion
    }

}
