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
using Microsoft.Xna.Framework.Storage;
using System.Linq;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using OpenTK.Graphics.ES10;
using OpenTK.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using GamePad = Microsoft.Xna.Framework.Input.GamePad;

#endregion

namespace Assignment1
{
  public class FoodFightGame : Game
  {
    public bool DebugMode = false;

    private bool _isPaused; // Show dark backdrop and unpausable with P
    private bool _isFrozen; // Don't show dark backdrop and unpause with P

    public int MaxGameTime { get; set; }

    public bool _dialogueIsPlaying { get; set; }
    public List<DialogueBubble> AllDialogues; 

    public GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    public int MaxNumberOfBadGuysTM { get; set; }
    public List<SatsuiNoHadoDoraemon> BadGuysTM { get; set; }// TM for Trademark lol

    public int MaxNumberOfAirGuns { get; set; }
    public List<AirGun> AmmoRack { get; set; }

    public int MaxNumberOfPowerUp { get; set; }
    public List<PowerUp> PowerUps { get; set; }

    public int MaxNumberOfGlacierPit { get; set; }
    public List<GlacierPit> GlacierPits { get; set; } 

    public List<Projectile> TotalFlyingProjectiles { get; set; } 
    public Doraemon doraemon { get; set; }
    private ScoreBoard scoreBoard;
    private TimeBoard timeBoard;
    private AmmoBoard ammoBoard;

    private Dorami dorami;
    public StatefulUIPanel DoramiHealthPanel { get; set; }
    private Sprite2D yard;
    public Rectangle yardBound { get; set; }
    public Rectangle windowBound { get; set; }

    public const int GRID_SIZE = 32; // Which is also the standard size of all sprites...
    public const int REACTION_THRESHOLD = 300;
    private int reactionTimer;

    public AudioEngine engine { get; set; }
    public SoundBank soundBank { get; set; }
    public WaveBank waveBank { get; set; }
    public Cue mainLoop { get; set; }
    public Cue victoryCue { get; set; }

    public Texture2D BannerTexture { get; set; }
    public Texture2D DebugTexture { get; set; }
    public Texture2D TransparentDarkTexture { get; set; }

    public SpriteFont mono8 { get; set; }
    public SpriteFont mono12 { get; set; }

    public int SCORE_PENALTY_MULTIPLIER = 1;

    public FoodFightGame()
    {
      graphics = new GraphicsDeviceManager(this);
      //  changing the back buffer size changes the window size (when in windowed mode)
      graphics.PreferredBackBufferWidth = 32 * (16 + 0);
      graphics.PreferredBackBufferHeight = 32 * (9 + 1);

      windowBound = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
      yardBound = new Rectangle(GRID_SIZE, GRID_SIZE, graphics.PreferredBackBufferWidth - 2 * GRID_SIZE, graphics.PreferredBackBufferHeight - 3 * GRID_SIZE);

      Content.RootDirectory = "Content";

      MaxNumberOfAirGuns = 8;
      MaxNumberOfBadGuysTM = 4;
      MaxNumberOfPowerUp = 7;
      MaxNumberOfGlacierPit = 5;

      BadGuysTM = new List<SatsuiNoHadoDoraemon>();
      AmmoRack = new List<AirGun>();
      PowerUps = new List<PowerUp>();
      TotalFlyingProjectiles = new List<Projectile>();
      GlacierPits = new List<GlacierPit>();
      AllDialogues = new List<DialogueBubble>();

      if (this.DebugMode)
        MaxGameTime = 5000; // 5 seconds debug mode
      else
        MaxGameTime = 5 * 1000 * 60; // 5 minutes normal play

      // I AM PAUSING GAME AT THE START HERE!!! LOL BAD CODE
      this.Pause();
    }

    protected override void Initialize()
    {
      base.Initialize();

      // Load the sound
      engine = new AudioEngine("Content\\doraemon.xgs");
      soundBank = new SoundBank(engine, "Content\\sound-bank.xsb");
      waveBank = new WaveBank(engine, "Content\\wave-bank.xwb");
      mainLoop = soundBank.GetCue("sound-cut-original");
      mainLoop.Play();

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
      scoreBoard = ScoreBoard.GetNewInstance(doraemon);
      timeBoard = TimeBoard.GetNewInstance(this);
      ammoBoard = AmmoBoard.GetNewInstance(this);


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
      DoramiHealthPanel = new StatefulUIPanel(this, doramiUIPanelTexture, doramiUIPanelPosition, 6, ref doramiLineSprite, dorami);


      // Create SatsuiNoHadoDoraemon
      Texture2D hadoDoraemonTexture = Content.Load<Texture2D>("hado-doraemon-walk");
      Rectangle mobSpawnBound = new Rectangle(yardBound.Left + GRID_SIZE * 3, yardBound.Top * 1, yardBound.Width - GRID_SIZE * 6, yardBound.Height - GRID_SIZE * 2);
      // Load texture for the projectile sprite
      Texture2D projectileTexture2D = Content.Load<Texture2D>("projectiles");
      // A list that keep track of all spawn points
      List<Rectangle> allHadoDoraHitBox = new List<Rectangle>();
      int[] hadoDoraemonLineSprite = new int[8];
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.N] = 0;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.NE] = 1;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.E] = 2;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.SE] = 3;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.S] = 4;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.SW] = 5;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.W] = 6;
      hadoDoraemonLineSprite[(int)AnimatedSprite.Status.NW] = 7;
      // Create Satsui No Hado Doraemon
      for (int i = 1; i <= MaxNumberOfBadGuysTM; i++)
      {
        int x = AnimatedSprite.rand.Next(mobSpawnBound.Left, mobSpawnBound.Right);
        int y = AnimatedSprite.rand.Next(mobSpawnBound.Top, mobSpawnBound.Bottom);
        Vector2 hadoDoraemonInitialPosition = new Vector2(x, y);
        SatsuiNoHadoDoraemon hadoDoraemon = new SatsuiNoHadoDoraemon(this, hadoDoraemonTexture, hadoDoraemonInitialPosition, 6, 8, ref hadoDoraemonLineSprite, true);
        hadoDoraemonInitialPosition = Helper.GetNoOverLappingSpawnLocation(allHadoDoraHitBox, hadoDoraemon, mobSpawnBound);
        hadoDoraemon.position = hadoDoraemonInitialPosition;
        
        BadGuysTM.Add(hadoDoraemon);
        allHadoDoraHitBox.Add(hadoDoraemon.GetHitBoxAsRectangle());
      }


      // Create Air gun for pickup on the floor
      for (int i = 0; i < MaxNumberOfAirGuns; i++)
        AmmoRack.Add(AirGun.GetNewInstance());

      // Create Power Ups for pickup on the floor
      for (int i = 0; i < MaxNumberOfPowerUp; i++)
        PowerUps.Add(PowerUp.GetNewInstance());

      // Create Glacier pits
      for (int i = 0; i < MaxNumberOfGlacierPit; i++)
        GlacierPits.Add(GlacierPit.GetNewInstance(this));

      // Load misc data
      _isPaused = true;

      // More plain texture
      BannerTexture = Content.Load<Texture2D>("banner");
      DebugTexture = new Texture2D(GraphicsDevice, 1, 1);
      DebugTexture.SetData(new Color[] {new Color(225, 0, 255, 90)});
      TransparentDarkTexture = new Texture2D(GraphicsDevice, 1, 1);
      TransparentDarkTexture.SetData(new Color[] {new Color(0, 0, 0, 200)});

      // Font
      mono8 = Content.Load<SpriteFont>("manaspace0");
      mono12 = Content.Load<SpriteFont>("manaspace12");

      // Play intro
      DialogueBubble doramiNeedsHelp1 = DialogueBubble.GetNewInstance(this, dorami.position, "Help me Doraemon!");
      AllDialogues.Add(doramiNeedsHelp1);
      doramiNeedsHelp1.Play();
      DialogueBubble doraemonIsComing1 = DialogueBubble.GetNewInstance(this, doraemon.position, "Hold on Dorami!");
      AllDialogues.Add(doraemonIsComing1);
      doraemonIsComing1.Play();
    }

    public void Pause() { _isPaused = true; }
    public void Resume() { _isPaused = false; }
    public bool IsPaused() { return _isPaused; }
    public void Freeze() { _isFrozen = true; } 
    public void Unfreeze() { _isFrozen = false; } 
    public bool IsFrozen() { return this._isFrozen; }

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


      // Allow the player to pause the game, with a reaction timer 
      // so player cannot press it multiple time in a row
      reactionTimer += gameTime.ElapsedGameTime.Milliseconds;
      if (this.IsPaused() && reactionTimer >= REACTION_THRESHOLD)
      {
        // Listen for unpause key
        if (Keyboard.GetState().IsKeyDown(Key.P))
        {
          this.Resume();
          reactionTimer = 0;
        }
      }
      else if (!this.IsPaused() && reactionTimer >= REACTION_THRESHOLD)
      {
        if (Keyboard.GetState().IsKeyDown(Key.P))
        {
          this.Pause();
          reactionTimer = 0;
        }
      }

      // Let the dialgoue flow
      if (this._dialogueIsPlaying)
      {
        // Dialogue challenge (will play all dialogue before moving on)
        this._dialogueIsPlaying = false;
        foreach (DialogueBubble dialogue in AllDialogues)
        {
          if (dialogue.IsPlaying())
          {
            dialogue.Update(gameTime);
            this._dialogueIsPlaying = true;
            break; // Break, to not update all dialoge at the same time
          }
        }

        if (this._dialogueIsPlaying)
          return;
        else
          this.Resume();
      }

      if (this.IsPaused()) return;


      dorami.Update(gameTime, yardBound);
      if (dorami.CheckCollision(doraemon))
      {
        if (dorami.IsSaved)
          return;

        dorami.IsSaved = true;
        mainLoop.Pause();
        Cue victory = soundBank.GetCue("sound-musicbox");
        victory.Play();
      }
      else
      {
        // Update power ups
        foreach (PowerUp powerUp in PowerUps)
        {
          powerUp.Update(gameTime, yardBound);
        }

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
              if (AnimatedSprite.rand.Next(0, 100) >= 90)
                AmmoRack[g] = AirGun.GetNewInstance();
            }
          }
        }

        doraemon.Update(gameTime, yardBound);
        scoreBoard.Update(gameTime, yardBound);
        timeBoard.Update(gameTime, yardBound);

        for (int p = 0; p < PowerUps.Count; p++)
        {
          PowerUp up = PowerUps[p];
          if (up.CheckCollision(doraemon))
          {
            doraemon.PickUpPowerUp(up);
            //if (AnimatedSprite.rand.Next(0, 100) >= 90)
            //  PowerUps[p] = PowerUp.GetNewInstance();
            continue;
          }
          else
          {
            up.Update(gameTime, yardBound);
            //if (up.IsExpired())
            //{
            //  if (AnimatedSprite.rand.Next(0, 100) >= 90)
            //    PowerUps[p] = PowerUp.GetNewInstance();
            //}
          }
        }

        foreach (Projectile bullet in TotalFlyingProjectiles)
        {
          if (bullet.IsInTrajectory(windowBound))
          {
            // Check if current bullet is hitting doraemon
            if (bullet.owner is SatsuiNoHadoDoraemon && bullet.CheckCollision(doraemon))
              doraemon.KnockOutBy((KnockableOpponent)bullet); // FIXME: This casting is dangerous
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
          {
            doraemon.KnockOutBy(badDoraemon);
          }
          badDoraemon.Update(gameTime, yardBound);
        }

        for (int i = 0; i < GlacierPits.Count; i++)
        {
          GlacierPit pit = GlacierPits[i];
          if (!doraemon.IsKnockOut() && pit.CheckCollision(doraemon))
          {
            doraemon.KnockOutBy(pit);
            GlacierPits[i] = GlacierPit.GetNewInstance(this);
            continue;
          }
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
      DoramiHealthPanel.Draw(spriteBatch, Vector2.Zero);
      doraemon.Draw(spriteBatch, Vector2.Zero);
      scoreBoard.Draw(spriteBatch, Vector2.Zero);
      timeBoard.Draw(spriteBatch, Vector2.Zero);
      ammoBoard.Draw(spriteBatch);
      foreach (Projectile bullet in TotalFlyingProjectiles) bullet.Draw(spriteBatch, Vector2.Zero);
      foreach (AirGun gun in AmmoRack) gun.Draw(spriteBatch, Vector2.Zero);
      foreach (SatsuiNoHadoDoraemon badDoraemon in BadGuysTM) badDoraemon.Draw(spriteBatch, Vector2.Zero);
      foreach (PowerUp powerUp in PowerUps) powerUp.Draw(spriteBatch, Vector2.Zero);
      foreach (GlacierPit pit in GlacierPits) pit.Draw(spriteBatch);

      spriteBatch.End();

      if (_dialogueIsPlaying)
      {
        this.Pause();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        foreach (DialogueBubble dialogue in AllDialogues)
          if (dialogue.IsPlaying())
            dialogue.Draw(spriteBatch);
        spriteBatch.End();
      }
      else if (this.IsPaused())
      {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        spriteBatch.Draw(TransparentDarkTexture, windowBound, Color.White);
        spriteBatch.Draw(BannerTexture, new Vector2(windowBound.Center.X - 110, windowBound.Center.Y - 75), Color.White);
        spriteBatch.DrawString(mono12, "U(P)AUSE", new Vector2(windowBound.Center.X - 40, windowBound.Center.Y + 32), Color.White);
        spriteBatch.End();
      }
      base.Draw(gameTime);
    }
  }
}


