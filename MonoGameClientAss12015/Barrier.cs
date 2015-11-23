using AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGameClientAss12015
{
    public class Barrier:Sprite
    {
        public int value;
        public int NumberOfHits;
        public string owner;

        public Barrier(Texture2D tx, Vector2 playerPos, SpriteFont f, int FrameCount, float layerDepth) : base(tx, playerPos, FrameCount,layerDepth)
        {

        }

        public void DrawWithMessage(SpriteBatch spriteBatch, SpriteFont font)
        {
            string barrierMessage = "Barrier Hits " + NumberOfHits.ToString();
            Vector2 msgLen = font.MeasureString(barrierMessage);
            spriteBatch.DrawString(font, barrierMessage, position + new Vector2(-spriteHeight, msgLen.X/2), Color.White);
            base.Draw(spriteBatch);
        }
    }
}
