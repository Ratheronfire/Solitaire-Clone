using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text.RegularExpressions;

/// card graphics courtesy of https://code.google.com/p/vector-playing-cards/

namespace Solitaire
{
    public enum MouseButtons { Left, Right, Middle };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Solitaire : Microsoft.Xna.Framework.Game
    {
        public static float cardScale = 0.3f;
        public static Texture2D cardBack;

        MouseState previousMouseState, currentMouseState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Card> cards;
        Random rng;

        int width = 1280;
        int height = 720;

        int cardTextureWidth = 500;
        int cardTextureHeight = 726;

        int maxAttempts = 100;
        bool placingCards = false;

        Card clickedCard;

        public Solitaire()
        {
            graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            cards = new List<Card>();

            rng = new Random();

            previousMouseState = currentMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cardBack = Content.Load<Texture2D>("Cards\\back");

            foreach (string suit in new string[]{"hearts", "diamonds", "clubs", "spades"})
            {
                for (byte i = 1; i <= 13; i++ )
                {
                    cards.Add(new Card(Content.Load<Texture2D>("Cards\\" + i + "-" + suit), i, suit));
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !placingCards)
            {
                relocateCards();
            }

            if (placingCards && Keyboard.GetState().IsKeyUp(Keys.Space))
                placingCards = false;

            if (mouseButtonJustPressed(MouseButtons.Left))
            {
                Console.WriteLine("Left mouse pressed");
                clickedCard = clickOnCard();

                if (clickedCard != null)
                    Console.WriteLine(clickedCard.ToString());
            }
            else if (clickedCard != null && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                clickedCard.pos = new Vector2(currentMouseState.X - (clickedCard.getWidth() / 2), currentMouseState.Y - (clickedCard.getHeight() / 2));
            }

            if (mouseButtonJustPressed(MouseButtons.Right))
            {
                Card rightClickedCard = clickOnCard();

                if (rightClickedCard != null)
                    rightClickedCard.IsVisible = !rightClickedCard.IsVisible;
            }

            if (mouseButtonJustPressed(MouseButtons.Middle))
            {
                cardScale = ((float)rng.Next(10, 30)) / 100;
                foreach (Card card in cards)
                    card.scale = cardScale;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BurlyWood);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            foreach (var card in cards)
            {
                card.draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Card clickOnCard()
        {
            Rectangle cursorRegion = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            foreach (Card card in cards)
            {
                if (card.bounds.Intersects(cursorRegion))
                {
                    return card;
                }
            }

            return null;
        }

        protected void relocateCards()
        {
            placingCards = true;
            foreach (var card in cards)
            {
                bool foundSpot = false;
                int attempts = 0;

                while (!foundSpot)
                {
                    card.pos = new Vector2(rng.Next((int)(width - cardTextureWidth * cardScale)), rng.Next((int)(height - cardTextureHeight * cardScale)));
                    attempts++;

                    foreach (Card otherCard in cards.Where(otherCard => card != otherCard))
                    {
                        if (card.bounds.Intersects(otherCard.bounds) && attempts < maxAttempts)
                        {
                            foundSpot = false;
                        }
                        else
                        {
                            foundSpot = true;
                        }

                        if (attempts == maxAttempts)
                        {
                            Console.WriteLine(String.Format("{0} overlapped with {1} after {2} attempts", card.ToString(), otherCard.ToString(), attempts));
                        }
                    }
                }
            }

        }

        public bool mouseButtonJustPressed(MouseButtons button)
        {
            if (!IsActive)
                return false;

            bool ret = false;

            switch (button)
            {
                case MouseButtons.Left: ret = currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released; break;
                case MouseButtons.Right: ret = currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released; break;
                case MouseButtons.Middle: ret = currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released; break;
            }

            return ret;
        }

        public bool mouseButtonJustReleased(MouseButtons button)
        {
            if (!IsActive)
                return false;

            bool ret = false;

            switch (button)
            {
                case MouseButtons.Left: ret = currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed; break;
                case MouseButtons.Right: ret = currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed; break;
                case MouseButtons.Middle: ret = currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed; break;
            }

            return ret;
        }
    }
}
