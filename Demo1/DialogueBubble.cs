using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FoodFight
{
  public class DialogueBubble : Sprite2D
  {
    public const int LINE_FEED = 20; // Determined empirically

    private static Texture2D _bubbleTexture;
    private static SpriteFont _mono8;
    private static Vector2 _textStartRelativeToBubbleBoundBox = new Vector2(12, 12);
    private static Vector2 _bubbleLeftTop = new Vector2(-130, -50);
    private static Vector2 _bubbleRightTop = new Vector2(-20, -50);

    private static Rectangle _bubbleSpriteRightBottom = new Rectangle(186, 0, 186, 60);
    private static Rectangle _bubbleSpriteRightTop = new Rectangle(186, 60, 186, 60);
    private static Rectangle _bubbleSpriteLeftBottom = new Rectangle(0, 0, 186, 60);
    private static Rectangle _bubbleSpriteLeftTop = new Rectangle(0, 60, 186, 60);
    private Rectangle _sourceRectangle;

    private enum XPositionRelativeToCenter { Left, Right }
    private enum YPositionRelativeToCenter { Top, Bottom }
    private XPositionRelativeToCenter _xPosition;
    private YPositionRelativeToCenter _yPosition;

    private string _message;
    private int _totalMessageRow;
    private Vector2 _rowOneStartPosition;
    private Vector2 _rowTwoStartPosition;
    private Cue _nextDialogueCue;
    

    private int _currentCharPosition;
    private int _currentRow;
    private int _currentRowCharPosition;

    private DialogueBubble(Texture2D texture, Vector2 position, string message, XPositionRelativeToCenter xPos, YPositionRelativeToCenter yPos) : base(texture, position, Vector2.One)
    {
      if (_bubbleTexture == null)
        _bubbleTexture = texture;
      this._message = message;
      this._xPosition = xPos;
      this._yPosition = yPos;

      if (this._xPosition == XPositionRelativeToCenter.Left)
        this._sourceRectangle = _bubbleSpriteRightTop;
      else
        this._sourceRectangle = _bubbleSpriteLeftTop;
    }

    public static DialogueBubble GetNewInstance(FoodFightGame level, Vector2 speakerPosition, string msg)
    {
      if (_bubbleTexture == null)
        _bubbleTexture = level.Content.Load<Texture2D>("speech");
      if (_mono8 == null)
        _mono8 = level.Content.Load<SpriteFont>("manaspace0");

      // We gonna place the bubble based on the player's position left or right of the screen middle
      XPositionRelativeToCenter xPos = speakerPosition.X - level.windowBound.Center.X >= 0
        ? XPositionRelativeToCenter.Right
        : XPositionRelativeToCenter.Left;
      YPositionRelativeToCenter yPos = speakerPosition.Y - level.windowBound.Center.Y >= 0
        ? YPositionRelativeToCenter.Bottom
        : YPositionRelativeToCenter.Top;

      Vector2 actualPositionBasedOnScreenLocation = Vector2.Zero;
      if (xPos == XPositionRelativeToCenter.Left)
        actualPositionBasedOnScreenLocation = Vector2.Add(speakerPosition, _bubbleRightTop);
      else
        actualPositionBasedOnScreenLocation = Vector2.Add(speakerPosition, _bubbleLeftTop);
      return new DialogueBubble(_bubbleTexture, actualPositionBasedOnScreenLocation, msg, xPos, yPos);
    }

    public bool IsPlaying()
    {
      return true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_bubbleTexture, this.position, this._sourceRectangle, Color.White);
      spriteBatch.DrawString(_mono8, _message, Vector2.Add(this.position, _textStartRelativeToBubbleBoundBox), Color.White);
    }
  }
}
