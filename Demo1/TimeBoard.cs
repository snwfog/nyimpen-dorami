using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FoodFight
{
  public class TimeBoard 
  {
    private Vector2 _position;
    private TimeSpan _elapsedGameTime;
    private static SpriteFont _mono12;
    private static Texture2D _clockTexture2D;
    private FoodFightGame _gameLevel;

    private TimeBoard(FoodFightGame level, Vector2 position)
    {
      this._gameLevel = level;
      this._position = position;
    }

    public static TimeBoard GetNewInstance(FoodFightGame level)
    {
      // Load the font
      if (_mono12 == null)
        _mono12 = level.Content.Load<SpriteFont>("manaspace12");
      // Load the clock
      if (_clockTexture2D == null)
        _clockTexture2D = level.Content.Load<Texture2D>("clock");

      // Generate the proper position
      Rectangle windowBound = level.windowBound;
      // These positional numbers are hardcoded
      Vector2 position = new Vector2(windowBound.Left + 210, windowBound.Bottom - 29);

      return new TimeBoard(level, position);
    }

    public void Update(GameTime gameClock, Rectangle yard)
    {
      if (!_gameLevel.doraemon.IsDead())
        _elapsedGameTime = gameClock.TotalGameTime;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      spriteBatch.DrawString(_mono12, _elapsedGameTime.ToString(@"mm\.ss\.fff"), this._position, Color.White);
      spriteBatch.Draw(_clockTexture2D, new Vector2(_position.X - 12, _position.Y + 5), Color.White); }
  }
}
