using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;

namespace FoodFight
{
  public class Helper
  {
    public static Vector2 GetNoOverLappingSpawnLocation(List<Rectangle> existingItems, AnimatedSprite newItem, Rectangle boundBox)
    {
      Rectangle newItemHitBox = newItem.GetHitBoxAsRectangle();

      for (int j = 0; j < existingItems.Count; j++)
      {
        if (existingItems[j].Intersects(newItemHitBox))
        {
          newItemHitBox.X = AnimatedSprite.rand.Next(boundBox.Left, boundBox.Right);
          newItemHitBox.Y = AnimatedSprite.rand.Next(boundBox.Top, boundBox.Bottom);
          j = 0;
        }
      }

      return new Vector2(newItemHitBox.X, newItemHitBox.Y);
    }
  }
}
