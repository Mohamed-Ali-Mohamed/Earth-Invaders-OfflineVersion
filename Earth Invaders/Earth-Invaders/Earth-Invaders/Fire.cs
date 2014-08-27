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
    /// This class defines all types of fires in the game
    /// fires are classified into 2 main types according to their source
    /// Player's fires & enemies' fires
    /// each fire also has a limited range (determined from start position) and a direction
    /// </summary>
    public class Fire : GameObject
    {
        #region Declarations
        Vector2 startPosition;
        Vector2 Direction;
        public String source;
        public Stack<Vector2> Vel=new Stack<Vector2>(50000);
        public Stack<bool> Enabl=new Stack<bool>(50000);
        public Stack<Vector2> WorL = new Stack<Vector2>(50000);
        #endregion
        
        #region Properties

        public Vector2 _startPosition
        {
            get { return startPosition; }
        }

        #endregion
        
        #region Constructor
        /// <summary>
        /// constructs a new fire
        /// with the appropriate image according to its source
        /// and sets the collision rectangle and plays the animation
        /// </summary>
        /// <param name="content">Load the image from the disk into the memory</param>
        /// <param name="firePos">the position of shooting the fire</param>
        /// <param name="direction">the direction of the fire</param>
        /// <param name="source">the source of the fire (Player or Enemy(A -> F) )</param>
        public Fire(ContentManager content, Vector2 firePos, Vector2 direction, String source)
        {
            this.worldLocation = firePos;
            this.Direction = direction;
            this.source = source;
            startPosition = firePos;
            if (source == "PLAYER")
            {
                animations.Add("fire",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Player\Fire11"),
                        42,
                        "fire"));

                animations["fire"].LoopAnimation = true;
                animations["fire"].FrameLength = 0.08f;
                frameWidth = 10;
                frameHeight = 10;
                CollisionRectangle = new Rectangle(2, 2, 8, 8);
            }
            if (source == "ENEMY_A" || source == "ENEMY_B" || source == "ENEMY_C" || source == "ENEMY_E" )
            {
                animations.Add("fire",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Fire0"),
                        42,
                        "fire"));

                animations["fire"].LoopAnimation = true;
                animations["fire"].FrameLength = 0.08f;
                frameWidth = 10;
                frameHeight = 10;
                CollisionRectangle = new Rectangle(2, 2, 8, 8);
            }
            if (source == "ENEMY_D")
            {
                animations.Add("fire",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Fire"),
                        48,
                        "fire"));

                animations["fire"].LoopAnimation = true;
                animations["fire"].FrameLength = 0.08f;
                frameWidth = 48;
                frameHeight = 48;
                CollisionRectangle = new Rectangle(2, 2, 46, 46);
            }
            if (source == "ENEMY_F1" || source == "ENEMY_F2")
            {
                if (source == "ENEMY_F1")
                animations.Add("fire",
                    new AnimationStrip(
                        content.Load<Texture2D>(@"Textures\Sprites\Fire1"),
                        44,
                        "fire"));
                if (source == "ENEMY_F2")
                    animations.Add("fire",
                        new AnimationStrip(
                            content.Load<Texture2D>(@"Textures\Sprites\Fire2"),
                            44,
                            "fire"));
                animations["fire"].LoopAnimation = true;
                animations["fire"].FrameLength = 0.08f;
                frameWidth = 44;
                frameHeight = 17;
                CollisionRectangle = new Rectangle(2, 2, 42, 15);
            }
            

            drawDepth = 0.825f;

            enabled = true;
            codeBasedBlocks = false;


            velocity = 600 * direction;
            PlayAnimation("fire");

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the fire periodically
        /// if time is reversed it updates the fire with its states for a period of time
        /// and saves the fire's states for a specific period of time 
        /// to handle time reverse
        /// </summary>
        /// <param name="gameTime">time elapsed from last call to this method</param>
        public override void Update(GameTime gameTime)
        {
            if (Player.Time_Reverse)
            {
                Vector2 oldLocation = worldLocation;
                if(Vel.Any())
                    velocity = -1*Vel.Pop();
                if (WorL.Any())
                    worldLocation = WorL.Pop();
                if (Enabl.Any())
                    enabled = Enabl.Pop();
                if (worldLocation == _startPosition || WorldCenter==_startPosition)
                    enabled = false;
                base.Update(gameTime);
            }
            else
            {
                Vector2 oldLocation = worldLocation;
                base.Update(gameTime);
                if (oldLocation == worldLocation)
                    enabled = false;
                if (worldLocation.Y != startPosition.Y && worldLocation.Y == oldLocation.Y)
                    enabled = false;
                if (worldLocation.X != startPosition.X && worldLocation.X == oldLocation.X)
                    enabled = false;
                //
                if (Vel.Count() == 49999 || Enabl.Count() == 49999 || WorL.Count() == 49999 )
                {
                    Stack<Vector2> Vel2 = new Stack<Vector2>(5000);
                    Stack<bool> Enabl2 = new Stack<bool>(5000);
                    Stack<Vector2> WorL2 = new Stack<Vector2>(5000);

                    for (int i = 0; i < 5000; i++)
                    {
                        Vel2.Push(Vel.Pop());
                        Enabl2.Push(Enabl.Pop());
                        WorL2.Push(WorL.Pop());
                    }
                    Vel.Clear();
                    Enabl.Clear();
                    WorL.Clear();
                    for (int i = 0; i < 5000; i++)
                    {
                        Vel.Push(Vel2.Pop());
                        Enabl.Push(Enabl2.Pop());
                        WorL.Push(WorL2.Pop());
                    }
                }
                //
                Vel.Push(velocity);
                Enabl.Push(enabled);
                WorL.Push(worldLocation);
            }

        }
        #endregion



    }
}
