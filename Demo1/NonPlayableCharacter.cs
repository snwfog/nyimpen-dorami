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
  class NonPlayableCharacter : AnimatedSprite
  {

    private int update_interval { get; set; }
    private int update_timer { get; set; }
    private float velocity { get; set; }


    public NonPlayableCharacter(Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus, bool isMovable) : base(texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.status = Status.IDLE;
      this.update_interval = 2000; // 2 seconds
      this.isMovable = isMovable;
      this.velocity = 0.6f;
      this.hitBoxSize = new Vector2(32, 16);
    }

    public override Rectangle GetHitBoxAsRectangle()
    {
      int x = (int)this.position.X;
      int y = (int) this.position.Y + 16; // Half height to give the isometric view effect
      int xSize = (int)hitBoxSize.X;
      int ySize = (int)hitBoxSize.Y;
      return new Rectangle(x, y, xSize, ySize);
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
      float hitBoxX = newPosition.X + sizeSprite.X - hitBoxSize.X;
      float hitBoxY = newPosition.Y + sizeSprite.Y - hitBoxSize.Y;

      Rectangle newBoundingBox = new Rectangle((int)hitBoxX, (int)hitBoxY, (int)hitBoxSize.X, (int)hitBoxSize.Y);
      return (bound.Contains(newBoundingBox));
    }

    private void CheckBoundaryAndMove(Vector2 newDirection, Rectangle yard)
    {
      Vector2 newPosition = this.position + Vector2.Multiply(newDirection, this.velocity);
      if (this.IsBounded(newPosition, yard))
        this.position += Vector2.Multiply(newDirection, this.velocity);
      else
      {
        // Change the status of the character
        this.status = this.DeflectDirection();
      }
    }

    /**
     * This method will deflect the character randomly based on the current
     * facing direction.
     */
    public Status DeflectDirection()
    {
      // Compute the deflection wheel, where 0 = North, going clockwise, NW = 7
      int currentDirectionAsInt = (int) currentDirection;
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
      return (Status) nextDirectionAsInt;
    }


    public new void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      finalPosition = this.position;
      base.Draw(spriteBatch, cameraPosition);
    }
  }
}
