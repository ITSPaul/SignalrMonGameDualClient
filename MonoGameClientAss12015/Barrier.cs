using AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGameClientAss12015
{
    class Barrier:Sprite
    {
        public int value;
        public int NumberOfHits;
        public string owner;

        public Barrier(Texture2D tx, Vector2 playerPos, SpriteFont f, int FrameCount, float layerDepth) : base(tx, playerPos, FrameCount,layerDepth)
        {

        }

        public void DrawWithMessage(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, owner, position + new Vector2(10, -20), Color.White);
            base.Draw(spriteBatch);
        }
    }
}
