using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire
{
    abstract class Pile
    {
        protected Stack<Card> cards;

        abstract public void addCard(Card card);
        abstract public Card removeCard();
        abstract public bool canPlaceCard(Card card);

        public List<Card> getCards()
        {
            return cards.ToList<Card>();
        }
    }

    class Deck : Pile
    {
        Stack<Card> hand;

        public Deck(Stack<Card> c)
        {
            cards = c;
            hand = new Stack<Card>();

            foreach (Card card in cards)
                card.pos = Solitaire.deckPos;
        }

        public override void addCard(Card card)
        {
            throw new NotImplementedException();
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
                    hand.Push(removeCard());
            }
            else
            {
                cards = (Stack<Card>) hand.Reverse();
                hand.Clear();
            }
        }

        public override bool canPlaceCard(Card c)
        {
            return false;
        }

        public List<Card> getHand()
        {
            return hand.ToList<Card>();
        }

        public override string ToString()
        {
            string ret = "DECK:\n";

            foreach (Card c in cards)
            {
                ret += "    " + c.ToString() + "\n";
            }

            ret += "HAND\n";

            foreach (Card c in hand)
            {
                ret += "    " + c.ToString() + "\n";
            }

            return ret;
        }
    }

    class FieldPile : Pile
    {
        int pos;

        public FieldPile(List<Card> c, int p)
        {
            cards = new Stack<Card>();
            pos = p;

            for (int i = 0; i < c.Count - 1; i++)
            {
                c[i].IsVisible = false;
                cards.Push(c[i]);
            }

            Card last = c.Last();

            last.IsVisible = true;
            cards.Push(last);

            Vector2 basePos = Solitaire.fieldPositions[pos];

            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards.ElementAt(i);
                card.pos = new Vector2(basePos.X, basePos.Y + Solitaire.fieldOffset * (cards.Count - 1 - i));
            }
        }

        public override void addCard(Card card)
        {
            if (canPlaceCard(card))
            {
                cards.Push(card);
                card.pos = Solitaire.fieldPositions[pos];
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

    class GoalPile : Pile
    {
        int pos;

        public GoalPile(int p)
        {
            cards = new Stack<Card>();
            pos = p;

            foreach (Card card in cards)
                card.pos = Solitaire.goalPositions[pos];
        }

        public override void addCard(Card card)
        {
            if (canPlaceCard(card))
            {
                cards.Push(card);
                card.pos = Solitaire.goalPositions[pos];
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
