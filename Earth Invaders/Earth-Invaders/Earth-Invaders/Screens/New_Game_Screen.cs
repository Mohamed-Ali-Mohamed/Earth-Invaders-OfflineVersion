using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Earth_Invaders
{
  
    public class New_Game_Screen : GameScreen
    {
        public override bool AcceptsInput
        {
            get { return true; }
        }

        

        float seconds;

        Color titleColor, descriptionColor;

        SpriteFont font;

        Vector2 position;

        InputSystem input;

        public override void Initialize()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);


            position = new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2) - 50, 200);

            input = ScreenSystem.InputSystem;
            input.NewAction("Pause", Keys.Escape);

            Entering += new TransitionEventHandler(PlayScreen_Entering);
        }

        void PlayScreen_Entering(object sender, TransitionEventArgs tea)
        {
            titleColor = Color.Green * TransitionPercent;
            descriptionColor = Color.White * TransitionPercent;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            font = content.Load<SpriteFont>("gamefont");
        }

        public override void UnloadContent()
        {
            font = null;
        }

        protected override void UpdateScreen(GameTime gameTime)
        {
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void HandleInput()
        {
            if (input.NewActionPress("Pause") && Game1.gameState != Game1.GameState.TitleScreen && Game1.gameState != Game1.GameState.HelpScreen)
            {

                Game1.gameState = Game1.GameState.TitleScreen;
                ScreenSystem.AddScreen(new PauseScreen(this));
            }
            else if (Game1.gameState != Game1.GameState.Pause && Game1.gameState != Game1.GameState.HelpScreen && Game1.gameState != Game1.GameState.TitleScreen && Game1.gameState != Game1.GameState.StartGame && Game1.gameState != Game1.GameState.Null)
            {
                Game1.gameState = Game1.GameState.Playing;
                
            }
            else if (Game1.gameState == Game1.GameState.Null)
            {
                ExitScreen();
                Game1.gameState = Game1.GameState.TitleScreen;
            }
        }

        protected override void DrawScreen(GameTime gameTime)
        {
            
        }
    }
}
