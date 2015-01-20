using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Solitaire
{
    public class Card
    {
        public Texture2D texture { get; set; }
        private Texture2D texStorage { get; set; }

        public Rectangle bounds { get; set; }

        public byte cardNumber { get; set; }
        public string suit { get; set; }

        public Pile currentPile { get; set; }

        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }

            set
            {
                isVisible = value;

                if (isVisible)
                    texture = texStorage;
                else
                    texture = Solitaire.cardBack;
            }
        }

        public Card(Texture2D t, byte n, string s)
        {
            texture = texStorage = t;
            cardNumber = n;
            suit = s;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, getPos(), null, Color.White, 0, Vector2.Zero, Solitaire.cardScale, SpriteEffects.None, 0);
        }

        public Vector2 getPos()
        {
            return new Vector2(bounds.Left, bounds.Top);
        }

        public float getWidth()
        {
            return texture.Width * Solitaire.cardScale;
        }

        public float getHeight()
        {
            return texture.Height * Solitaire.cardScale;
        }

        public override string ToString()
        {
            return cardNumber + " of " + suit + (isVisible ? "" : " (hidden)");
        }
    }
}
