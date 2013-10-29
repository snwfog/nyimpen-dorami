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
  public class ScoreBoard : AnimatedSprite
  {
    private IStatefulCharacter link;
    public static SpriteFont mono12;
    public static Texture2D doraHeadTexture;
    private Vector2 _scorePosition; // This position will be updated depending number of digit
    private Vector2 _doraHeadPosition; // This will remain constant throughout the game play
    public const int CHAR_WIDTH = 10; // Char witdth including the spacing
    public const int SMALL_PADDING = 10; // A small padding between dora's head and the numerical score

    private int _scoreX;
    private int _scoreY;

    private ScoreBoard(FoodFightGame level, Texture2D texture, Vector2 position, int nbMaxFramesY, ref int[] lineSpriteAccToStatus, IStatefulCharacter reference) : base(level, texture, position, 1, nbMaxFramesY, ref lineSpriteAccToStatus)
    {
      this.link = reference;
      Rectangle windowBound = gameLevel.windowBound;
      _doraHeadPosition = new Vector2(windowBound.Right - 32, windowBound.Bottom - 32);
      this._scoreX = (int)_doraHeadPosition.X - SMALL_PADDING - CHAR_WIDTH * 1; // 1 represent 1 digit, which is 0
      this._scoreY = (int)_doraHeadPosition.Y + 3;
      _scorePosition = new Vector2(_scoreX, _doraHeadPosition.Y);
    }

    public static ScoreBoard GetNewInstance(IStatefulCharacter link)
    {
      // I am reusing the health sprite for doraemon's points
      if (doraHeadTexture == null)
        doraHeadTexture = gameLevel.Content.Load<Texture2D>("health");

      // Load the font
      if (mono12 == null)
        mono12 = gameLevel.Content.Load<SpriteFont>("manaspace12");


      // Initial position of the scoreboard is at the bottom right
      int[] dummy = new int[1];
      return new ScoreBoard(gameLevel, doraHeadTexture, Vector2.One, 1, ref dummy, link);
    }

    public override void Update(GameTime gameClock, Rectangle yard)
    {
      // Check score and plot the position based on the score digit
      int numberOfDigit = 1;
      int currentScore = link.GetCurrentState();
      int maxScoreThreshold = gameLevel.MaxNumberOfPowerUp * PowerUp.HIGHEST_POWER_UP_SCORE;

      // Scoreunit goes from 1, 10, 100, etc..
      for (int scoreUnit = 1, multiplier = 10; currentScore >= scoreUnit * multiplier; numberOfDigit++, scoreUnit *= multiplier)
      {
      }
      
      // Position of the score board will be relative to the head position
      _scoreX = (int)_doraHeadPosition.X - SMALL_PADDING - CHAR_WIDTH * numberOfDigit;
      _scorePosition = new Vector2(_scoreX, _scoreY);
      base.Update(gameClock, yard);
    }


    public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
      // Draw dora's head
      spriteBatch.DrawString(mono12, link.GetCurrentState().ToString(), _scorePosition, Color.White);
      spriteBatch.Draw(doraHeadTexture, _doraHeadPosition, new Rectangle(0, 0, 32, 32), Color.White);
    }
  }
}
