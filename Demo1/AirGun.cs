using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Demo1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  class AirGun : AnimatedSprite
  {
    private bool IsConsumed { get; set; }
    private const int MIN_TTL = 20 * 1000;
    private const int MAX_TTL = 40 * 1000;
    private int timeToLive = rand.Next(MIN_TTL, MAX_TTL);

    public AirGun(Texture2D texture, Vector2 position, ref int[] lineSpriteAccToStatus)
      : base(texture, position, 1, 1, ref lineSpriteAccToStatus)
    {
      this.status = Status.IDLE;
      this.idleStatus = Status.N;
      this.finalPosition = this.position;
    }

    public void ReRack(Rectangle bound)
    {
      this.IsConsumed = false;
      this.timeToLive = rand.Next(MIN_TTL, MAX_TTL);
      this.timer = 0; // Essentially reset the timer
      // Respawn a new location
      int x = AnimatedSprite.rand.Next(bound.Left, bound.Right);
      int y = AnimatedSprite.rand.Next(bound.Top, bound.Bottom);
      this.position = new Vector2(x, y);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      if (!this.IsConsumed && !this.IsExpired())
      {
        Rectangle sourceRect = new Rectangle(0, 0, (int)sizeSprite.X, (int)sizeSprite.Y);
        spriteBatch.Draw(texture, this.position, sourceRect, Color.White);
      }
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      this.timer += gameClock.ElapsedGameTime.Milliseconds;
      base.Update(gameClock, yard);
    }

    public bool IsExpired()
    {
      return this.timer >= this.timeToLive;
    }
  }
}
