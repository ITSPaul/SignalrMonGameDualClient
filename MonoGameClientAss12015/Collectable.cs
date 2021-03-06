﻿using AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace MonoGameClientAss12015
{
    public class Collectable : Sprite
    {
        public int value;
        public string owner;

        public Collectable(Texture2D tx, Vector2 playerPos, SpriteFont f, int FrameCount, float layerDepth) : base(tx,playerPos,FrameCount,layerDepth)
        {
            value = Utility.NextRandom(20, 50);

        }

        public void DrawWithMessage(SpriteBatch spriteBatch, SpriteFont font)
        {
            string collectableMessage = "Value " + value.ToString();
            Vector2 msgLen = font.MeasureString(collectableMessage);
            spriteBatch.DrawString(font, collectableMessage, position + new Vector2(-spriteHeight, msgLen.X / 2), Color.White);
            base.Draw(spriteBatch);
        }
    }

    public class SuperCollectable : Collectable
    {
        public SuperCollectable(Texture2D tx, Vector2 playerPos, SpriteFont f, int FrameCount, float layerDepth) : base(tx, playerPos,f, FrameCount,layerDepth)
        {
            value = 1000;
        }


    }
}
