using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire
{
    partial class IPile
    {
        protected Stack<Card> cards;

        public IPile(Stack<Card> c)
        {
            cards = c;
        }

        public void addCard(Card card);
        public Card removeCard();
        public bool canPlaceCard(Card card);
    }

    class Deck : IPile
    {
        Stack<Card> hand;

        public Deck(Stack<Card> c, Stack<Card> h) : base(c)
        {
            hand = h;
        }

        public void tryTakeCards(int cardCount)
        {
            if (cards.Count > 0)
            {
                if (cards.Count < cardCount)
                    cardCount = cards.Count;

                for (int i = 0; i < cardCount; i++)
                    hand.Push(cards.Pop());
            }
            else
            {
                cards = (Stack<Card>) hand.Reverse();
            }
        }

        public bool canPlaceCard()
        {
            return false;
        }
    }

    class FieldPile : IPile
    {
        public FieldPile(List<Card> c) : base(new Stack<Card>(c))
        {
            for (int i = 0; i < c.Count - 1; i++)
            {
                c[i].IsVisible = false;
                cards.Push(c[i]);
            }

            c.Last().IsVisible = true;
            cards.Push(c.Last());
        }

        public void addCard(Card card)
        {
            if (canPlaceCard(card))
                cards.Push(card);
        }

        public Card removeCard()
        {
            return cards.Pop();
        }

        public bool canPlaceCard(Card card)
        {
            if (cards.Count == 0)
                return card.cardNumber == 13;

            Card topCard = cards.Peek();

            return ((topCard.suit == "hearts" || topCard.suit == "diamonds") && (card.suit == "clubs" || card.suit == "spades") ||
                (topCard.suit == "clubs" || topCard.suit == "spades") && (card.suit == "hearts" || card.suit == "diamonds")) &&
                card.cardNumber == topCard.cardNumber - 1;
        }
    }

    class GoalPile : IPile
    {
        public GoalPile() : base(new Stack<Card>()) {}

        public void addCard(Card card)
        {
            if (canPlaceCard(card))
                cards.Push(card);
        }

        public Card removeCard()
        {
            return cards.Pop();
        }
        
        public bool canPlaceCard(Card card)
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
    }
}
