using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solitaire
{
    class Field
    {
        public Pile deck;
        public List<Pile> fieldPiles, goalPiles;

        public Field(List<Card> c)
        {
            List<Card> cards = new List<Card>(c);
            fieldPiles = new List<Pile>();
            goalPiles = new List<Pile>();

            for (int i = 0; i < 7; i++)
            {
                List<Card> cardPile = new List<Card>();

                for (int j = 0; j <= i; j++)
                {
                    Card cardToAdd = cards.First();

                    cardPile.Add(cardToAdd);
                    cards.Remove(cardToAdd);
                }

                fieldPiles.Add(new FieldPile(cardPile, i));
            }

            deck = new Deck(new Stack<Card>(cards));

            for (int i = 0; i < 4; i++)
                goalPiles.Add(new GoalPile(i));
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
