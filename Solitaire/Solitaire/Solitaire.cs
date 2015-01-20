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
        public readonly static float cardScale = 0.3f;
        public static Texture2D cardBack, emptyPile;

        public readonly static int screenWidth = 1280;
        public readonly static int screenHeight = 720;

        public readonly static int cardTextureWidth = (int) (500 * cardScale);
        public readonly static int cardTextureHeight = (int) (726 * cardScale);

        public readonly static int cardOffset = 15;
        public readonly static int cardsPerHand = 3;

        public static readonly int gridWidth = screenWidth / 7;
        public static readonly int gridHeight = cardTextureHeight + 10;

        Field game;

        MouseState previousMouseState, currentMouseState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Card> cards;
        Random rng;

        Card clickedCard;
        Rectangle clickedCardStartBounds;

        public Solitaire()
        {
            graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

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
            emptyPile = Content.Load<Texture2D>("Cards\\empty");

            foreach (string suit in new string[]{"hearts", "diamonds", "clubs", "spades"})
            {
                for (byte i = 1; i <= 13; i++ )
                {
                    cards.Add(new Card(Content.Load<Texture2D>("Cards\\" + i + "-" + suit), i, suit));
                }
            }

            game = new Field(cards);
            Console.WriteLine(game.ToString());
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

            if (mouseButtonJustPressed(MouseButtons.Left))
            {
                Pile currentPile = game.findCursorPile(currentMouseState);

                if (currentPile is Hand || currentPile is FieldPile || currentPile is GoalPile)
                    clickedCard = game.findCardUnderCursor(currentMouseState, currentPile);

                if (clickedCard != null)
                    clickedCardStartBounds = clickedCard.bounds;
            }
            else if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (clickedCard != null && clickedCard.IsVisible)
                    clickedCard.bounds = new Rectangle(currentMouseState.X - (cardTextureWidth / 2), currentMouseState.Y - (cardTextureHeight / 2), cardTextureWidth, cardTextureHeight);
            }
            else if (mouseButtonJustReleased(MouseButtons.Left))
            {
                Pile currentPile = game.findCursorPile(currentMouseState);

                if (clickedCard == null && currentPile is Deck)
                    game.deck.tryTakeCards(cardsPerHand);

                if (clickedCard != null)
                {
                    if ((currentPile is FieldPile || currentPile is GoalPile) && currentPile.canPlaceCard(clickedCard))
                    {
                        currentPile.addCard(clickedCard);
                    }
                    else
                        clickedCard.bounds = clickedCardStartBounds;

                    clickedCard = null;
                }
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(cardBack, new Vector2(game.deck.bounds.Left, game.deck.bounds.Top), null, Color.White, 0, Vector2.Zero, cardScale, SpriteEffects.None, 0);

            foreach (Card handCard in game.hand.getCards())
            {
                handCard.draw(spriteBatch);
            }

            foreach (GoalPile goalPile in game.goalPiles)
            {
                spriteBatch.Draw(emptyPile, new Vector2(goalPile.bounds.Left, goalPile.bounds.Top), null, Color.White, 0, Vector2.Zero, cardScale, SpriteEffects.None, 0);
            }

            foreach (Pile fieldPile in game.fieldPiles)
            {
                List<Card> reversedPile = new List<Card>(fieldPile.getCards().Reverse<Card>());
                foreach (Card card in reversedPile)
                    if (card != clickedCard)
                        card.draw(spriteBatch);
            }

            if (clickedCard != null)
                clickedCard.draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
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
