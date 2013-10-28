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
  public class PowerUp : AnimatedSprite
  {
    private static Texture2D powerUpGrapeFruitTexture;
    private static Texture2D powerUpLettuceTexture;
    private static Texture2D powerUpPepperTexture;
    private static Texture2D powerUpRadishTexture;
    public enum PowerUpType { Grape, Lettuce, Pepper, Radish }

    public bool IsConsumed { get; set; }
    private const int MIN_TTL = 2000 * 1000; // Never expire
    private const int MAX_TTL = 2000 * 1000;
    private int timeToLive = rand.Next(MIN_TTL, MAX_TTL);
    public PowerUpType powerUpType { get; set; }
    public int point { get; set; }

    public PowerUp(FoodFightGame level, Texture2D texture, Vector2 position, PowerUpType powerUpType, int point, ref int[] lineSpriteAccToStatus) : base(level, texture, position, 4, 1, ref lineSpriteAccToStatus)
    {
      this.status = Status.IDLE;
      this.idleStatus = Status.N;
      this.finalPosition = this.position;
      this.IsConsumed = false;
      this.currentFrame = rand.Next(0, nbMaxFramesX);
      this.interval = 300;
      this.hitBox = new Rectangle(6, 8, 20, 19);
      this.powerUpType = powerUpType;
      this.point = point;
    }

    public static PowerUp GetNewInstance()
    {
      if (powerUpGrapeFruitTexture == null)
      {
        powerUpGrapeFruitTexture = gameLevel.Content.Load<Texture2D>("power-up-grape-fruit");
      }
      if (powerUpLettuceTexture == null)
      {
        powerUpLettuceTexture = gameLevel.Content.Load<Texture2D>("power-up-lettuce");
      }
      if (powerUpPepperTexture == null)
      {
        powerUpPepperTexture = gameLevel.Content.Load<Texture2D>("power-up-pepper");
      }
      if (powerUpRadishTexture == null)
      {
        powerUpRadishTexture = gameLevel.Content.Load<Texture2D>("power-up-radish");
      }

      int x = gameLevel.yardBound.Left + FoodFightGame.GRID_SIZE * 2;
      int y = gameLevel.yardBound.Top * 1;
      int width = gameLevel.yardBound.Right - FoodFightGame.GRID_SIZE * 4;
      int height = gameLevel.yardBound.Bottom - FoodFightGame.GRID_SIZE * 2;
      Rectangle spawnArea = new Rectangle(x, y, width, height);

      int positionX = AnimatedSprite.rand.Next(spawnArea.Left, spawnArea.Right);
      int positionY = AnimatedSprite.rand.Next(spawnArea.Top, spawnArea.Bottom);
      Vector2 spawnLocation = new Vector2(positionX, positionY);

      int[] powerUpLineSprite = new int[1];
      powerUpLineSprite[(int) AnimatedSprite.Status.N] = 0;

      Texture2D powerUpTexture2D;
      PowerUpType type;
      int point;
      switch (rand.Next(0, 4))
      {
        case 0:
          powerUpTexture2D = powerUpLettuceTexture;
          type = PowerUpType.Lettuce;
          point = 2;
          break;
        case 1:
          powerUpTexture2D = powerUpPepperTexture;
          type = PowerUpType.Pepper;
          point = 4;
          break;
        case 2:
          powerUpTexture2D = powerUpRadishTexture;
          type = PowerUpType.Radish;
          point = 5;
          break;
        default:
          powerUpTexture2D = powerUpGrapeFruitTexture;
          type = PowerUpType.Grape;
          point = 1;
          break;
      }

      return new PowerUp(gameLevel, powerUpTexture2D, spawnLocation, type, point, ref powerUpLineSprite);
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      timer += gameClock.ElapsedGameTime.Milliseconds;
      if (timer >= interval)
      {
        ++currentFrame;
        timer = 0;
        currentFrame %= nbMaxFramesX;
      }

    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      if (!this.IsConsumed && !this.IsExpired())
        base.Draw(spriteBatch, cameraPosition);
    }

    public bool IsExpired()
    {
      return this.timer >= this.timeToLive;
    }
  }
}
