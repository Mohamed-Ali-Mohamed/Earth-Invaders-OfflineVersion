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
namespace Earth_Invaders
{
    class Sound_Effect_Volume : Sound_Option
    {
        string _prevEntry, _nextEntry, _selectedEntry, _cancelMenu;
        public static float _fireSound = 0.3f;
        public static float _jumpSound = 0.3f;
        public static float _stoneSound = 0.3f;

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
        MainMenuEntry _low, _meduim, _heigh;
        public Sound_Effect_Volume()
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
            _low = new MainMenuEntry(this, "Set to Low");
            _meduim = new MainMenuEntry(this, "Set to Meduim");
            _heigh = new MainMenuEntry(this, "Set to Heigh");
            back = new MainMenuEntry(this, "Back");

            //Set up the screen events
            Removing += new EventHandler(MainMenuRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            //Set up the entry events, and load a submenu.
            _low.Selected += new EventHandler(setLow);
            _meduim.Selected += new EventHandler(setMedium);
            _heigh.Selected += new EventHandler(setHeigh);
            back.Selected += new EventHandler(Back);

            //Finally, add all entries to the list
            MenuEntries.Add(_low);
            MenuEntries.Add(_meduim);
            MenuEntries.Add(_heigh);
            MenuEntries.Add(back);
        }
        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"menu");

            //Initialize is called before LoadContent, so if you want to 
            //use relative position with the line spacing like below,
            //you need to do it after load content and spritefont
            _low.SetPosition(new Vector2(((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / 2) - 50, 200), true);
            _meduim.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), _low, true);
            _heigh.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), _meduim, true);
            back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), _heigh, true);
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
        public void setLow(object sender, EventArgs e)
        {
            float volume = 0;
            float pitch = 0;
            float pan = 0;
            set_Low(volume, pitch, pan);
        }

        public float set_Low(float _volume, float _pitch, float _pan)
        {
            _fireSound = 0.3f;
            _jumpSound = 0.3f;
            _stoneSound = 0.3f;
            _volume = 0.3f;
            _pitch = 0.0f;
            _pan = 0.0f;
            return _volume + _pitch + _pan;
        }
        void setMedium(object sender, EventArgs e)
        {
            _fireSound = 0.6f;
            _jumpSound = 0.6f;
            _stoneSound = 0.6f;
            float volume = 0;
            float pitch = 0;
            float pan = 0;
            set_Medium(volume, pitch, pan);

        }
        public float set_Medium(float _volume, float _pitch, float _pan)
        {
            _volume = 0.6f;
            _pitch = 0.0f;
            _pan = 0.0f;
            return _volume + _pitch + _pan;
        }
        void setHeigh(object sender, EventArgs e)
        {
            float volume = 0;
            float pitch = 0;
            float pan = 0;
            set_Hiegh(volume, pitch, pan);
        }
        public float set_Hiegh(float _volume, float _pitch, float _pan)
        {
            _fireSound = 1.0f;
            _jumpSound = 1.0f;
            _stoneSound = 1.0f;
            _volume = 1.0f;
            _pitch = 0.0f;
            _pan = 0.0f;
            
            return _volume + _pitch + _pan;
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
