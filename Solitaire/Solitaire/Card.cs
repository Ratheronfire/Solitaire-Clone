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

        public Vector2 pos { get; set; }
        public float scale { get; set; }

        public byte cardNumber { get; set; }
        public string suit { get; set; }

        public Rectangle bounds
        {
            get
            {
                return new Rectangle((int) pos.X, (int) pos.Y, (int) getWidth(), (int) getHeight());
            }
        }

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

                if (!isVisible)
                    texture = Solitaire.cardBack;
                else
                    texture = texStorage;
            }
        }

        public Card(Texture2D t, byte n, string s)
        {
            texture = texStorage = t;
            cardNumber = n;
            suit = s;

            scale = Solitaire.cardScale;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, pos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public float getWidth()
        {
            return texture.Width * scale;
        }

        public float getHeight()
        {
            return texture.Height * scale;
        }

        public string ToString()
        {
            return cardNumber + " of " + suit;
        }
    }
}
