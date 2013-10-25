using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Demo1
{
  abstract class AnimatedSprite
  {
    public static Random rand = new Random();
    protected Texture2D texture;
    protected Vector2 position;
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
    public Vector2 finalPosition;
    public Vector2 hitBoxSize { get; set; }

    public AnimatedSprite(Texture2D texture, Vector2 position,
        int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus)
    {
      this.texture = texture;
      this.position = position;
      this.nbMaxFramesX = nbMaxFramesX;
      this.nbMaxFramesY = nbMaxFramesY;
      this.lineSpritesAccToStatus = lineSpriteAccToStatus;
      this.sizeSprite = new Vector2(this.texture.Width / this.nbMaxFramesX, texture.Height / this.nbMaxFramesY);
      this.currentFrame = nbMaxFramesX * lineSpritesAccToStatus[(int) status];
      this.hitBoxSize = this.sizeSprite;
    }

    public virtual Rectangle GetHitBoxAsRectangle()
    {
      int x = (int)this.position.X;
      int y = (int) this.position.Y;
      int xSize = (int)hitBoxSize.X;
      int ySize = (int)hitBoxSize.Y;
      return new Rectangle(x, y, xSize, ySize);
    }

    public bool CheckCollision(AnimatedSprite sprite)
    {
      return this.GetHitBoxAsRectangle().Intersects(sprite.GetHitBoxAsRectangle());
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
      float zIndex = (this.position.Y + this.sizeSprite.Y) / (32 * 9);
      spriteBatch.Draw(texture, sourceRect, null, Color.White, 0, finalPosition, SpriteEffects.None, zIndex);

    }
  }
}
