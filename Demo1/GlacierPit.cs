using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics.ES10;

namespace FoodFight
{
  public class GlacierPit : KnockableOpponent
  {
    public static List<GlacierPit> AllGlacierPits;
    public static List<Rectangle> AllGlacierPitsHitBox; 
 
    private static Texture2D glacierPitTexture2D;
    private int _knockOutInterval;
    private int _knockOutPenalty;

    private Vector2 _position;
    private Rectangle _hitBox;

    private FoodFightGame _gameLevel;

    private GlacierPit(FoodFightGame level, Vector2 position)
    {
      this._gameLevel = level;
      this._knockOutInterval = 5000;
      this._knockOutPenalty = 3;

      this._hitBox = new Rectangle(6, 5, 20, 21);
      this._position = position;
    }

    public static GlacierPit GetNewInstance(FoodFightGame level)
    {
      if (glacierPitTexture2D == null)
        glacierPitTexture2D = level.Content.Load<Texture2D>("glacier-pit");
      if (AllGlacierPitsHitBox == null)
        AllGlacierPitsHitBox = new List<Rectangle>();

      int x = level.yardBound.Left + FoodFightGame.GRID_SIZE * 2;
      int y = level.yardBound.Top * 1;
      int width = level.yardBound.Right - FoodFightGame.GRID_SIZE * 4;
      int height = level.yardBound.Bottom - FoodFightGame.GRID_SIZE * 2;
      Rectangle spawnArea = new Rectangle(x, y, width, height);

      int positionX = AnimatedSprite.rand.Next(spawnArea.Left, spawnArea.Right);
      int positionY = AnimatedSprite.rand.Next(spawnArea.Top, spawnArea.Bottom);
      Vector2 spawnLocation = new Vector2(positionX, positionY);

      GlacierPit pit = new GlacierPit(level, spawnLocation);

      // Add them to the static list tracking
      spawnLocation = Helper.GetNoOverLappingSpawnLocation(AllGlacierPitsHitBox, pit.GetHitBoxAsRectangle(), spawnArea);

      // Modify the location
      pit._position = spawnLocation;

      // Add the pit into the rectangle list
      //AllGlacierPits.Add(pit);
      AllGlacierPitsHitBox.Add(pit.GetHitBoxAsRectangle());

      return pit;
    }

    public Rectangle GetHitBoxAsRectangle()
    {
      return new Rectangle((int)(this._position.X + this._hitBox.X), (int)(this._position.Y + this._hitBox.Y), this._hitBox.Width, this._hitBox.Height);
    }

    public int KnockOutInterval() { return this._knockOutInterval; }
    public int KnockOutPenaltyScore() { return this._knockOutPenalty; }

    public bool CheckCollision(AnimatedSprite sprite)
    {
      return this.GetHitBoxAsRectangle().Intersects(sprite.GetHitBoxAsRectangle());
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (this._gameLevel.DebugMode)
        spriteBatch.Draw(new Texture2D(_gameLevel.graphics.GraphicsDevice, 1, 1), this.GetHitBoxAsRectangle(), Color.White);
      spriteBatch.Draw(glacierPitTexture2D, _position, Color.White);
    }
  }


}
