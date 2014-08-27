using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Earth_Invaders
{
    public class PauseScreen : MenuScreen 
    {
        public static float _test = 1.0f;
        string prevEntry, nextEntry, selectedEntry, cancelMenu;
        public override string PreviousEntryActionName
        {
            get { return prevEntry; }
        }

        public override string NextEntryActionName
        {
            get { return nextEntry; }
        }

        public override string SelectedEntryActionName
        {
            get { return selectedEntry; }
        }

        public override string MenuCancelActionName
        {
            get { return cancelMenu; }
        }

        MainMenuEntry resume, option,back, quit;

        public PauseScreen(GameScreen parent)
        {
            //Set up the parent to resume when pause screen is done
            this.Parent = parent;

            //Set up the action names
            prevEntry = "MenuUp";
            nextEntry = "MenuDown";
            selectedEntry = "MenuAccept";
            cancelMenu = "MenuCancel";

            //Customize the text colors.
            Selected = Color.Yellow;
            Highlighted = Color.Green;
            Normal = Color.White;
        }

        public override void Initialize()
        {
            //Keys are already mapped from Menu Screen so we do not need to map
            //them again

            //Initialize the entries and set up the events

            resume = new MainMenuEntry(this, "Resume Game");
            resume.Selected += new EventHandler(ResumeSelect);
            option = new MainMenuEntry(this, "Option & Help");
            option.Selected += new EventHandler(OptionSelect);
            back = new MainMenuEntry(this, "Exit Game");
            back.Selected += new EventHandler(BackSelect);
            quit = new MainMenuEntry(this, "Quit Game");
            quit.Selected += new EventHandler(QuitSelect);

            //Finally, add all entries to the list
            MenuEntries.Add(resume);
            MenuEntries.Add(option);
            MenuEntries.Add(back);
            MenuEntries.Add(quit);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>("menu");
            //Set up positioning
            resume.SetPosition(new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2) - 50, 200), true);
            option.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), resume, true);
            back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), option, true);
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), back, true);
        }

        void ResumeSelect(object sender, EventArgs e)
        {
            Game1.gameState = Game1.GameState.Playing;
            MenuCancel();
        }
        void OptionSelect(object sender, EventArgs e)
        {
            Game1.gameState = Game1.GameState.TitleScreen;
            MenuCancel();

            ScreenSystem.AddScreen(new Option_Screen());
        }
        void BackSelect(object sender, EventArgs e)
        {
            _test = 0.3f;
            MenuCancel();
            Game1.gameState = Game1.GameState.Null;
            ScreenSystem.AddScreen(new GameMenuScreen());
        }
        void QuitSelect(object sender, EventArgs e)
        {
            ScreenSystem.Game.Exit();
        }
    }
}
