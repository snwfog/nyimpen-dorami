using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  public class SatsuiNoHadoDoraemon : NonPlayableCharacter, Shootable
  {
    private int MAX_FIRE_INTERVAL_IN_SECOND { get; set; }
    private int MIN_FIRE_INTERVAL_IN_SECOND { get; set; }
    private int fireInterval { get; set; }
    private int fireTimer { get; set; }
    static public Texture2D ammoTexture;
    public int AmmoCount { get; set; }
    public List<Projectile> ammoRack { get; set; }
    private Color tint { get; set; }
    public SatsuiNoHadoDoraemon(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus, bool isMovable) : base(level, texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus, isMovable)
    {
      this.velocity = 0.5f;
      MAX_FIRE_INTERVAL_IN_SECOND = 2;
      MIN_FIRE_INTERVAL_IN_SECOND = 2;
      fireInterval = 1000 * MAX_FIRE_INTERVAL_IN_SECOND;
      int r = rand.Next(0, 255);
      int g = rand.Next(0, 255);
      int b = rand.Next(0, 255);
      tint = new Color(r, g, b);
      status = Status.IDLE;
      idleStatus = Status.S;
      AmmoCount = 10000; // Essentially unlimited ammo
      ammoRack = new List<Projectile>();
    }

    public void ChargeAmmo(Projectile ammo)
    {
      if (this.ammoRack.Count < AmmoCount)
        this.ammoRack.Add(ammo);
    }

    public void Shoot()
    {
      // Don't shoot if ammo is empty or bad guy is idled
      if (AmmoCount == 0 || this.status == Status.IDLE)
        return;

      if (ammoTexture == null)
        ammoTexture = gameLevel.Content.Load<Texture2D>("projectiles");

      Projectile silverBullet = new Projectile(gameLevel, ammoTexture, this.position, nbMaxFramesX, nbMaxFramesY, ref lineSpritesAccToStatus, this);
      ammoRack.Add(silverBullet);
      gameLevel.TotalFlyingProjectiles.Add(silverBullet);
      // Deprecated
      //silverBullet.Fire();
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {

      fireTimer += gameClock.ElapsedGameTime.Milliseconds;
      if (fireTimer >= fireInterval && this.status != Status.IDLE)
      {
        // Randomly choose another fire interval
        this.fireInterval = rand.Next(MIN_FIRE_INTERVAL_IN_SECOND * 1000, MAX_FIRE_INTERVAL_IN_SECOND * 1000);
        this.Shoot();
        fireTimer = 0;
      }

      // Check the interaction with the rest of the world objects in the game
      List<SatsuiNoHadoDoraemon> badGuysTM = this.gameLevel.BadGuysTM;
      foreach (SatsuiNoHadoDoraemon badDora in badGuysTM)
      {
        if (!badDora.Equals(this) && badDora.WillCollideWith(this))
        {
          if (this.status != Status.IDLE)
            this.DeflectDirection();
          if (badDora.status != Status.IDLE)
            badDora.DeflectDirection();
        }
      }
      base.Update(gameClock, yard);
    }

    public new void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      Rectangle sourceRect;
      if (status != Status.IDLE)
      {
        int line = lineSpritesAccToStatus[(int) status];
        int currentFrameX = line == 0 ? currentFrame : currentFrame % nbMaxFramesX;
        sourceRect = new Rectangle(currentFrameX * (int)sizeSprite.X, line * (int)sizeSprite.Y, (int)sizeSprite.X, (int)sizeSprite.Y);
      }
      else
      {
        int line = lineSpritesAccToStatus[(int) idleStatus];
        sourceRect = new Rectangle(currentFrame * (int)sizeSprite.X,  line * (int)sizeSprite.Y, (int)sizeSprite.X, (int)sizeSprite.Y);
      }

      finalPosition = this.position;
      // spriteBatch.Draw(texture, finalPosition, sourceRect, this.tint);
      // We going to draw the z-index of this character based on his height
      // So the higher y-axis, the higher the index
      // Adds a sprite to a batch of sprites for rendering using the specified texture, destination rectangle, source rectangle, color, rotation, origin, effects and layer
      // spriteBatch.Draw(texture, finalPosition, sourceRect, Color.White);
      // Get the z-index from height
      //float zIndex = (this.position.Y + this.sizeSprite.Y) / (32 * 9);
      spriteBatch.Draw(texture, finalPosition, sourceRect, this.tint, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
      spriteBatch.Draw(new Texture2D(gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);
    }
  }
}
