using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Earth_Invaders;
namespace Earth_Invaders
{
    class BackGround_sound : Sound_Option
    {
        string _prevEntry, _nextEntry, _selectedEntry, _cancelMenu;
        public override string PreviousEntryActionName
        {
            get { return _prevEntry; }
        }

        public override string NextEntryActionName
        {
            get { return _nextEntry; }
        }

        public override string SelectedEntryActionName
        {
            get { return _selectedEntry; }
        }

        public override string MenuCancelActionName
        {
            get { return _cancelMenu; }
        }
        MainMenuEntry _Normal, Medium, Hiegh;
          public BackGround_sound()
        {
            //Set up the action names
            _prevEntry = "MenuUp";
            _nextEntry = "MenuDown";
            _selectedEntry = "MenuAccept";
            _cancelMenu = "MenuCancel";

            //Allow transitions
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);

            //Customize the text colors.
            Selected = Color.Yellow;
            Highlighted = Color.Green;
            Normal = Color.White;
        }
          public override void Initialize()
          {
              //Get a reference to the input system
              InputSystem input = ScreenSystem.InputSystem;

              //Load the actions
              input.NewAction(PreviousEntryActionName, Keys.Up);
              input.NewAction(NextEntryActionName, Keys.Down);
              input.NewAction(SelectedEntryActionName, Keys.Enter);

              //Initialize the entries
              _Normal = new MainMenuEntry(this, "Set to Low");
              Medium = new MainMenuEntry(this, "Set to Meduim");
              Hiegh = new MainMenuEntry(this, "Set to Heigh");
              back = new MainMenuEntry(this, "Back");

              //Set up the screen events
              Removing += new EventHandler(MainMenuRemoving);
              Entering += new TransitionEventHandler(MainMenuScreen_Entering);
              Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

              //Set up the entry events, and load a submenu.
              _Normal.Selected += new EventHandler(setNormal);
              Medium.Selected += new EventHandler(setMedium);
              Hiegh.Selected += new EventHandler(setHiegh);
              back.Selected += new EventHandler(Back);

              //Finally, add all entries to the list
              MenuEntries.Add(_Normal);
              MenuEntries.Add(Medium);
              MenuEntries.Add(Hiegh);
              MenuEntries.Add(back);
          }
          public override void LoadContent()
          {
              ContentManager content = ScreenSystem.Content;
              SpriteFont = content.Load<SpriteFont>(@"menu");

              //Initialize is called before LoadContent, so if you want to 
              //use relative position with the line spacing like below,
              //you need to do it after load content and spritefont
              _Normal.SetPosition(new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2) - 50, 200), true);
              Medium.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5),_Normal, true);
              Hiegh.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5),Medium, true);
              back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5),Hiegh, true);
          }

          public override void UnloadContent()
          {
              SpriteFont = null;
          }

          void MainMenuScreen_Entering(object sender, TransitionEventArgs tea)
          {
              //Slide effect from left to right
              float effect = (float)Math.Pow(tea.percent - 1, 2) * -100;
              foreach (MenuEntry entry in MenuEntries)
              {
                  entry.Acceleration = new Vector2(effect, 0);
                  entry.Position = entry.InitialPosition + entry.Acceleration;
              }
          }
          void MainMenuScreen_Exiting(object sender, TransitionEventArgs tea)
          {
              //Slide effect from right to left
              float effect = (float)Math.Pow(tea.percent - 1, 2) * 100;
              foreach (MenuEntry entry in MenuEntries)
              {
                  entry.Acceleration = new Vector2(effect, 0);
                  entry.Position = entry.InitialPosition - entry.Acceleration;
              }
          }
          void setNormal(object sender, EventArgs e)
          {
            //  ContentManager content = ScreenSystem.Content;
              // BG = content.Load<Song>(@"Sound\intro");
              MediaPlayer.Volume = 0.3f;   
          }
          void setMedium(object sender, EventArgs e)
          {
              //ContentManager content = ScreenSystem.Content;
              // BG = content.Load<Song>(@"Sound\intro");
              MediaPlayer.Volume = 0.6f;
          }

          void setHiegh(object sender, EventArgs e)
          {
             // ContentManager content = ScreenSystem.Content;
              MediaPlayer.Volume = 1.0f;
          }
          void Back(object sender, EventArgs e)
          {
              ExitScreen();
              ScreenSystem.AddScreen(new Sound_Option());
          }
          void MainMenuRemoving(object sender, EventArgs e)
          {
              MenuEntries.Clear();
          }


         
    }
}
