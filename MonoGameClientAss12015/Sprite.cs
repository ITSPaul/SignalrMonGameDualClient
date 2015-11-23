using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AnimatedSprite
{
    public class Sprite
    {
        //sprite texture and position
        protected Texture2D spriteImage;
        private bool visible;
        protected Vector2 origin;
        protected float angleOfRotation;
        protected float spriteDepth = 1f;
        protected Vector2 WorldOrigin
        {
            get { return position + origin; }
            
        }

        static protected Rectangle CameraRect;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public Texture2D SpriteImage
        {
            get { return spriteImage; }
            set { spriteImage = value; }
        }
        public Vector2 position;

        //the number of frames in the sprite sheet
        //the current fram in the animation
        //the time between frames
        int numberOfFrames = 0;
        int currentFrame = 0;
        int mililsecondsBetweenFrames = 100;
        float timer = 0f;

        //the width and height of our texture
        public int spriteWidth = 0;
        public int spriteHeight = 0;

        //the source of our image within the sprite sheet to draw
        Rectangle sourceRectangle;

        public Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }
        

        

        public Sprite(Texture2D texture,Vector2 userPosition, int framecount, float layerDepth)
        {
            spriteDepth = layerDepth;
            spriteImage = texture;
            position = userPosition;
            numberOfFrames = framecount;
            spriteHeight = spriteImage.Height;
            visible = true;
            spriteWidth = spriteImage.Width / framecount;
            // added to allow sprites to rotate
            origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            angleOfRotation = 0;
        }


        public virtual void Update(GameTime gametime)
        {
            timer += (float)gametime.ElapsedGameTime.Milliseconds;

            //if the timer is greater then the time between frames, then animate
                    if (timer > mililsecondsBetweenFrames)
                    {
                        //moce to the next frame
                        currentFrame++;

                        //if we have exceed the number of frames
                        if (currentFrame > numberOfFrames - 1)
                        {
                            currentFrame = 0;
                        }
                        //reset our timer
                        timer = 0f;
                    }
            //set the source to be the current frame in our animation
                    sourceRectangle = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            
            }
        public bool collisionDetect(Sprite other)
        {
            Rectangle myBound = new Rectangle((int)this.position.X, (int)this.position.Y, this.spriteWidth, this.spriteHeight);
            Rectangle otherBound = new Rectangle((int)other.position.X, (int)other.position.Y, other.spriteWidth, other.spriteHeight);
            if (myBound.Intersects(otherBound))
                return true;
            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(spriteImage,
                    position, sourceRectangle,
                    Color.White, angleOfRotation, origin,
                    1.0f, SpriteEffects.None, spriteDepth);
            }
        }       

    }
}
