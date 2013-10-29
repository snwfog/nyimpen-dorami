using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Assignment1
{
  abstract public class AnimatedSprite
  {
    static protected FoodFightGame gameLevel;
    public Color tint { get; set; }
    public static Random rand = new Random();
    protected Texture2D texture;
    public Vector2 position { get; set; }
    protected int nbMaxFramesX;
    protected int nbMaxFramesY;
    protected int currentFrame;
    protected float timer = 0;
    protected float interval = 100;
    protected Vector2 sizeSprite;
    protected bool IsStateChanged { get; set; }
    public enum Status { N, NE, E, SE, S, SW, W, NW, IDLE};
    public Status status { get; set; }
    public Status idleStatus { get; set; }
    protected bool isMovable { get; set; }
    protected int[] lineSpritesAccToStatus;
    public Vector2 finalPosition { get; set; }
    public Rectangle hitBox { get; set; }

    // Could pass the level as a ref
    public AnimatedSprite(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus)
    {
      this.tint = Color.White;
      gameLevel = level;
      this.texture = texture;
      this.position = position;
      this.finalPosition = position;
      this.nbMaxFramesX = nbMaxFramesX;
      this.nbMaxFramesY = nbMaxFramesY;
      this.lineSpritesAccToStatus = lineSpriteAccToStatus;
      this.sizeSprite = new Vector2(this.texture.Width / this.nbMaxFramesX, texture.Height / this.nbMaxFramesY);
      this.currentFrame = nbMaxFramesX * lineSpritesAccToStatus[(int) status];
    }

    public virtual Rectangle GetHitBoxAsRectangle(Vector2 newPosition)
    {
      return new Rectangle((int)(newPosition.X + this.hitBox.X), (int)(newPosition.Y + this.hitBox.Y), this.hitBox.Width, this.hitBox.Height);
    }

    public virtual Rectangle GetHitBoxAsRectangle()
    {
      return new Rectangle((int)(this.position.X + this.hitBox.X), (int)(this.position.Y + this.hitBox.Y), this.hitBox.Width, this.hitBox.Height);
    }

    public Vector2 GetDirection()
    {
      Vector2 newDirection = Vector2.Zero;
      switch (this.status)
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

      return newDirection;
    }


    public bool CheckCollision(AnimatedSprite sprite)
    {
      Rectangle thisHitBox = this.GetHitBoxAsRectangle();
      Rectangle otherHitBox = sprite.GetHitBoxAsRectangle();
      return thisHitBox.Intersects(otherHitBox);
    }

    public Vector2 GetLocalPosition(int viewportWidth)
    {
      return new Vector2(position.X % viewportWidth, position.Y);
    }

    public virtual void Update(GameTime gameClock, Rectangle yard)
    {
      timer += gameClock.ElapsedGameTime.Milliseconds;
      if (status == Status.IDLE && this.isMovable)
      {
        currentFrame = 0;
      }
      else if (timer >= interval && this.isMovable)
      {
        ++currentFrame;
        timer = 0;
        currentFrame %= nbMaxFramesX;
      }


      //if (currentFrame == nbMaxFramesX * (lineSpritesAccToStatus[(int) status] + 1))
      //  currentFrame = nbMaxFramesX*lineSpritesAccToStatus[(int) status];
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      Rectangle sourceRect;
      if (status != Status.IDLE)
      {
        int line = lineSpritesAccToStatus[(int) status];
        int currentFrameX = line == 0 ? currentFrame : currentFrame % nbMaxFramesX;
        sourceRect = new Rectangle(currentFrameX * (int)sizeSprite.X, line * (int)sizeSprite.Y, (int)sizeSprite.X, (int)sizeSprite.Y);
      }
      else
      {
        int line = lineSpritesAccToStatus[(int) idleStatus];
        sourceRect = new Rectangle(currentFrame * (int)sizeSprite.X,  line * (int)sizeSprite.Y, (int)sizeSprite.X, (int)sizeSprite.Y);
      }

      // We going to draw the z-index of this character based on his height
      // So the higher y-axis, the higher the index
      // Adds a sprite to a batch of sprites for rendering using the specified texture, destination rectangle, source rectangle, color, rotation, origin, effects and layer
      // spriteBatch.Draw(texture, finalPosition, sourceRect, Color.White);
      // Get the z-index from height
      // float zIndex = (this.position.Y + this.sizeSprite.Y) / (32 * 9);
      spriteBatch.Draw(texture, finalPosition, sourceRect, this.tint, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
      if (gameLevel.DebugMode)
        spriteBatch.Draw(new Texture2D(gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);

    }
  }
}
