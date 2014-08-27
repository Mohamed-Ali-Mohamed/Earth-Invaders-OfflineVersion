using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Earth_Invaders
{
    public class PresentLogoScreen : LogoScreen
    {
        public Video vid;
        public VideoPlayer vidplayer;
        public Rectangle vidRectangel;
        public Texture2D vidTexture;
        public PresentLogoScreen()
            : base() { }

        public PresentLogoScreen(Color fadeColor, float fadePercent)
            : base(fadeColor, fadePercent) { }

        public override void Initialize()
        {
            ScreenTime = TimeSpan.FromSeconds(0);
            Removing += new EventHandler(RemovingScreen);

            vidplayer = new VideoPlayer();
            base.Initialize();
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            Texture = content.Load<Texture2D>("Textures\\Sprites\\present");

        }

        public override void UnloadContent()
        {
            Texture = null;
        }

        void RemovingScreen(object sender, EventArgs e)
        {


            //Screen to load when this one is done
            Game1.gameState = Game1.GameState.TitleScreen;
            ContentManager content = ScreenSystem.Content;

            ScreenSystem.AddScreen(new GameMenuScreen());
        }
    }
}
