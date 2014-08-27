using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Tile_Engine;
using ScreenSystemLibrary;
using System.Windows.Forms;

namespace Earth_Invaders
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        string[,] ScoreBoard = new string[10, 3];
        public static int LoadLevelNumber, LoadLevelScore, LoadHealth;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch, spriteBatch2, spriteBatch3;
        public static SpriteBatch spriteBatch1;
        //
        public static ScreenSystem screenSystem;
        //
        //
        public static Texture2D[] world_background;
        public static Texture2D[] Screens_background;
        public static Rectangle[] rectangle;
        //
        Player player;
        public static SpriteFont Buxton_Sketch24, Buxton_Sketch20;
        
        public enum GameState { Null, TitleScreen, StartGame, Pause, Playing, PlayerDead, GameOver, HelpScreen, Log_in, Sign_in, ScoreBoard };
        public static GameState gameState = GameState.Null;

        Vector2 Die = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/4, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2);

        Vector2 levelPosition = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width *.1f, 5);
        Vector2 scorePosition = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width *.4f, 5);
        Vector2 healthPosition = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width *.7f, 5);

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //
            world_background = new Texture2D[6];
            Screens_background = new Texture2D[6];
            rectangle = new Rectangle[6];
            screenSystem = new ScreenSystem(this);
            Components.Add(screenSystem);
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  
        /// </summary>
        protected override void Initialize()
        {
            //
            Form MyGameForm = (Form)Form.FromHandle(Window.Handle);
            MyGameForm.FormBorderStyle = FormBorderStyle.None;
            for (int i = 0; i < 3; i++)
            {
                Screens_background[i] = Content.Load<Texture2D>(@"Textures\Screens background\" + i);
            }
            rectangle[5] = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            rectangle[5].Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //
            // TODO: Add your initialization logic here

            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;



            this.graphics.ApplyChanges();
            //

            screenSystem.AddScreen(new FCISLogoScreen(Color.Black, 2.1f));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            TileMap.Initialize(
                Content.Load<Texture2D>(@"Textures\Sprites\PlatformTiles"));
            TileMap.spriteFont =
                Content.Load<SpriteFont>(@"Fonts\Buxton Sketch24");

            Buxton_Sketch24 = Content.Load<SpriteFont>(@"Fonts\Buxton Sketch24");
            Buxton_Sketch20 = Content.Load<SpriteFont>(@"Fonts\Buxton Sketch20");

            //
            spriteBatch1 = new SpriteBatch(GraphicsDevice);
            spriteBatch2 = new SpriteBatch(GraphicsDevice);
            spriteBatch3 = new SpriteBatch(GraphicsDevice);

            //



            Camera.WorldRectangle = new Rectangle(0, 0, 160 * 64, 12 * 64);
            Camera.Position = Vector2.Zero;
            Camera.ViewPortWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Camera.ViewPortHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            player = new Player(Content);

            LevelManager.Initialize(Content, player);


        }

        private void StartNewGame()
        {
            player._health = LoadHealth;
            player.Revive();
            player.WorldLocation = Vector2.Zero;
            player.Score = LoadLevelScore;
            LevelManager.LoadLevel(LoadLevelNumber);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameState == GameState.StartGame)
            {

                StartNewGame();
                gameState = GameState.Playing;
            }
            if (gameState == GameState.Pause)
            {
                gameState = GameState.TitleScreen;
            }
            if (gameState == GameState.Playing)
            {

                player.Update(gameTime);
                LevelManager.Update(gameTime);
                if (Player.Dead)
                {

                    player._health = 0;
                    gameState = GameState.PlayerDead;
                }
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">time elapsed from last call to this method</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend);


            if (gameState == GameState.TitleScreen)
            {
                spriteBatch3.Begin();
                spriteBatch3.Draw(Screens_background[0], rectangle[5], Color.White);
                spriteBatch3.End();
            }

            if (gameState == GameState.HelpScreen)
            {
                spriteBatch3.Begin();
                spriteBatch3.Draw(Screens_background[1], rectangle[5], Color.White);
                spriteBatch3.End();
            }

            

            if ((gameState == GameState.Playing) || (gameState == GameState.PlayerDead) || (gameState == GameState.GameOver))
            {
                //
                spriteBatch2.Begin();
                for (int i = 0; i < 5; i++)
                {
                    spriteBatch2.Draw(world_background[i], rectangle[i], Color.White);
                }
                spriteBatch2.End();
                //
                TileMap.Draw(spriteBatch);
                player.Draw(spriteBatch);
                LevelManager.Draw(spriteBatch);
                spriteBatch.DrawString(
                    Buxton_Sketch24,
                    "Score: " + player.Score.ToString(),
                    scorePosition,
                    Color.Red);
                spriteBatch.DrawString(
                    Buxton_Sketch24,
                    "Health: " + player._health.ToString(),
                    healthPosition,
                    Color.Red);
                spriteBatch.DrawString(
                    Buxton_Sketch24,
                    "Level: " + LevelManager.LevelName,
                    levelPosition,
                    Color.Red);
                if (Player.Dead)
                {
                    spriteBatch.DrawString(
                        Buxton_Sketch24,
                        "\"YOU DIED\" Press Shift to back to live again ;-)",
                        Die,
                        Color.Red);
                }

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
