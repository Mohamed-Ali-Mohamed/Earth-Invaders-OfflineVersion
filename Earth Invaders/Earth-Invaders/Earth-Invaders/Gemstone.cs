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
    /// Gemstone is the gems that appears in a fixed location in the game
    /// it increases the player score after taking it
    /// </summary>
    public class Gemstone : GameObject
    {
        #region Constructor
        /// <summary>
        /// constructs a gemStone
        /// by setting its location and loading its image then playing the animation
        /// it sets the collision rectangle
        /// </summary>
        /// <param name="Content">Load the image from the disk into the memory</param>
        /// <param name="cellX">the x position of the tile on which the gem is drawn</param>
        /// <param name="cellY">the y position of the tile on which the gem is drawn</param>
        public Gemstone(ContentManager Content, int cellX, int cellY)
        {
            worldLocation.X = TileMap.TileWidth * cellX;
            worldLocation.Y = TileMap.TileHeight * cellY;

            frameWidth = TileMap.TileWidth;
            frameHeight = TileMap.TileHeight;

            animations.Add("idle",
                new AnimationStrip(
                    Content.Load<Texture2D>(@"Textures\Sprites\Gem"),
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
