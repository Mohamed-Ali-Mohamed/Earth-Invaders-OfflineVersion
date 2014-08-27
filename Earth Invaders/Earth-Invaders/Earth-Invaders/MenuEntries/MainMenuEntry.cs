using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;

//Example on how to use the ScreenSystem.  If you wish to use it in your own
//project, grab the ScreenSystem.dll found in the ScreenSystemImplementation
//folder and paste it in your project's folder.  Once that is done, right click
//References on your code project (not your content project) and click Add
//Reference.  Click the browse tab and look for the DLL.
namespace Earth_Invaders
{
    public class MainMenuEntry:MenuEntry
    {
        public MainMenuEntry(MenuScreen menu, string title)
            : base(menu, title)
        {

        }

        public override void AnimateHighlighted(GameTime gameTime)
        {
            //Code to pulsate the active entry
            float pulse = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3) + 1);
            Scale = 1 + pulse * 0.05f;


        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(InitialPosition.X, InitialPosition.Y);
        }
    }
}
