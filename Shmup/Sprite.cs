using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shmup
{
	public class Sprite
	{
		public Texture2D spriteTexture;
		public Vector2 spritePos;

		public Sprite(Texture2D newTxr, Vector2 newPos)
		{
			spriteTexture = newTxr;
			spritePos = newPos;
		}

		public virtual void Update(GameTime gameTime, Point screenSize) { }

		public void Draw(SpriteBatch _spriteBatch)
		{
			_spriteBatch.Draw(
				spriteTexture,
				new Rectangle((int)spritePos.X, (int)spritePos.Y, spriteTexture.Width, spriteTexture.Height),
				Color.White
				);
		}
	}
}
