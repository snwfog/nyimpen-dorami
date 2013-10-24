using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  class SatsuiNoHadoDoraemon : NonPlayableCharacter
  {
    private Color tint { get; set; }
    public SatsuiNoHadoDoraemon(Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY,
      ref int[] lineSpriteAccToStatus, bool isMovable)
      : base(texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus, isMovable)
    {
      int r = rand.Next(0, 255);
      int g = rand.Next(0, 255);
      int b = rand.Next(0, 255);
      tint = new Color(r, g, b);
      status = Status.IDLE;
      idleStatus = Status.S;
    }

    public new void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
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

      finalPosition = this.position;
      spriteBatch.Draw(texture, finalPosition, sourceRect, this.tint);
    }
  }
}
