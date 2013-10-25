using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  class Projectile : AnimatedSprite
  {
    protected AnimatedSprite owner;
    protected Rectangle hitBox;
    protected float velocity;

    public Projectile(Texture2D texture, Vector2 position, int nbMaxFrameX, int nbMaxFramesY,
      ref int[] lineSpriteAccToStatus, AnimatedSprite owner) : base(texture, position, nbMaxFrameX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.velocity = 0.7f;

      if (owner.status != Status.IDLE)
      {
        this.status = owner.status;
        this.SetHitBox(this.status);
      }

    }

    /**
     * Compute the optimized hit box position based on
     * the facing of the projectile
     */
    public void SetHitBox(Status status)
    {
      switch(status)
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

    public override Rectangle GetHitBoxAsRectangle()
    {
      // This side effect function is not really cool and should be refactored
      this.SetHitBox(this.status);
      return this.hitBox;
    }

    public bool IsInTrajectory(Rectangle windowBound)
    {
      return windowBound.Intersects(this.GetHitBoxAsRectangle());
    }

    public void Move(GameTime gameClock, Rectangle yard)
    {
      // Randomly choose a new direction for this
      // non-playable character
      // If this NPC is movable, move him accordingly
      if (this.isMovable)
      {
        Vector2 newDirection = Vector2.Zero;
        //switch (rand.Next(lineSpritesAccToStatus.Length - 1))
        switch (status)
        {
          case Status.NE:
            // NE
            //this.status = Status.NE;
            newDirection = new Vector2(0.707f, -0.707f);
            break;
          case Status.SE:
            // SE
            //this.status = Status.SE;
            newDirection = new Vector2(0.707f, 0.707f);
            break;
          case Status.SW:
            // SW
            //this.status = Status.SW;
            newDirection = new Vector2(-0.707f, 0.707f);
            break;
          case Status.NW:
            // NW
            //this.status = Status.NW;
            newDirection = new Vector2(-0.707f, -0.707f);
            break;
          case Status.N:
            // N
            //this.status = Status.N;
            newDirection = new Vector2(0.0f, -1.0f);
            break;
          case Status.E:
            // E
            //this.status = Status.E;
            newDirection = new Vector2(1.0f, 0.0f);
            break;
          case Status.S:
            // S
            //this.status = Status.S;
            newDirection = new Vector2(0.0f, 1.0f);
            break;
          case Status.W:
            // W
            //this.status = Status.W;
            newDirection = new Vector2(-1.0f, 0.0f);
            break;
          default:
            break;
        }
        // if (status != Status.IDLE)
        // {
        //  this.currentFrame = 0;
        //  this.idleStatus = prevStatus;
        //  this.status = Status.IDLE;
        // }
        Vector2 newPosition = this.position + Vector2.Multiply(newDirection, this.velocity);
        if (this.status != Status.IDLE)
         this.position += newPosition;
      }
    }
  }
}
