#region Using Statements
using System;
using System.Collections.Generic;
using FoodFight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Linq;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
#endregion

namespace Demo1
{
  public class Game1 : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private int maxNumberOfBadGuysTM = 4;
    private List<SatsuiNoHadoDoraemon> badGuysTM; // TM for Trademark lol
    private int maxNumberOfAirGuns = 2;
    private List<AirGun> ammoRack;
    private Character doraemon;
    private NonPlayableCharacter dorami;
    private Sprite2D yard;
    private Rectangle yardBound { get; set; }
    private Rectangle windowBound { get; set; }
    public const int GRID_SIZE = 32; // Which is also the standard size of all sprites...

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      //  changing the back buffer size changes the window size (when in windowed mode)
      graphics.PreferredBackBufferWidth = 32 * (16 + 0);
      graphics.PreferredBackBufferHeight = 32 * (9 + 1);

      windowBound = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
      yardBound = new Rectangle(GRID_SIZE, GRID_SIZE, graphics.PreferredBackBufferWidth - 2 * GRID_SIZE, graphics.PreferredBackBufferHeight - 3 * GRID_SIZE);

      Content.RootDirectory = "Content";

      badGuysTM = new List<SatsuiNoHadoDoraemon>();
      ammoRack = new List<AirGun>();
    }

    protected override void Initialize()
    {
      base.Initialize();
      spriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);

      // Load the background image
      Texture2D yardTexture = Content.Load<Texture2D>("yard");
      yard = new Sprite2D(yardTexture, Vector2.Multiply(Vector2.Zero, new Vector2(32, 32)), new Vector2(this.graphics.PreferredBackBufferWidth - 32 * 2, this.graphics.PreferredBackBufferHeight - 32 * 2));

      // Create Doraemon character
      Texture2D doraemonTexture = Content.Load<Texture2D>("doraemon-walk-shadow");

      // Position characters position
      Vector2 doraemonInitialPosition = new Vector2(graphics.PreferredBackBufferWidth - 32 * 2, graphics.PreferredBackBufferHeight / 2 - 32 * 2);
      int[] doraemonLineSprite = new int[8];
      doraemonLineSprite[(int)AnimatedSprite.Status.N] = 0;
      doraemonLineSprite[(int)AnimatedSprite.Status.NE] = 1;
      doraemonLineSprite[(int)AnimatedSprite.Status.E] = 2;
      doraemonLineSprite[(int)AnimatedSprite.Status.SE] = 3;
      doraemonLineSprite[(int)AnimatedSprite.Status.S] = 4;
      doraemonLineSprite[(int)AnimatedSprite.Status.SW] = 5;
      doraemonLineSprite[(int)AnimatedSprite.Status.W] = 6;
      doraemonLineSprite[(int)AnimatedSprite.Status.NW] = 7;
      doraemon = new Character(doraemonTexture, doraemonInitialPosition, 6, 8, ref doraemonLineSprite);


      // Create Dorami character
      Texture2D doramiTexture = Content.Load<Texture2D>("dorami-shadow");
      Vector2 doramiInitialPosition = new Vector2(32, this.graphics.PreferredBackBufferHeight / 2 - 64);
      int[] doramiLineSprite = new int[8];
      doramiLineSprite[(int) AnimatedSprite.Status.S] = 0;
      dorami = new NonPlayableCharacter(doramiTexture, doramiInitialPosition, 6, 1, ref doramiLineSprite, false);

      // Create SatsuiNoHadoDoraemon
      Texture2D hadoDoraemonTexture = Content.Load<Texture2D>("hado-doraemon-walk");
      Rectangle mobSpawnBound = new Rectangle(yardBound.Left + GRID_SIZE * 2, yardBound.Top * 1, yardBound.Right - GRID_SIZE * 4, yardBound.Bottom - GRID_SIZE * 2);
      // Create Satsui No Hado Doraemon
      for (int i = 1; i <= maxNumberOfBadGuysTM; i++)
      {
        int x = AnimatedSprite.rand.Next(mobSpawnBound.Left, mobSpawnBound.Right);
        int y = AnimatedSprite.rand.Next(mobSpawnBound.Top, mobSpawnBound.Bottom);
        Vector2 hadoDoraemonInitialPosition = new Vector2(x, y);
        int[] hadoDoraemonLineSprite = new int[8];
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.N] = 0;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.NE] = 1;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.E] = 2;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.SE] = 3;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.S] = 4;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.SW] = 5;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.W] = 6;
        hadoDoraemonLineSprite[(int)AnimatedSprite.Status.NW] = 7;

        SatsuiNoHadoDoraemon hadoDoraemon = new SatsuiNoHadoDoraemon(hadoDoraemonTexture, hadoDoraemonInitialPosition, 6, 8, ref hadoDoraemonLineSprite, true);
        badGuysTM.Add(hadoDoraemon);
      }


      // Create Air gun
      Texture2D airGunTexture2D = Content.Load<Texture2D>("canon-on-the-ground");
      Rectangle airGunSpawnBound = new Rectangle(yardBound.Left + GRID_SIZE * 2, yardBound.Top * 1, yardBound.Right - GRID_SIZE * 4, yardBound.Bottom - GRID_SIZE * 2);
      // Give 2 chance of creating airgun, after this, it will be determined by the Update method
      for (int i = 1; i <= maxNumberOfAirGuns; i++)
        this.SpawnAirGun(airGunSpawnBound, airGunTexture2D);
    }

    private void SpawnAirGun(Rectangle bound, Texture2D airGunTexture)
    {
      // 50% of spawning a gun, while the rack is not yet full

      if (AnimatedSprite.rand.Next(50, 100) >= 50 && ammoRack.Count < maxNumberOfAirGuns)
      {
        int x = AnimatedSprite.rand.Next(bound.Left, bound.Right);
        int y = AnimatedSprite.rand.Next(bound.Top, bound.Bottom);
        Vector2 airGunSpawnPosition = new Vector2(x, y);
        int[] airGunLineSprite = new int[1];
        airGunLineSprite[(int)AnimatedSprite.Status.N] = 0;
        ammoRack.Add(new AirGun(airGunTexture, airGunSpawnPosition, ref airGunLineSprite));
      }
    }



    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // Load a 2D texture sprite
    }

    protected override void UnloadContent()
    {
      //  Free the previously alocated resources
      // mySprite1.texture.Dispose();
      spriteBatch.Dispose();
    }


    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        this.Exit();

      // Respawns the airgun if necessary
      foreach (AirGun gun in ammoRack)
      {
        if (gun.IsExpired())
        {
          if (AnimatedSprite.rand.Next(0, 100) >= 50)
          {
            Rectangle airGunSpawnBound = new Rectangle(yardBound.Left + GRID_SIZE * 2, yardBound.Top * 1, yardBound.Right - GRID_SIZE * 4, yardBound.Bottom - GRID_SIZE * 2);
            gun.ReRack(airGunSpawnBound);
          }
        }
      }

      if (ammoRack.Count < maxNumberOfAirGuns)
      {
        this.SpawnAirGun();
      }

      doraemon.Update(gameTime, yardBound);
      dorami.Update(gameTime, yardBound);

      foreach (SatsuiNoHadoDoraemon badDoraemon in badGuysTM)
        badDoraemon.Update(gameTime, yardBound);

      base.Update(gameTime);
    }


    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      graphics.GraphicsDevice.Clear(Color.Black);

      // Draw the sprite using Alpha Blend, which uses transparency information if avaliable
      spriteBatch.Begin();// (SpriteBlendMode.AlphaBlend);
      yard.Draw(spriteBatch);
      // mySprite1.Draw(spriteBatch);
      dorami.Draw(spriteBatch, Vector2.Zero);
      doraemon.Draw(spriteBatch, Vector2.Zero);
      foreach (AirGun gun in ammoRack)
        gun.Draw(spriteBatch, Vector2.Zero);

      foreach (SatsuiNoHadoDoraemon badDoraemon in badGuysTM)
        badDoraemon.Draw(spriteBatch, Vector2.Zero);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}


