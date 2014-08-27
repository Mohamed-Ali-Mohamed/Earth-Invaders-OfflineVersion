using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Earth_Invaders
{
    public class FCISLogoScreen:LogoScreen
    {
        public FCISLogoScreen()
            : base() { }

        public FCISLogoScreen(Color fadeColor, float fadePercent) 
            : base(fadeColor, fadePercent) { }

        public override void Initialize()
        {
            ScreenTime = TimeSpan.FromSeconds(0);
            Removing += new EventHandler(RemovingScreen);
            base.Initialize();
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SoundEffect _first;
            _first = content.Load<SoundEffect>(@"Sound\First");
            _first.Play();
            Texture = content.Load<Texture2D>("Textures\\Sprites\\logo");
        }

        public override void UnloadContent()
        {
            Texture = null;
        }

        void RemovingScreen(object sender, EventArgs e)
        {
            //Screen to load when this one is done
            ScreenSystem.AddScreen(new PresentLogoScreen(Color.Black, 2.1f));
            
        }
    }
}
