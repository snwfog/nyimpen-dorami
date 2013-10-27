using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  public class Projectile : AnimatedSprite
  {
    protected AnimatedSprite owner;
    protected float velocity;

    public Projectile(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFrameX, int nbMaxFramesY,
      ref int[] lineSpriteAccToStatus, AnimatedSprite owner) : base(level, texture, position, nbMaxFrameX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.velocity = 1.5f;
      this.owner = owner;
      this.status = owner.status;
      this.setHitBoxRectangleAndSize();
      this.isMovable = true;
    }

    public override Rectangle GetHitBoxAsRectangle()
    {
      return new Rectangle((int)(this.position.X + this.hitBox.X), (int)(this.position.Y + this.hitBox.Y), this.hitBox.Width, this.hitBox.Height);
    }

    /**
     * Compute the optimized hit box position based on
     * the facing of the projectile
     */
    private void setHitBoxRectangleAndSize()
    {
      Rectangle selectHitBox = new Rectangle(0, 0, 32, 32);
      switch(this.status)
      {
        case Status.NE:
          this.hitBox = new Rectangle(22, 15, 9, 9);
          break;
        case Status.SE:
          this.hitBox = new Rectangle(14, 21, 9, 10);
          break;
        case Status.SW:
          this.hitBox = new Rectangle(1, 18, 9, 9);
          break;
        case Status.NW:
          this.hitBox = new Rectangle(6, 11, 9, 9);
          break;
        case Status.N:
          this.hitBox = new Rectangle(20, 0, 9, 11);
          break;
        case Status.E:
          this.hitBox = new Rectangle(21, 20, 11, 9);
          break;
        case Status.S:
          this.hitBox = new Rectangle(4, 27, 9, 10);
          break;
        case Status.W:
          this.hitBox = new Rectangle(0, 18, 11, 9);
          break;
        default:
        break;
      }
    }

    public bool IsInTrajectory(Rectangle windowBound)
    {
      return windowBound.Intersects(this.GetHitBoxAsRectangle());
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      this.Move(gameClock, yard);
      base.Update(gameClock, yard);
    }

    public void Move(GameTime gameClock, Rectangle yard)
    {
      this.position += Vector2.Multiply(this.GetDirection(), this.velocity);
      this.finalPosition = this.position;
    }
  }
}
