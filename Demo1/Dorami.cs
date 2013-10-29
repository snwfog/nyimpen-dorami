using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  public class Dorami : NonPlayableCharacter, IStatefulCharacter
  {
    private static Texture2D savedTexture2D;
    private int savedAnimationInterval;
    private int saveFrame;
    private int saveFrameX;
    public bool IsSaved { get; set; }

    private int totalHealth;
    public int Health { get; set; }
    public Dorami(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesY, ref int[] lineSpriteAccToStatus) : base(level, texture, position, 1, nbMaxFramesY, ref lineSpriteAccToStatus, false)
    {
      this.Health = nbMaxFramesY;
      this.totalHealth = Health;
      this.update_interval = 1000 * 10;  
      this.finalPosition = this.position;
      this.savedAnimationInterval = 1000;
      this.IsSaved = false;
      this.saveFrame = 0;
      savedTexture2D = level.Content.Load<Texture2D>("saved");
      this.saveFrameX = 6;
    }

    public int GetNumberOfStates()
    {
      return nbMaxFramesY;
    }

    public int GetCurrentState()
    {
      return Health;
    }

    public int GetStateUpdateInterval()
    {
      return this.update_interval;
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      update_timer += gameClock.ElapsedGameTime.Milliseconds;
      if (IsSaved)
      {
        if (update_timer >= savedAnimationInterval)
        {
          ++this.saveFrame;
          update_timer = 0;
          if (this.saveFrame >= this.saveFrameX)
            Environment.Exit(0);
        }
      }
      else if (update_timer >= update_interval)
      {
        if (Health > 0)
          Health--;
        else
          Health = nbMaxFramesY;

        update_timer = 0;

        Health = Health == 0 ? totalHealth : Health;
        //if (Health == 0)
        //  throw new GamerPrivilegeException();
        base.Update(gameClock, yard);
      }
    }
  
    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      if (IsSaved)
      {
        Rectangle sourceRect = new Rectangle(this.saveFrame * (int)sizeSprite.X, 0, (int)sizeSprite.X, (int)sizeSprite.Y);
        spriteBatch.Draw(savedTexture2D, finalPosition, sourceRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        spriteBatch.Draw(new Texture2D(gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);
        
      }
      else
      {
        int line = lineSpritesAccToStatus[(Health - 1)];
        Rectangle sourceRect = new Rectangle(0, (int)(line * sizeSprite.Y), (int)sizeSprite.X, (int)sizeSprite.Y);
        spriteBatch.Draw(texture, finalPosition, sourceRect, Color.White);
        spriteBatch.Draw(new Texture2D(gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);
      }
    }
  }
}
