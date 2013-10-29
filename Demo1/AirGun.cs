using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  public class AirGun : AnimatedSprite
  {
    private static Texture2D airGunTexture;
    public int Ammo { get; set; }
    public bool IsConsumed { get; set; }
    private const int MIN_TTL = 20 * 1000;
    private const int MAX_TTL = 40 * 1000;
    private int timeToLive = rand.Next(MIN_TTL, MAX_TTL);

    public int PickUpPlayCount { get; set; }
    public AirGun(FoodFightGame level, Texture2D texture, Vector2 position, ref int[] lineSpriteAccToStatus)
      : base(level, texture, position, 1, 1, ref lineSpriteAccToStatus)
    {
      this.Ammo = 3;
      this.status = Status.IDLE;
      this.idleStatus = Status.N;
      this.finalPosition = this.position;
      this.IsConsumed = false;
      this.PickUpPlayCount = 1;
    }

    public bool HasAmmo()
    {
      return Ammo > 0;
    }

    public void Fire()
    {
      if (Ammo > 0)
      {
        Ammo--;
        // Play shooting sound
        Cue shoot = gameLevel.soundBank.GetCue("sound-shoot");
        shoot.Play();
      }
      else
      {
        throw new Exception("Gun has no ammo, but was attempted to be fired.");
      }
    }

    public static AirGun GetNewInstance()
    {
      if (airGunTexture == null)
        airGunTexture = gameLevel.Content.Load<Texture2D>("canon-on-the-ground");

      int x = gameLevel.yardBound.Left + FoodFightGame.GRID_SIZE * 2;
      int y = gameLevel.yardBound.Top * 1;
      int width = gameLevel.yardBound.Right - FoodFightGame.GRID_SIZE * 4;
      int height = gameLevel.yardBound.Bottom - FoodFightGame.GRID_SIZE * 2;
      Rectangle spawnArea = new Rectangle(x, y, width, height);

      int positionX = AnimatedSprite.rand.Next(spawnArea.Left, spawnArea.Right);
      int positionY = AnimatedSprite.rand.Next(spawnArea.Top, spawnArea.Bottom);
      Vector2 spawnLocation = new Vector2(positionX, positionY);
      int[] airGunLineSprite = new int[1];
      airGunLineSprite[(int) AnimatedSprite.Status.N] = 0;

      return new AirGun(gameLevel, airGunTexture, spawnLocation, ref airGunLineSprite);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      if (!this.IsConsumed && !this.IsExpired())
      {
        Rectangle sourceRect = new Rectangle(0, 0, (int)sizeSprite.X, (int)sizeSprite.Y);
        spriteBatch.Draw(texture, this.position, sourceRect, Color.White);
      }
    }

    public bool IsExpired()
    {
      return this.timer >= this.timeToLive;
    }
  }
}
