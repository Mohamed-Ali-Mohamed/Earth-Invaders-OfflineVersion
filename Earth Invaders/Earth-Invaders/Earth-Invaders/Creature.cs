using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Earth_Invaders
{
    /// <summary>
    /// This abstract class defines a moving creature in the game
    /// there is 2 kinds of creatures in this game : player and enemies
    /// </summary>
    public abstract class Creature : GameObject
    {
        /// <summary>
        /// kills the creature when its health = zero
        /// </summary>
        public abstract void Kill();
        /// <summary>
        /// check the collision between the creatures and each others 
        /// and collision between creatures and fires
        /// </summary>
        public abstract void CheckCollision();
    }
}
