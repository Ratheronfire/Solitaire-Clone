using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire
{
    public abstract class Pile
    {
        protected Stack<Card> cards;

        public Rectangle bounds { get; set; }

        abstract public void addCard(Card card);
        abstract public Card removeCard();
        abstract public bool canPlaceCard(Card card);

        public virtual List<Card> getCards()
        {
            return cards.ToList<Card>();
        }
    }

    public class Deck : Pile
    {
        public Rectangle handBounds { get; set; }
        public Hand hand { get; set; }

        public Deck(Stack<Card> c, Rectangle b)
        {
            cards = c;
            bounds = b;

            foreach (Card card in cards)
            {
                card.currentPile = this;
                card.bounds = bounds;
            }
        }

        public override void addCard(Card card)
        {
            cards.Push(card);

            card.bounds = bounds;
            card.IsVisible = false;
        }

        public override Card removeCard()
        {
            return cards.Pop();
        }

        public void tryTakeCards(int cardCount)
        {
            if (cards.Count > 0)
            {
                if (cards.Count < cardCount)
                    cardCount = cards.Count;

                for (int i = 0; i < cardCount; i++)
                {
                    Card movingCard = removeCard();
                    hand.addCard(movingCard, i * Solitaire.cardOffset);
                }
            }
            else
            {
                while (hand.cardCount() > 0)
                {
                    Card movingCard = hand.removeCard();
                    addCard(movingCard);
                }

                hand.ClearHand();
            }
        }

        public override bool canPlaceCard(Card c)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string ret = "DECK:\n";

            foreach (Card c in cards)
            {
                ret += "    " + c.ToString() + "\n";
            }

            return ret;
        }
    }

    public class Hand : Pile
    {
        protected new Queue<Card> cards;
        public Deck deck { get; set; }

        public Hand(Rectangle b)
        {
            cards = new Queue<Card>();
            bounds = b;

            foreach (Card card in cards)
            {
                card.currentPile = this;
                card.bounds = bounds;
            }
        }

        public override void addCard(Card card)
        {
            throw new NotImplementedException();
        }

        public void addCard(Card card, int offset)
        {
            cards.Enqueue(card);

            card.IsVisible = true;
            card.bounds = bounds;
            card.bounds = new Rectangle(card.bounds.X + offset, card.bounds.Y, Solitaire.cardTextureWidth, Solitaire.cardTextureHeight);
        }

        public override Card removeCard()
        {
            return cards.Dequeue();
        }

        public override bool canPlaceCard(Card card)
        {
            throw new NotImplementedException();
        }

        public override List<Card> getCards()
        {
            return cards.ToList<Card>();
        }

        public int cardCount()
        {
            return cards.Count;
        }

        public void ClearHand()
        {
            cards.Clear();
        }

        public override string ToString()
        {
            string ret = "HAND:\n";

            foreach (Card c in cards)
            {
                ret += "    " + c.ToString() + "\n";
            }

            return ret;
        }
    }

    public class FieldPile : Pile
    {
        int index;

        public FieldPile(List<Card> c, int n, Rectangle b)
        {
            cards = new Stack<Card>();
            index = n;

            foreach (Card card in c)
            {
                card.IsVisible = false;
                card.currentPile = this;

                cards.Push(card);
            }

            Card last = c.Last();

            last.IsVisible = true;
            cards.Push(last);

            bounds = b;

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards.ElementAt(i);
                card.bounds = new Rectangle(bounds.X, bounds.Y + Solitaire.cardOffset * (cards.Count - 1 - i), Solitaire.cardTextureWidth, Solitaire.cardTextureHeight);
            }
        }

        public override void addCard(Card card)
        {
            if (canPlaceCard(card))
            {
                cards.Push(card);

                card.currentPile = this;

                card.bounds = new Rectangle(bounds.X, bounds.Y + Solitaire.cardOffset * (cards.Count - 1), Solitaire.cardTextureWidth, Solitaire.cardTextureHeight);
            }
        }

        public override Card removeCard()
        {
            return cards.Pop();
        }

        public override bool canPlaceCard(Card card)
        {
            if (cards.Count == 0)
                return card.cardNumber == 13;

            Card topCard = cards.Peek();

            return ((topCard.suit == "hearts" || topCard.suit == "diamonds") && (card.suit == "clubs" || card.suit == "spades") ||
                (topCard.suit == "clubs" || topCard.suit == "spades") && (card.suit == "hearts" || card.suit == "diamonds")) &&
                card.cardNumber == topCard.cardNumber - 1;
        }

        public override string ToString()
        {
            string ret = "CARDS:\n";

            foreach (Card c in cards)
            {
                ret += "    " + c.ToString() + "\n";
            }

            return ret;
        }
    }

    public class GoalPile : Pile
    {
        int index;

        public GoalPile(int n, Rectangle b)
        {
            cards = new Stack<Card>();
            index = n;
            bounds = b;

            foreach (Card card in cards)
            {
                card.bounds = bounds;
                card.currentPile = this;
            }
        }

        public override void addCard(Card card)
        {
            if (canPlaceCard(card))
            {
                cards.Push(card);

                card.currentPile = this;
                card.bounds = bounds;
            }
        }

        public override Card removeCard()
        {
            return cards.Pop();
        }

        public override bool canPlaceCard(Card card)
        {
            if (cards.Count == 0)
                return card.cardNumber == 1;

            Card topCard = cards.Peek();

            return topCard.suit == card.suit && topCard.cardNumber == card.cardNumber - 1;
        }

        public bool isComplete()
        {
            return cards.Count == 13;
        }

        public override string ToString()
        {
            string ret = "CARDS:\n";

            foreach (Card c in cards)
            {
                ret += "    " + c.ToString() + "\n";
            }

            return ret;
        }
    }
}
