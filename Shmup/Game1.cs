using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


// Add a Missile class, as a child of Sprite class
// Use List<Missiles> to reference multiple missiles and iterate over them

namespace Shmup
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Texture2D saucerTxr, missileTxr, backgroundTxr;
		Point screenSize = new Point(800, 450);
		
		Sprite backgroundSprite;
		PlayerSprite playerSprite;
		List<MissileSprite> missiles = new List<MissileSprite>();

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			_graphics.PreferredBackBufferWidth = screenSize.X;
			_graphics.PreferredBackBufferHeight = screenSize.Y;
			_graphics.ApplyChanges();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			saucerTxr = Content.Load<Texture2D>("saucer");
			missileTxr = Content.Load<Texture2D>("missile");
			backgroundTxr = Content.Load<Texture2D>("background");

			backgroundSprite = new Sprite(backgroundTxr, new Vector2());
			playerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X/6, screenSize.Y/2));
		}

		protected override void Update(GameTime gameTime)
		{
			Random rng = new Random();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (missiles.Count < 5)
				missiles.Add(new MissileSprite(
					missileTxr, 
					new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height))
					));

			playerSprite.Update(gameTime, screenSize);
			foreach (MissileSprite missile in missiles) missile.Update(gameTime, screenSize);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();

			backgroundSprite.Draw(_spriteBatch);
			playerSprite.Draw(_spriteBatch);
			foreach(MissileSprite missile in missiles) missile.Draw(_spriteBatch);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
