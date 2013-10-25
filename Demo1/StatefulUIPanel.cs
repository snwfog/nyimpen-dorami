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
  public class StatefulUIPanel : AnimatedSprite
  {
    IStatefulCharacter link;
    public StatefulUIPanel(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesY, ref int[] lineSpriteAccToStatus, IStatefulCharacter reference) : base(level, texture, position, 1, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.link = reference;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      int line = lineSpritesAccToStatus[this.link.GetCurrentState() - 1];
      Rectangle sourceRect = new Rectangle(0, (int)(line * sizeSprite.Y), (int)sizeSprite.X, (int)sizeSprite.Y);
      spriteBatch.Draw(texture, finalPosition, sourceRect, Color.White);
    }
  }
}
