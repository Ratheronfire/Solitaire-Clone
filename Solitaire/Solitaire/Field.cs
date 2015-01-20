using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire
{
    class Field
    {
        public Deck deck;
        public Hand hand;
        public List<FieldPile> fieldPiles;
        public List<GoalPile> goalPiles;

        public Field(List<Card> c)
        {
            Rectangle deckBounds = new Rectangle(0, 0, Solitaire.cardTextureWidth, Solitaire.cardTextureHeight);
            Rectangle handBounds = new Rectangle(Solitaire.gridWidth, 0, Solitaire.cardTextureWidth, Solitaire.cardTextureHeight);

            List<Rectangle> fieldBounds = new List<Rectangle>();
            List<Rectangle> goalBounds = new List<Rectangle>();

            for (int i = 0; i < 8; i++)
            {
                fieldBounds.Add(new Rectangle(i * Solitaire.gridWidth, Solitaire.gridHeight, Solitaire.cardTextureWidth, Solitaire.cardTextureHeight));
            }

            for (int i = 3; i < 7; i++)
            {
                goalBounds.Add(new Rectangle(i * Solitaire.gridWidth, 0, Solitaire.cardTextureWidth, Solitaire.cardTextureHeight));
            }

            List<Card> cards = new List<Card>(c);
            fieldPiles = new List<FieldPile>();
            goalPiles = new List<GoalPile>();

            for (int i = 0; i < 7; i++)
            {
                List<Card> cardPile = new List<Card>();

                for (int j = 0; j <= i; j++)
                {
                    Card cardToAdd = cards.First();

                    cardPile.Add(cardToAdd);
                    cards.Remove(cardToAdd);
                }

                fieldPiles.Add(new FieldPile(cardPile, i, fieldBounds[i]));
            }

            deck = new Deck(new Stack<Card>(cards), deckBounds);
            hand = new Hand(handBounds);

            deck.hand = hand;
            hand.deck = deck;

            for (int i = 0; i < 4; i++)
                goalPiles.Add(new GoalPile(i, goalBounds[i]));
        }

        public List<Card> shuffle(List<Card> items)
        {
            Random rng = new Random();
            List<Card> shuffledList = new List<Card>();

            int count = items.Count;

            while(count > 0)
            {
                int i = rng.Next(0, count - 1);
                int j = 0;

                foreach (Card c in items)
                {
                    if (j == i)
                        shuffledList.Add(items[j]);
                    else if (!shuffledList.Contains(items[j]))
                        j--;
                }
            }

            return shuffledList;
        }

        public Pile findCursorPile(MouseState currentMouseState)
        {
            Rectangle cursorRegion = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            if (cursorRegion.Intersects(deck.bounds))
                return deck;
            else if (cursorRegion.Intersects(hand.bounds))
                return hand;

            foreach (FieldPile fieldPile in fieldPiles)
                if (cursorRegion.Intersects(fieldPile.bounds))
                    return fieldPile;

            foreach (GoalPile goalPile in goalPiles)
                if (cursorRegion.Intersects(goalPile.bounds))
                    return goalPile;

            return null;
        }

        public Card findCardUnderCursor(MouseState currentMouseState, Pile cardPile)
        {
            Rectangle cursorRegion = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            foreach (Card card in cardPile.getCards())
            {
                if (card.IsVisible && card.bounds.Intersects(cursorRegion))
                {
                    return card;
                }
            }

            return null;
        }

        public List<Card> getCards()
        {
            List<Card> cards = new List<Card>();

            cards.AddRange(deck.getCards());
            cards.AddRange(hand.getCards());

            foreach (FieldPile fieldPile in fieldPiles)
                cards.AddRange(fieldPile.getCards());

            foreach (GoalPile goalPile in goalPiles)
                cards.AddRange(goalPile.getCards());

            return cards;
        }

        public override string ToString()
        {
            string ret = "";

            ret += deck.ToString();

            for (int i = 0; i < fieldPiles.Count; i++)
            {
                ret += "FIELD PILE " + (i+1) + "\n" + fieldPiles[i].ToString();
            }

            for (int i = 0; i < goalPiles.Count; i++)
            {
                ret += "GOAL PILE " + (i+1) + "\n" + goalPiles[i].ToString();
            }

            return ret;
        }
    }
}
