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
    /// <summary>
    /// this class Defines the health hearts that increases 
    /// the player's health after taking it
    /// </summary>
    /// <param name="Content">to load the image form the disk into the memory to be able to draw it</param>
    /// <param name="cellX">the x index of the tile on which the health item will be drawn</param>
    /// <param name="cellY">the y index of the tile on which the health item will be drawn</param>
    public class Health : GameObject
    {
        #region Constructor
        public Health(ContentManager Content, int cellX, int cellY)
        {
            worldLocation.X = TileMap.TileWidth * cellX;
            worldLocation.Y = TileMap.TileHeight * cellY;

            frameWidth = TileMap.TileWidth;
            frameHeight = TileMap.TileHeight;

            animations.Add("idle",
                new AnimationStrip(
                    Content.Load<Texture2D>(@"Textures\Sprites\Health"),
                    48,
                    "idle"));

            animations["idle"].LoopAnimation = true;
            animations["idle"].FrameLength = 0.15f;
            PlayAnimation("idle");
            drawDepth = 0.875f;

            CollisionRectangle = new Rectangle(9, 24, 30, 24);
            enabled = true;
        }
        #endregion

    }
}
