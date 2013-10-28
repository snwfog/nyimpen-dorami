#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
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

namespace Assignment1
{
  public class FoodFightGame : Game
  {
    public GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private int maxNumberOfBadGuysTM = 10;
    public List<SatsuiNoHadoDoraemon> BadGuysTM { get; set; }// TM for Trademark lol
    private int maxNumberOfAirGuns = 2;
    public List<AirGun> AmmoRack { get; set; }
    public List<Projectile> TotalFlyingProjectiles { get; set; } 
    public Doraemon doraemon { get; set; }
    private Dorami dorami;
    private StatefulUIPanel doramiUIPanel; 
    private Sprite2D yard;
    public Rectangle yardBound { get; set; }
    public Rectangle windowBound { get; set; }
    public const int GRID_SIZE = 32; // Which is also the standard size of all sprites...

    public FoodFightGame()
    {
      graphics = new GraphicsDeviceManager(this);
      //  changing the back buffer size changes the window size (when in windowed mode)
      graphics.PreferredBackBufferWidth = 32 * (16 + 0);
      graphics.PreferredBackBufferHeight = 32 * (9 + 1);

      windowBound = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
      yardBound = new Rectangle(GRID_SIZE, GRID_SIZE, graphics.PreferredBackBufferWidth - 2 * GRID_SIZE, graphics.PreferredBackBufferHeight - 3 * GRID_SIZE);

      Content.RootDirectory = "Content";

      BadGuysTM = new List<SatsuiNoHadoDoraemon>();
      AmmoRack = new List<AirGun>();
      TotalFlyingProjectiles = new List<Projectile>();
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
      doraemon = new Doraemon(this, doraemonTexture, doraemonInitialPosition, 6, 8, ref doraemonLineSprite);


      // Create Dorami character
      Texture2D doramiTexture = Content.Load<Texture2D>("dorami");
      Vector2 doramiInitialPosition = new Vector2(32, this.graphics.PreferredBackBufferHeight / 2 - 64);
      int[] doramiLineSprite = new int[6];
      doramiLineSprite[0] = 5;
      doramiLineSprite[1] = 4;
      doramiLineSprite[2] = 3;
      doramiLineSprite[3] = 2;
      doramiLineSprite[4] = 1;
      doramiLineSprite[5] = 0;
      dorami = new Dorami(this, doramiTexture, doramiInitialPosition, 6, ref doramiLineSprite);
      // Create Dorami HP UI Panel
      Texture2D doramiUIPanelTexture = Content.Load<Texture2D>("dorami-health");
      int doramiUIPanelPositionX = windowBound.Left;
      int doramiUIPanelPositionY = windowBound.Bottom - GRID_SIZE;
      Vector2 doramiUIPanelPosition = new Vector2(doramiUIPanelPositionX, doramiUIPanelPositionY);
      doramiUIPanel = new StatefulUIPanel(this, doramiUIPanelTexture, doramiUIPanelPosition, 6, ref doramiLineSprite, dorami);


      // Create SatsuiNoHadoDoraemon
      Texture2D hadoDoraemonTexture = Content.Load<Texture2D>("hado-doraemon-walk");
      Rectangle mobSpawnBound = new Rectangle(yardBound.Left + GRID_SIZE * 3, yardBound.Top * 1, yardBound.Right - GRID_SIZE * 6, yardBound.Bottom - GRID_SIZE * 2);
      // Load texture for the projectile sprite
      Texture2D projectileTexture2D = Content.Load<Texture2D>("projectiles");
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

        SatsuiNoHadoDoraemon hadoDoraemon = new SatsuiNoHadoDoraemon(this, hadoDoraemonTexture, hadoDoraemonInitialPosition, 6, 8, ref hadoDoraemonLineSprite, true);
        for (int j = 0; j < BadGuysTM.Count; j++)
        {
          if (BadGuysTM[j].GetHitBoxAsRectangle().Intersects(hadoDoraemon.GetHitBoxAsRectangle()))
          {
            x = AnimatedSprite.rand.Next(mobSpawnBound.Left, mobSpawnBound.Right);
            y = AnimatedSprite.rand.Next(mobSpawnBound.Top, mobSpawnBound.Bottom);
            hadoDoraemonInitialPosition = new Vector2(x, y);
            hadoDoraemon.position = hadoDoraemonInitialPosition;
            j = 0;
          }
        }

        BadGuysTM.Add(hadoDoraemon);
      }


      // Create Air gun for pickup on the floor
      for (int i = 0; i < maxNumberOfAirGuns; i++)
        AmmoRack.Add(AirGun.GetNewInstance());
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

      dorami.Update(gameTime, yardBound);
      if (dorami.CheckCollision(doraemon))
        dorami.IsSaved = true;
      else
      {
        // Respawns the airgun if necessary
        for (int g = 0; g < AmmoRack.Count; g++)
        {
          AirGun gun = AmmoRack[g];
          if (gun.CheckCollision(doraemon))
          {
            doraemon.PickUpGun(gun);
            AmmoRack[g] = AirGun.GetNewInstance();
            continue;
          }
          else
          {
            gun.Update(gameTime, yardBound);
            if (gun.IsExpired())
            {
              // if (AnimatedSprite.rand.Next(0, 100) == 99)
              if (AnimatedSprite.rand.Next(0, 100) >= 50)
                AmmoRack[g] = AirGun.GetNewInstance();
            }
          }
        }

        doraemon.Update(gameTime, yardBound);

        foreach (Projectile bullet in TotalFlyingProjectiles)
        {
          if (bullet.IsInTrajectory(windowBound))
          {
            // Check if current bullet is hitting doraemon
            if (bullet.owner is SatsuiNoHadoDoraemon && bullet.CheckCollision(doraemon))
              doraemon.KnockOut(5000);
            else if (bullet.owner is Doraemon)
            {
              foreach (SatsuiNoHadoDoraemon badDora in BadGuysTM)
              {
                if (!badDora.IsKnockOut())
                {
                  if (badDora.CheckCollision(bullet))
                  {
                    badDora.KnockOut(10000);
                  }
                }
              }
            }

            bullet.Update(gameTime, windowBound);
          }
        }

        foreach (SatsuiNoHadoDoraemon badDoraemon in BadGuysTM)
        {
          if (!badDoraemon.IsKnockOut() && badDoraemon.CheckCollision(doraemon))
            doraemon.KnockOut(5000);
          badDoraemon.Update(gameTime, yardBound);
        }
        base.Update(gameTime);
      }
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
      doramiUIPanel.Draw(spriteBatch, Vector2.Zero);
      doraemon.Draw(spriteBatch, Vector2.Zero);
      foreach (Projectile bullet in TotalFlyingProjectiles) bullet.Draw(spriteBatch, Vector2.Zero);
      foreach (AirGun gun in AmmoRack) gun.Draw(spriteBatch, Vector2.Zero);
      foreach (SatsuiNoHadoDoraemon badDoraemon in BadGuysTM) badDoraemon.Draw(spriteBatch, Vector2.Zero);

      spriteBatch.End();
      base.Draw(gameTime);
    }
  }
}


