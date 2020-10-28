using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// simple collision detection
// player lives / death
// counter
// UI (font, imported)


namespace Shmup
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Texture2D saucerTxr, missileTxr, backgroundTxr;
		Point screenSize = new Point(800, 450);
		float spawnCooldown = 2;
		
		Sprite backgroundSprite;
		PlayerSprite playerSprite;
		List<MissileSprite> missiles = new List<MissileSprite>();
		SpriteFont uiFont;

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
			uiFont = Content.Load<SpriteFont>("UIFont");

			backgroundSprite = new Sprite(backgroundTxr, new Vector2());
			playerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X/6, screenSize.Y/2));
		}

		protected override void Update(GameTime gameTime)
		{
			Random rng = new Random();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (spawnCooldown > 0)
			{
				spawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			else if (missiles.Count < 5)
			{
				missiles.Add(new MissileSprite(
					missileTxr,
					new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height))
					));
				spawnCooldown = (float)(rng.NextDouble() + 0.5);
			}

			playerSprite.Update(gameTime, screenSize);
			foreach (MissileSprite missile in missiles) missile.Update(gameTime, screenSize);

			missiles.RemoveAll(missile => missile.dead);

			base.Update(gameTime);

			//Debug.WriteLine(missiles.Count);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();

			backgroundSprite.Draw(_spriteBatch);
			playerSprite.Draw(_spriteBatch);
			foreach(MissileSprite missile in missiles) missile.Draw(_spriteBatch);

			_spriteBatch.DrawString(uiFont, "this is a TEST!", new Vector2(10,10), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
