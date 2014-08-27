using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Tile_Engine;
using Microsoft.Xna.Framework.Media;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework.Audio;
namespace Earth_Invaders
{
    /// <summary>
    /// player class
    /// defines the characteristics of the player 
    /// such that fall speed , score, health, move scale
    /// it also declares the stacks which are used to save player states for a period of time
    /// to enable him to go back in case of time reversed 
    /// </summary>
    public class Player : Creature
    {
        #region Declarations
        float _dieVolume = 1.0f;
        float pitch = 0.0f;
        float pan = 0.0f;
        private Vector2 fallSpeed = new Vector2(0, 20);
        private float moveScale = 299.9995f;
        private static bool dead = false;
        private int score = 0, scoreComplement=0;
        private bool firePressed = false;
        private ContentManager content;
        private Vector2 fireLocation;
        private Vector2 fireDirection;

        private int health = 100, healthComplement=100;

        public static Stack<string> Ani;
        public static Stack<Vector2> Vel;
        public static Stack<bool> Flip;
        public static Stack<Vector2> Worl;
        public static Stack<Vector2> PasWorl;
        private string newAnimation;
        public static bool Time_Reverse;
        private int counter;

        //
        // Sound
        SoundEffect fire;
        SoundEffect jump;
        //Health bar
        Texture2D mHealthBar;
        //
        Texture2D Key_E;
        //
        #endregion

        #region Properties
        // ///////////////////////////////////////
        public int _health
        {
            get 
            {
                if (health != ~healthComplement)
                    health = ~healthComplement;
                return health; 
            }
            set 
            { 
                health = value;
                healthComplement = ~value;
            }
        }
        /// /////////////////////////////////
        public static bool Dead
        {
            get { return dead; }
            set { dead = value; }
        }

        public int Score
        {
            get 
            {
                if (score != ~scoreComplement)
                    score = ~scoreComplement;
                return score; 
            }
            set 
            { 
                score = value;
                scoreComplement = ~value;
            }
        }


        #endregion


        #region Constructor
        /// <summary>
        /// constructs the player
        /// It loads the health bar image and all animation strips of the player
        /// and declare the stacks which records the player state to use it in time reverse when needed
        /// </summary>
        /// <param name="content">to load images of health bar and animations from disk into memory </param>
        public Player(ContentManager content)
        {
            Ani = new Stack<string>(50000);
            Vel = new Stack<Vector2>(50000);
            Flip = new Stack<bool>(50000);
            Worl = new Stack<Vector2>(50000);
            counter = -1;
            Time_Reverse = false;


            //////////////////////////////////////////////////////////////////////////
            //Health bar
            //Load the HealthBar image from the disk into the Texture2D object
            mHealthBar = content.Load<Texture2D>(@"Textures\Sprites\HealthBar6") as Texture2D;
            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            //Key E
            //Load the E image from the disk into the Texture2D object
            Key_E = content.Load<Texture2D>(@"Textures\Sprites\Key_E") as Texture2D;
            //////////////////////////////////////////////////////////////////////////


            this.content = content;
            animations.Add("idle",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Idle"),
                    75,//133
                    "idle"));
            animations["idle"].LoopAnimation = true;

            animations.Add("run",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Run"),
                    75,
                    "run"));
            animations["run"].LoopAnimation = true;

            animations.Add("run&up",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Run&Up"),
                    75,
                    "run&up"));
            animations["run&up"].LoopAnimation = true;

            animations.Add("up",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Up"),
                    75,
                    "up"));
            animations["up"].LoopAnimation = true;

            animations.Add("down",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Down"),
                    75,
                    "down"));
            animations["down"].LoopAnimation = true;

            animations.Add("jump",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Jump"),
                    75,
                    "jump"));
            animations["jump"].LoopAnimation = false;
            animations["jump"].FrameLength = 0.08f;
            animations["jump"].NextAnimation = "idle";

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Die"),
                     75,
                    "die"));
            animations["die"].LoopAnimation = false;

            frameWidth = 63;
            frameHeight = 79;
            CollisionRectangle = new Rectangle(18, 23, 23, 55);

            drawDepth = 0.825f;

            enabled = true;
            codeBasedBlocks = false;
            PlayAnimation("idle");
        }
        #endregion

        #region Public Methods
        // <summary>
        /// Update the player object 
        /// it plays the appropriate animation and changes player direction and velocity
        /// according to keys that are currently pressed
        /// it also handles player shooting and jumping
        /// it repositions the camera to be suitable to the position of the player
        /// </summary>
        /// <param name="gameTime">time elapsed from last call of this method</param>
        public void Update(GameTime gameTime)
        {
            newAnimation = "idle";
            velocity = new Vector2(0, velocity.Y);
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyState = Keyboard.GetState();

            if (Dead)
            {
                if (counter == -1 && keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift))
                {
                    Score -= 100;
                    Dead = false;
                    counter = 0;
                    _health = 100;
                }
            }
            if (!Dead && counter > -1)
            {
                _health = 100;
                counter++;
                Time_Reverse = true;
                if (Ani.Any())
                    newAnimation = Ani.Pop();

                if (Flip.Any())
                    flipped = Flip.Pop();
                if (Worl.Any())
                    worldLocation = Worl.Pop();
                if (Vel.Any())
                    velocity = Vel.Pop();
                if (counter >= 100 && onGround)
                {
                    _health = 100;
                    counter = -1;
                }
                PlayAnimation(newAnimation);
            }
            else if (!Dead && counter == -1)
            {
                Time_Reverse = false;
                if (keyState.IsKeyDown(Keys.A) && keyState.IsKeyDown(Keys.W) && !keyState.IsKeyDown(Keys.D))
                {
                    flipped = false;
                    newAnimation = "run&up";
                    velocity = new Vector2(-moveScale, velocity.Y);
                }
                else if (keyState.IsKeyDown(Keys.D) && keyState.IsKeyDown(Keys.W) && !keyState.IsKeyDown(Keys.A))
                {
                    flipped = true;
                    newAnimation = "run&up";
                    velocity = new Vector2(moveScale, velocity.Y);

                }
                else if (keyState.IsKeyDown(Keys.A) && !keyState.IsKeyDown(Keys.D))
                {
                    flipped = false;
                    newAnimation = "run";
                    velocity = new Vector2(-moveScale, velocity.Y);

                }

                else if (keyState.IsKeyDown(Keys.D) && !keyState.IsKeyDown(Keys.A))
                {
                    flipped = true;
                    newAnimation = "run";
                    velocity = new Vector2(moveScale, velocity.Y);

                }

                else if (keyState.IsKeyDown(Keys.W) && !keyState.IsKeyDown(Keys.D) && !keyState.IsKeyDown(Keys.A))
                {
                    newAnimation = "up";
                }

                else if (keyState.IsKeyDown(Keys.S) && !keyState.IsKeyDown(Keys.D) && !keyState.IsKeyDown(Keys.A))
                {
                    newAnimation = "down";
                }


                if (keyState.IsKeyDown(Keys.Space))
                {
                    if (onGround)
                    {
                        Jump();
                        newAnimation = "jump";
                    }
                }

                if (!firePressed && (keyState.IsKeyDown(Keys.LeftAlt) || keyState.IsKeyDown(Keys.RightAlt)))
                {


                    fireLocation = this.worldLocation;
                    if (newAnimation == "run&up")
                    {
                        if (!flipped)
                        {
                            fireDirection = new Vector2(-1, -1);
                            fireLocation.Y -= 15;
                            fireLocation.X += 5;
                        }
                        else
                        {
                            fireDirection = new Vector2(1, -1);
                            fireLocation.Y -= 15;
                            fireLocation.X -= 3;
                        }

                    }
                    else if (newAnimation == "up")
                    {
                        fireDirection = new Vector2(0, -1);
                        //to adjust the bullet position with respecto to the sela7 :D
                        if (flipped)
                        {
                            fireLocation.X -= 28f;
                            fireLocation.Y -= 30f;
                        }
                        else
                        {
                            fireLocation.X += 32f;
                            fireLocation.Y -= 30f;
                        }
                    }
                    else
                    {
                        fireDirection.Y = 0;
                        fireDirection.X = 0;
                        if (newAnimation == "down")
                        {
                            if (flipped)
                                fireDirection = new Vector2(1, 0);
                            else
                                fireDirection = new Vector2(-1, 0);
                            fireLocation.Y += 10f;
                        }
                        else if (!flipped)
                        {
                            fireDirection = new Vector2(-1, 0);
                        }
                        else if (flipped)
                        {
                            fireDirection = new Vector2(1, 0);
                        }

                    }

                    if (flipped)
                        fireLocation.X += 52;
                    if (fireDirection.X == 0 && fireDirection.Y == 1)
                    {
                        if (flipped)
                        {
                            fireLocation.X -= 26;
                        }
                        else
                        {
                            fireLocation.X += 26;
                        }
                    }
                    fireLocation.Y += 30;

                    fire = content.Load<SoundEffect>(@"Sound\Fire");
                    // Change Volume
                    float volume = Sound_Effect_Volume._fireSound;
                    float pitch = 0.0f;
                    float pan = 0.0f;
                    fire.Play(volume, pitch, pan);
                    //

                    LevelManager.fires.Add(new Fire(content, fireLocation, fireDirection, "PLAYER"));
                    firePressed = true;
                }

                if (firePressed && !keyState.IsKeyDown(Keys.LeftAlt) && !keyState.IsKeyDown(Keys.RightAlt))
                {
                    firePressed = false;
                }

                if (currentAnimation == "jump")
                    newAnimation = "jump";
                if (newAnimation == "down")
                {
                    CollisionRectangle = new Rectangle(18, 33, 23, 45);
                }
                else
                {
                    CollisionRectangle = new Rectangle(18, 25, 23, 53);
                }
                if (keyState.IsKeyDown(Keys.E))
                {
                    checkLevelTransition();
                }

                if (newAnimation != currentAnimation)
                {
                    PlayAnimation(newAnimation);
                }
                //
                if (Ani.Count() == 49999 || Vel.Count() == 49999 || Flip.Count() == 49999 || Worl.Count() == 49999)
                {
                    Stack<string> Ani2 = new Stack<string>(5000);
                    Stack<Vector2> Vel2 = new Stack<Vector2>(5000);
                    Stack<bool> Flip2 = new Stack<bool>(5000);
                    Stack<Vector2> Worl2 = new Stack<Vector2>(5000);
                    for (int i = 0; i < 5000; i++)
                    {
                        Ani2.Push(Ani.Pop());
                        Vel2.Push(Vel.Pop());
                        Flip2.Push(Flip.Pop());
                        Worl2.Push(Worl.Pop());
                    }
                    Ani.Clear();
                    Vel.Clear();
                    Flip.Clear();
                    Worl.Clear();
                    for (int i = 0; i < 5000; i++)
                    {
                        Ani.Push(Ani2.Pop());
                        Vel.Push(Vel2.Pop());
                        Flip.Push(Flip2.Pop());
                        Worl.Push(Worl2.Pop());
                    }
                }
                //
                Ani.Push(newAnimation);
                Vel.Push(velocity);
                Flip.Push(flipped);
                Worl.Push(worldLocation);
            }



            velocity += fallSpeed;

            repositionCamera();

            base.Update(gameTime);
        }

        /// <summary>
        /// Make the player jump by setting its velocity in y direction
        /// to -650 and plays the sound effect of jumping
        /// </summary>
        public void Jump()
        {
            velocity.Y = -650;
            jump = content.Load<SoundEffect>(@"Sound\Jump");
            float volume = Sound_Effect_Volume._jumpSound; ;
            float pitch = 0.0f;
            float pan = 0.0f;
            jump.Play(volume, pitch, pan);
        }

        /// <summary>
        /// kills the player 
        /// it plays the animation and the sound effect of dying
        /// and sets counter to -1 to allow time reverse
        /// </summary>
        public override void Kill()
        {
            PlayAnimation("die");
            velocity.X = 0;
            dead = true;
            counter = -1;
            SoundEffect die = content.Load<SoundEffect>(@"Sound\Die");
            die.Play(_dieVolume, pitch, pan);
        }

        /// <summary>
        /// detects the collision between the player and fires
        /// it decreases player's health according to the type of the 
        /// enemy which is shooted this fire
        /// it also removes the fire after hitting the player
        /// </summary>
        public override void CheckCollision()
        {
            for (int x = LevelManager.fires.Count - 1; x >= 0; x--)
            {
                if (this.CollisionRectangle.Intersects(LevelManager.fires[x].CollisionRectangle) && LevelManager.fires[x].source != "PLAYER")
                {

                    if (_health > 5 && LevelManager.fires[x].source == "ENEMY_D")
                        _health -= 5;
                    else if (_health > 2 && (LevelManager.fires[x].source == "ENEMY_F1" || LevelManager.fires[x].source == "ENEMY_F2"))
                        _health -= 2;
                    else if (_health > 1 && LevelManager.fires[x].source != "ENEMY_D" && LevelManager.fires[x].source != "ENEMY_F1" && LevelManager.fires[x].source != "ENEMY_F2")
                        _health--;
                    else
                        this.Kill();
                    LevelManager.fires.RemoveAt(x);
                }
            }
        }

        /// <summary>
        /// make the player alive after death (revive him)
        /// it plays the animation of idle and sets the value of "dead" field to true
        /// </summary>
        public void Revive()
        {
            PlayAnimation("idle");
            dead = false;
        }

        /// <summary>
        /// Draws the player character in the appropriate location in screen
        /// and draws the health bar of the player
        /// </summary>
        /// <param name="spriteBatch">the spriteSheet which is used in drawing health bar and player</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //Health bar
            int width = (int)((50) * (health / 100.0));
            //Draw the health for the health bar
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), width, 10)),
                new Rectangle(0, 45, 10, 10), Color.Red);
            //Draw the box around the health bar
            spriteBatch.Draw(mHealthBar, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X - 25, (int)(WorldCenter.Y - 50), 50, 16)),
                new Rectangle(0, 0, 50, 16), Color.White);
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //int screenLocX = (int)Camera.WorldTScreen(worldLocation).X;
            //Vector2 V = new Vector2(100, 100);
            //Game1.spriteBatch1.Begin();
            //Game1.spriteBatch1.DrawString(
            //           Game1.Buxton_Sketch24,
            //           worldLocation.X.ToString() + " & " + screenLocX.ToString(), V,
            //           Color.Red);
            //Game1.spriteBatch1.End();
            Vector2 centerCell = TileMap.GetCellByPixel(WorldCenter);
            if (TileMap.CellCodeValue(centerCell).StartsWith("T_"))
            {
                spriteBatch.Draw(Key_E, Camera.WorldToScreen(new Rectangle((int)WorldCenter.X -15 , (int)(WorldCenter.Y - 78), 28, 28)),
                new Rectangle(0, 0, 28, 28), Color.White);
            }
            base.Draw(spriteBatch);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// change the position of the camera
        /// </summary>
        private void repositionCamera()
        {
            int screenLocX = (int)Camera.WorldToScreen(worldLocation).X;
            int Meddel = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2);
            if (Meddel % 5 != 0)
            {
                for (int i = 1; i < 5; i++)
                {
                    if ( (Meddel+i) % 5 == 0 )
                    {
                        Meddel = Meddel + i;
                        break;
                    }
                    else if ( (Meddel - i) % 5 == 0)
                    {
                        Meddel = Meddel - i;
                        break;
                    }
                }
            }
            if (worldLocation.X < 10240 - Meddel)
            {
                Camera.Move(new Vector2(screenLocX - Meddel, 0));
            }
            if (screenLocX >= Meddel - 5 && screenLocX <= Meddel + 5 && worldLocation.X >= Meddel && worldLocation.X < 10240-Meddel)
            {
                for (int i = 0; i < 5; i++)
                {
                    Game1.rectangle[i].X += Meddel - screenLocX;
                }
            }

        }

        /// <summary>
        /// handles transitions between levels when the player get to the end of any level
        /// by checking the cell code value then using it to get to the next level
        /// </summary>
        private void checkLevelTransition()
        {
            Vector2 centerCell = TileMap.GetCellByPixel(WorldCenter);
            if (TileMap.CellCodeValue(centerCell).StartsWith("T_"))
            {

                string[] code = TileMap.CellCodeValue(centerCell).Split('_');

                if (code.Length != 4)
                    return;

                LevelManager.LoadLevel(int.Parse(code[1]));

                WorldLocation = new Vector2(
                    int.Parse(code[2]) * TileMap.TileWidth,
                    int.Parse(code[3]) * TileMap.TileHeight);

                LevelManager.RespawnLocation = WorldLocation;

                velocity = Vector2.Zero;
            }
        }

        #endregion


    }
}
