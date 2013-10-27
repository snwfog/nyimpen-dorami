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
  public class NonPlayableCharacter : AnimatedSprite
  {

    protected int update_interval { get; set; }
    protected int update_timer { get; set; }
    protected float velocity { get; set; }


    public NonPlayableCharacter(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus, bool isMovable) : base(level, texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.status = Status.IDLE;
      this.update_interval = 2000; // 2 seconds
      this.isMovable = isMovable;
      this.velocity = 5.0f;
      this.hitBox = new Rectangle(8, 20, 16, 11);
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      timer += gameClock.ElapsedGameTime.Milliseconds;
      update_timer += gameClock.ElapsedGameTime.Milliseconds;

      // Update this NPC's action
      if (update_timer >= update_interval && this.isMovable)
      {
        update_timer = 0;
        switch (AnimatedSprite.rand.Next(3))
        {
          case 0:
          case 1:
            // Stays Idle
            this.status = Status.IDLE;
            this.idleStatus = Status.S;
            break;
          case 2:
            // Move the NPC
            // Update the NPC with one of the 8 directions
            this.status = (Status)rand.Next(lineSpritesAccToStatus.Length - 1);
            break;
        }

      }

      if (this.isMovable && this.status != Status.IDLE)
      {
        if (timer >= interval)
        {
          ++currentFrame;
          timer = 0;
        }
        this.Move(gameClock, yard);
      }
      else
      {
        if (update_timer >= update_interval)
        {
          ++currentFrame;
          currentFrame %= nbMaxFramesX;
          update_timer = 0;
        }
      }

      base.Update(gameClock, yard);
    }


    public bool WillCollideWith(NonPlayableCharacter sprite)
    {
      Rectangle thisHitBox, otherHitBox;
      if (this.status != Status.IDLE)
      {
        Vector2 newPosition = this.position + Vector2.Multiply(this.GetDirection(), this.velocity);
        thisHitBox = this.GetHitBoxAsRectangle(newPosition);
      }
      else
      {
        thisHitBox = GetHitBoxAsRectangle();
      }

      if (sprite.status != Status.IDLE)
      {
        Vector2 newPosition = sprite.position + Vector2.Multiply(sprite.GetDirection(), sprite.velocity);
        otherHitBox = sprite.GetHitBoxAsRectangle(newPosition);
      }
      else
      {
        otherHitBox = sprite.GetHitBoxAsRectangle();
      }

      return thisHitBox.Intersects(otherHitBox) && otherHitBox.Intersects(thisHitBox);
      // return false;
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

        this.CheckBoundaryAndMove(newDirection, yard);
        //else if (status != Status.IDLE)
        //{
        //  this.currentFrame = 0;
        //  this.idleStatus = prevStatus;
        //  this.status = Status.IDLE;
        //}
        //if (this.status != Status.IDLE)
        //  this.position += Vector2.Multiply(this.direction, this.velocity);
      }
    }


    protected bool IsBounded(Vector2 newPosition, Rectangle bound)
    {
      return bound.Contains(this.GetHitBoxAsRectangle(newPosition));
    }

    private void CheckBoundaryAndMove(Vector2 newDirection, Rectangle yard)
    {
      Vector2 newPosition = this.position + Vector2.Multiply(newDirection, this.velocity);
      if (this.IsBounded(newPosition, yard))
        this.position += Vector2.Multiply(newDirection, this.velocity);
      else
      {
        // Change the status of the character
        this.DeflectDirection();
      }
    }

    /**
     * This method will deflect the character randomly based on the current
     * facing direction.
     */
    public void DeflectDirection()
    {
      if (this.status != Status.IDLE)
      {
        // Compute the deflection wheel, where 0 = North, going clockwise, NW = 7
        int currentDirectionAsInt = (int) this.status;
        // Proper deflection array because I am too lazy to do it object oritented
        int[,] deflectionTable = new int[,]
        {
          {2, 3, 4, 5, 6}, // 0
          {3, 4, 5, 6, 7}, // 1
          {4, 5, 6, 7, 0}, // 2
          {5, 6, 7, 0, 1}, // 3
          {6, 7, 0, 1, 2}, // 4
          {7, 0, 1, 2, 3}, // 5
          {0, 1, 2, 3, 4}, // 6
          {1, 2, 3, 4, 5}  // 7
        };

        int nextDirectionAsInt = deflectionTable[currentDirectionAsInt, rand.Next(0, 5)];
        this.status = (Status) nextDirectionAsInt;
      }
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      finalPosition = this.position;
      base.Draw(spriteBatch, cameraPosition);
    }
  }
}
