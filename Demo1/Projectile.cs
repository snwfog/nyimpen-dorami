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
  public class Projectile : AnimatedSprite, KnockableOpponent
  {
    public AnimatedSprite owner { get; set; }
    protected float velocity;

    private int _knockOutInterval;
    private int _knockOutPenalty;

    public Projectile(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFrameX, int nbMaxFramesY,
      ref int[] lineSpriteAccToStatus, AnimatedSprite owner) : base(level, texture, position, nbMaxFrameX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.velocity = 1.5f;

      this._knockOutPenalty = 2;
      this._knockOutInterval = 1000;

      this.owner = owner;
      this.tint = owner.tint;
      if (this.owner.status == Status.IDLE)
        this.status = this.owner.idleStatus;
      else
        this.status = this.owner.status;
      this.setHitBoxRectangleAndSize();
      this.isMovable = true;
    }

    public int KnockOutInterval() { return this._knockOutInterval; }
    public int KnockOutPenaltyScore() { return this._knockOutPenalty; }



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

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      int line = lineSpritesAccToStatus[(int) this.status];
      Rectangle sourceRectangle = new Rectangle(0, line * (int)sizeSprite.Y, (int)sizeSprite.X, (int)sizeSprite.Y);

      spriteBatch.Draw(texture, finalPosition, sourceRectangle, this.tint, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
      if (gameLevel.DebugMode)
        spriteBatch.Draw(new Texture2D(gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);
    }
  }
}
