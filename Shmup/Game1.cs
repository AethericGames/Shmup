using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// progression / timer



namespace Shmup
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Texture2D saucerTxr, missileTxr, backgroundTxr, particleTxr;
		Point screenSize = new Point(800, 450);
		float spawnCooldown = 2;
		float playTime = 0;
		Sprite backgroundSprite;
		PlayerSprite playerSprite;
		List<MissileSprite> missileList = new List<MissileSprite>();
		List<ParticleSprite> particleList = new List<ParticleSprite>();
		SpriteFont uiFont, bigFont;
		SoundEffect shipExplodeSnd, missileExplodeSnd;

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
			particleTxr = Content.Load<Texture2D>("particle");
			uiFont = Content.Load<SpriteFont>("UIFont");
			bigFont = Content.Load<SpriteFont>("BigFont");
			shipExplodeSnd = Content.Load<SoundEffect>("shipExplode");
			missileExplodeSnd = Content.Load<SoundEffect>("missileExplode");

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
			else if (playerSprite.playerLives > 0 && missileList.Count < (Math.Min(playTime, 120f) / 120f) * 8 + 2)
			{
				missileList.Add(new MissileSprite(
					missileTxr,
					new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height)),
					(Math.Min(playTime, 120f)/120f) * 20000f + 50f
					));
				spawnCooldown = (float)(rng.NextDouble() + 0.5);
			}

			if (playerSprite.playerLives > 0) 
			{
				playerSprite.Update(gameTime, screenSize);
				playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			
			foreach (MissileSprite missile in missileList)
			{
				missile.Update(gameTime, screenSize);
				
				if(playerSprite.playerLives > 0 && playerSprite.IsColliding(missile))
				{
					for (int i = 0; i < 16; i++)
						particleList.Add(new ParticleSprite(particleTxr,
							new Vector2(
								missile.spritePos.X + (missileTxr.Width / 2) - (particleTxr.Width / 2),
								missile.spritePos.Y + (missileTxr.Height / 2) - (particleTxr.Height / 2)
								)
							));
					
					missile.dead = true;
					playerSprite.playerLives--;
					missileExplodeSnd.Play();
					if(playerSprite.playerLives == 0)
					{
						for (int i = 0; i < 32; i++)
							particleList.Add(new ParticleSprite(particleTxr,
								new Vector2(
									playerSprite.spritePos.X + (saucerTxr.Width / 2) - (particleTxr.Width / 2),
									playerSprite.spritePos.Y + (saucerTxr.Height / 2) - (particleTxr.Height / 2)
									)
								));
						shipExplodeSnd.Play();
					}
				}
			}

			foreach (ParticleSprite particle in particleList) particle.Update(gameTime, screenSize);

			missileList.RemoveAll(missile => missile.dead);
			particleList.RemoveAll(particle => particle.currentLife <= 0);

			base.Update(gameTime);

			//Debug.WriteLine(missileList.Count);
		}

		protected override void Draw(GameTime gameTime)
		{
			_spriteBatch.Begin();

			backgroundSprite.Draw(_spriteBatch);
			if (playerSprite.playerLives > 0) playerSprite.Draw(_spriteBatch);
			foreach (MissileSprite missile in missileList) missile.Draw(_spriteBatch);
			foreach (ParticleSprite particle in particleList) particle.Draw(_spriteBatch);

			_spriteBatch.DrawString(
				uiFont,
				"Lives: " + playerSprite.playerLives,
				new Vector2(14,14),
				Color.Black
				);

			_spriteBatch.DrawString(
				uiFont,
				"Lives: " + playerSprite.playerLives,
				new Vector2(10, 10),
				Color.White
				);

			_spriteBatch.DrawString(
				uiFont,
				"Time: " + Math.Round(playTime),
				new Vector2(14, 44),
				Color.Black
				);

			_spriteBatch.DrawString(
				uiFont,
				"Time: " + Math.Round(playTime),
				new Vector2(10, 40),
				Color.White
				);

			if (playerSprite.playerLives <= 0)
			{
				Vector2 textSize = bigFont.MeasureString("GAME OVER");
				_spriteBatch.DrawString(
					bigFont,
					"GAME OVER",
					new Vector2((screenSize.X / 2) - (textSize.X / 2) + 8, (screenSize.Y / 2) - (textSize.Y / 2) + 8),
					Color.Black
					);

				_spriteBatch.DrawString(
					bigFont,
					"GAME OVER",
					new Vector2((screenSize.X / 2) - (textSize.X / 2), (screenSize.Y / 2) - (textSize.Y / 2)),
					Color.White
					);
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
