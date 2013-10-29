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
  public class AmmoBoard
  {

    private Vector2 _position;
    private static SpriteFont _mono12;
    private static Texture2D _ammoTexture2D;
    private FoodFightGame _gameLevel;
    private Doraemon _link;

    private AmmoBoard(FoodFightGame level, Doraemon link, Vector2 position)
    {
      this._gameLevel = level;
      this._position = position;
      this._link = link;
    }

    public static AmmoBoard GetNewInstance(FoodFightGame level)
    {
      // Load the font
      if (_mono12 == null)
        _mono12 = level.Content.Load<SpriteFont>("manaspace12");
      // Load the ammo
      if (_ammoTexture2D == null)
        _ammoTexture2D = level.Content.Load<Texture2D>("projectiles");

      Rectangle windowBound = level.windowBound;
      Vector2 position = new Vector2(windowBound.Left + 350, windowBound.Bottom - 29);
      return new AmmoBoard(level, level.doraemon, position);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      // All these position here are hard coded
      spriteBatch.Draw(_ammoTexture2D, new Vector2(_position.X + 7, _position.Y + 7), new Rectangle(20, 0, 9, 15), Color.White);
      spriteBatch.DrawString(_mono12, _link.gun.Ammo + "x", new Vector2(_position.X - 20, _position.Y), Color.White);
    }
  }
}
