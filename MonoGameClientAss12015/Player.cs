using CameraNS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using AnimatedSprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGameClientAss12015
{
    public class Player : Sprite
    {
        public SpriteFont font;
        public string info;
        public int score;
        public int health;
        public int strength;
        public string textureName;
        public float speed;
        public string clientID;
        public string playerId = string.Empty;
        private Vector2 worldBound;
        Vector2 size;
        

        public List<Sprite> _projectiles;

        public Vector2 WorldBound
        {
            get
            {
                return worldBound;
            }

            set
            {
                worldBound = value;
            }
        }

        public Player(Texture2D tx, Vector2 playerPos, float Speed, SpriteFont f, int FrameCount, float layerDepth, Vector2 worldBounds) :base(tx,playerPos,FrameCount,layerDepth )
        {
            font = f;
            font = f;
            health = 100;
            speed = Speed;
            textureName = tx.Name;
            size = new Vector2(tx.Width, tx.Height);
            worldBound = worldBounds;
        }


        public override void Update(GameTime gameTime)
        {
#if ANDROID
            
            var velocity = GetDesiredVelocityFromInput();
            position.X += (int)(velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds) * 5;
            position.Y += (int)(velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds)* 5;
            TouchCollection t = TouchPanel.GetState();

#endif
            if (InputEngineNS.InputEngine.IsKeyHeld(Keys.A))
                position += new Vector2(-1, 0) * speed;
            if (InputEngineNS.InputEngine.IsKeyHeld(Keys.W))
                position += new Vector2(0, -1) * speed;
            if (InputEngineNS.InputEngine.IsKeyHeld(Keys.S))
                position += new Vector2(0, 1) * speed;
            if (InputEngineNS.InputEngine.IsKeyHeld(Keys.D))
                position += new Vector2(1, 0) * speed;
            position = Vector2.Clamp(position, size/2, WorldBound - size/2);
            base.Update(gameTime);
        }
#if ANDROID
        Vector2 GetDesiredVelocityFromInput()
        {
            Vector2 desiredVelocity = new Vector2();

            TouchCollection touchCollection = TouchPanel.GetState();

            if (touchCollection.Count > 0)
            {
                desiredVelocity.X = touchCollection[0].Position.X - position.X;
                desiredVelocity.Y = touchCollection[0].Position.Y - position.Y;

                if (desiredVelocity.X != 0 || desiredVelocity.Y != 0)
                {
                    desiredVelocity.Normalize();
                    const float desiredSpeed = 200;
                    desiredVelocity *= desiredSpeed;
                }
            }
            return desiredVelocity;
        }
#endif
        public void DrawWithMessage(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, playerId, position + new Vector2(10, -20), Color.White);
            base.Draw(spriteBatch);
        }

    }

    public class Opponent : Sprite
    {
        public SpriteFont font;
        public string info;
        public int score;
        public int health;
        public int strength;
        public string textureName;
        public string playerId = string.Empty;
        public float speed;
        public string clientID;

        public Opponent(Texture2D tx, Vector2 playerPos, float Speed, SpriteFont f, int FrameCount, float layerDepth) 
            :base(tx,playerPos,FrameCount,layerDepth)
        {
            font = f;
            font = f;
            health = 100;
            speed = Speed;
            textureName = tx.Name;
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        public void DrawWithMessage(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, playerId, position + new Vector2(10, -20), Color.White);
            base.Draw(spriteBatch);
        }

        

    }
}
