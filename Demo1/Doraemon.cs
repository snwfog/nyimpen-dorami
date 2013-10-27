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
  class Doraemon : Character
  {
    private static Texture2D knockOutTexture2D;
    private int knockOutAnimationInterval;
    private bool isKnockOut;
    private int knockOutTimer;
    private int knockOutInterval;

    public Doraemon(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesX, int nbMaxFramesY, ref int[] lineSpriteAccToStatus): base(level, texture, position, nbMaxFramesX, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      knockOutAnimationInterval = 200; // This is longer than the regular sprite update
      isKnockOut = false;
      if (knockOutTexture2D == null)
        knockOutTexture2D = level.Content.Load<Texture2D>("knockout");
    }

    public bool IsKnockOut()
    {
      return isKnockOut;
    }

    public void KnockOut(int durationInSeconds)
    {
      if (isKnockOut)
        return;

      isKnockOut = true;
      knockOutInterval = durationInSeconds * 1000;
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      timer += gameClock.ElapsedGameTime.Milliseconds;
      knockOutTimer += gameClock.ElapsedGameTime.Milliseconds;
      if (IsKnockOut())
      {
        if (knockOutTimer >= knockOutInterval)
          isKnockOut = false;

        if (timer >= knockOutAnimationInterval)
        {
          timer = 0;
          ++currentFrame;
          currentFrame %= nbMaxFramesX;
        }
      }

      base.Update(gameClock, yard);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      base.Draw(spriteBatch, cameraPosition);
    }
  }
}
