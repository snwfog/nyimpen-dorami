using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  public class Sprite2D
  {
    public Texture2D texture { get; set; }
    public Vector2 position { get; set; }
    public Vector2 size { get; set; }

    public Sprite2D(Texture2D newTexture, Vector2 newPosition, Vector2 newSize)
    {
      this.texture = newTexture;
      this.position = newPosition;
      this.size = newSize;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(texture, position, Color.White);
    }
  }
}
