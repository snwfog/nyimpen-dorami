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
    private int timeToLive = 6000; // 6 seconds

    public AirGun(Texture2D texture, Vector2 position, ref int[] lineSpriteAccToStatus)
      : base(texture, position, 1, 1, ref lineSpriteAccToStatus)
    {
      this.status = Status.IDLE;
      this.idleStatus = Status.N;
      this.finalPosition = this.position;
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
