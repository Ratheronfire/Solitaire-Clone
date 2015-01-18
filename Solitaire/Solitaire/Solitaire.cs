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
    public enum GridTypes { Deck, Field, Goal };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Solitaire : Microsoft.Xna.Framework.Game
    {
        public static float cardScale = 0.3f;
        public static Texture2D cardBack, emptyPile;

        public static int screenWidth = 1280;
        public static int screenHeight = 720;

        public static int cardTextureWidth = 500;
        public static int cardTextureHeight = 726;

        public static Rectangle cardBounds;

        public static int fieldOffset = 5;

        public static float gridWidth, gridHeight;

        public static Vector2 deckPos;
        public static List<Vector2> fieldPositions, goalPositions;

        Field game;

        MouseState previousMouseState, currentMouseState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Card> cards;
        Random rng;

        Card clickedCard;
        Vector2 clickedCardStartPos;

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

            gridWidth = screenWidth / 7;
            gridHeight = (cardTextureHeight * cardScale) + 10;

            deckPos = Vector2.Zero;

            fieldPositions = new List<Vector2>();
            goalPositions = new List<Vector2>();

            for (int i = 0; i < 8; i++)
            {
                fieldPositions.Add(new Vector2(i * gridWidth, gridHeight));
            }

            for (int i = 3; i < 7; i++)
            {
                goalPositions.Add(new Vector2(i * gridWidth, 0));
            }

            cardBounds = new Rectangle(0, 0, (int) (cardTextureWidth * cardScale), (int) (cardTextureHeight * cardScale));

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
                clickedCard = findCardUnderCursor();

                if (clickedCard != null)
                    clickedCardStartPos = clickedCard.pos;
            }
            else if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (clickedCard != null && clickedCard.IsVisible)
                    clickedCard.pos = new Vector2(currentMouseState.X - (clickedCard.getWidth() / 2), currentMouseState.Y - (clickedCard.getHeight() / 2));
            }
            else if (mouseButtonJustReleased(MouseButtons.Left))
            {
                if (clickedCard != null)
                {
                    clickedCard.pos = clickedCardStartPos;
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

            spriteBatch.Draw(cardBack, deckPos, null, Color.White, 0, Vector2.Zero, cardScale, SpriteEffects.None, 0);

            foreach (Vector2 goalPos in goalPositions)
            {
                spriteBatch.Draw(emptyPile, goalPos, null, Color.White, 0, Vector2.Zero, cardScale, SpriteEffects.None, 0);
            }

            foreach (Pile fieldPile in game.fieldPiles)
            {
                List<Card> reversedPile = new List<Card>(fieldPile.getCards().Reverse<Card>()) ;
                foreach (Card card in reversedPile)
                    if (card != clickedCard)
                        card.draw(spriteBatch);
            }

            if (clickedCard != null)
                clickedCard.draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Card findCardUnderCursor()
        {
            Rectangle cursorRegion = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            foreach (Card card in cards)
            {
                if (card.IsVisible && card.bounds.Intersects(cursorRegion))
                {
                    return card;
                }
            }

            return null;
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
