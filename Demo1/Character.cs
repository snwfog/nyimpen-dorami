using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FoodFight
{
  class Character : AnimatedSprite
  {
    private Vector2 direction { get; set; }
    private float velocity { get; set; }

    public Character(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus)
      : base(level, texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.status = Status.W;
      this.velocity = 2.6f;
      this.isMovable = true;
      this.hitBox = new Rectangle(8, 20, 16, 11);
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      this.Move(gameClock, yard);
      base.Update(gameClock, yard);
    }

    public void Move(GameTime gameClock, Rectangle yard)
    {
      KeyboardState ks = Keyboard.GetState();
      Status prevStatus = this.status;
      if (ks.IsKeyDown(Keys.W) && ks.IsKeyDown(Keys.D))
      {
        // NE
        this.status = Status.NE;
        this.direction = new Vector2(0.707f, -0.707f);

      }
      else if (ks.IsKeyDown(Keys.D) && ks.IsKeyDown(Keys.S))
      {
        // SE
        this.status = Status.SE;
        this.direction = new Vector2(0.707f, 0.707f);
      }
      else if (ks.IsKeyDown(Keys.S) && ks.IsKeyDown(Keys.A))
      {
        // SW
        this.status = Status.SW;
        this.direction = new Vector2(-0.707f, 0.707f);
      }
      else if (ks.IsKeyDown(Keys.A) && ks.IsKeyDown(Keys.W))
      {
        // NW
        this.status = Status.NW;
        this.direction = new Vector2(-0.707f, -0.707f);

      }
      else if (ks.IsKeyDown(Keys.W))
      {
        // N
        this.status = Status.N;
        this.direction = new Vector2(0.0f, -1.0f);
      }
      else if (ks.IsKeyDown(Keys.D))
      {
        // E
        this.status = Status.E;
        this.direction = new Vector2(1.0f, 0.0f);
      }
      else if (ks.IsKeyDown(Keys.S))
      {
        // S
        this.status = Status.S;
        this.direction = new Vector2(0.0f, 1.0f);
      }
      else if (ks.IsKeyDown(Keys.A))
      {
        // W
        this.status = Status.W;
        this.direction = new Vector2(-1.0f, 0.0f);
      }
      else if (status != Status.IDLE)
      {
        this.currentFrame = 0;
        this.idleStatus = prevStatus;
        this.status = Status.IDLE;
      }

      if (this.status != Status.IDLE)
      {
        Vector2 nextPosition = this.position + Vector2.Multiply(this.direction, this.velocity);
        // Compute a new bounding box for this character sprite
        if (this.IsBounded(nextPosition, yard))
          this.position += Vector2.Multiply(this.direction, this.velocity);
      }
    }

    protected bool IsBounded(Vector2 newPosition, Rectangle bound)
    {
      return bound.Contains(this.GetHitBoxAsRectangle(newPosition));
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      finalPosition = this.position;
      base.Draw(spriteBatch, cameraPosition);
    }
  }
}
