using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FoodFight
{
  public class Doraemon : Character, Shootable, IStatefulCharacter
  {
    private int _score;

    public static Texture2D ammoTexture;
    public List<Projectile> ammoRack { get; set; } 
    private int MAX_FIRE_INTERVAL { get; set; }
    private int MIN_FIRE_INTERVAL { get; set; }
    private int fireInterval { get; set; }
    private int fireTimer { get; set; }

    public static Texture2D doraemonWalkingWithoutGun { get; set; }
    public static Texture2D doraemonWalkingWithGun { get; set; }
    private int knockOutFrame;
    private static Texture2D knockOutTexture2D;
    private int knockOutAnimationInterval;
    private bool isKnockOut;
    private int knockOutTimer;
    private int knockOutInterval;
    public AirGun gun { get; set; }

    private bool _isDead;

    public Doraemon(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus): base(level, texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this._score = 20;
      this.velocity = 0.4f;

      this.tint = Color.White;
      MAX_FIRE_INTERVAL = 200;
      MIN_FIRE_INTERVAL = 200;
      fireInterval = MAX_FIRE_INTERVAL;

      knockOutFrame = 0;
      knockOutAnimationInterval = 1000; // This is longer than the regular sprite update
      isKnockOut = false;
      if (knockOutTexture2D == null)
        knockOutTexture2D = level.Content.Load<Texture2D>("knockout");
      ammoRack = new List<Projectile>();

      if (ammoTexture == null)
        ammoTexture = level.Content.Load<Texture2D>("projectiles");
      if (doraemonWalkingWithGun == null)
        doraemonWalkingWithGun = level.Content.Load<Texture2D>("doraemon-walk-with-gun");
      doraemonWalkingWithoutGun = texture;

      AirGun defaultGun = new AirGun(level, level.Content.Load<Texture2D>("canon-on-the-ground"), Vector2.Zero, ref lineSpriteAccToStatus);
      defaultGun.Ammo = 2;

      this.gun = defaultGun;
    }

    public int GetScore() { return this._score; }

    public int GetNumberOfStates()
    {
      return 0; // 0 == unlimited amount of state but its not really used here
    }

    public int GetStateUpdateInterval()
    {
      return 0; // 0 == no delay
    }

    public int GetCurrentState()
    {
      return _score;
    }

    public void PickUpGun(AirGun gun)
    {
      if (gun.IsConsumed)
        return;

      this.gun = gun;
      gun.IsConsumed = true; // This code is brittle, should use observer pattern
      // Play pickup cue
      gameLevel.soundBank.PlayCue("sound-pickup");
    }

    public void PickUpPowerUp(PowerUp up)
    {
      if (up.IsConsumed)
        return;

      this._score += up.point;
      up.IsConsumed = true;
      // Play pickup cue
      gameLevel.soundBank.PlayCue("sound-pickup");
    }

    public bool IsKnockOut()
    {
      return isKnockOut;
    }

    public void KnockOutBy(KnockableOpponent opponent)
    {
      if (isKnockOut)
        return;

      // gameLevel.mainLoop.Pause();
      // gameLevel.soundBank.PlayCue("sound-died");
      this.LoseScore(gameLevel.SCORE_PENALTY_MULTIPLIER * opponent.KnockOutPenaltyScore());
      gameLevel.soundBank.PlayCue("sound-bump");
      isKnockOut = true;
      knockOutInterval = opponent.KnockOutInterval();
    }

    public void Died()
    {
      this._isDead = true;
      gameLevel.mainLoop.Pause();
      gameLevel.soundBank.PlayCue("sound-died");
      //if (knockOutFrame == nbMaxFramesX - 1)
      //{
      //  Environment.Exit(0);
      //}
    }

    public bool IsDead() { return this._isDead; }

    public void LoseScore(int penalty)
    {
      this._score = (this._score - penalty) < 0 ? 0 : this._score - penalty;
      if (this._score <= 0)
        this.Died();
    }

    private bool HasGun()
    {
      return gun != null && gun.HasAmmo();
    }

    public void Shoot()
    {
      Projectile bullet = new Projectile(gameLevel, ammoTexture, this.position, nbMaxFramesX, nbMaxFramesY, ref lineSpritesAccToStatus, this);
      ammoRack.Add(bullet);
      gameLevel.TotalFlyingProjectiles.Add(bullet);
      gun.Fire();
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      fireTimer += gameClock.ElapsedGameTime.Milliseconds;

      if (this.HasGun())
      {
        texture = doraemonWalkingWithGun;
        if (fireTimer >= fireInterval && Keyboard.GetState().IsKeyDown(Keys.Space))
        {
          this.Shoot();
          fireTimer = 0;
        }
      }
      else
      {
        texture = doraemonWalkingWithoutGun;
      }

      timer += gameClock.ElapsedGameTime.Milliseconds;
      knockOutTimer += gameClock.ElapsedGameTime.Milliseconds;
      if (this._isDead)
      {
        if (timer >= knockOutAnimationInterval)
        {
          timer = 0;
          ++knockOutFrame;
          if (knockOutFrame >= nbMaxFramesX)
          {
            Environment.Exit(0);
          }
        }
        
      }
      else if (IsKnockOut())
      {
        //if (knockOutTimer >= knockOutInterval)
        //{
        //  isKnockOut = false;
        //}

        if (timer >= knockOutAnimationInterval)
        {
          timer = 0;
          ++knockOutFrame;
          if (knockOutFrame >= nbMaxFramesX)
          {
            if (_isDead)
            {
              
            }
            else
            {
              this.isKnockOut = false;
              knockOutFrame %= nbMaxFramesX;
            }
          }
        }
      }
      else      
        base.Update(gameClock, yard);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      if (IsKnockOut())
      {
        Rectangle sourceRect = new Rectangle(knockOutFrame * (int)sizeSprite.X, 0, (int)sizeSprite.X, (int)sizeSprite.Y);
        spriteBatch.Draw(knockOutTexture2D, finalPosition, sourceRect, this.tint, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

        if (gameLevel.DebugMode)
          spriteBatch.Draw(new Texture2D(gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);
      }
      else
        base.Draw(spriteBatch, cameraPosition);
    }
  }
}
