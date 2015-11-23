using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraNS
{
    class Camera
    {
        Vector2 _camPos = Vector2.Zero;
        Vector2 _worldBound;
        public Matrix CurrentCameraTranslation { get
            {
                return Matrix.CreateTranslation(new Vector3(
                    -CamPos,
                    0));
            } }

        public Vector2 CamPos
        {
            get
            {
                return _camPos;
            }

            set
            {
                _camPos = value;
            }
        }

        public Camera(Vector2 startPos, Vector2 bound)
        {
            CamPos = startPos;
            _worldBound = bound;
        }

        public void move(Vector2 delta, Viewport v)
        {
            CamPos += delta;
            CamPos = Vector2.Clamp(CamPos, Vector2.Zero, _worldBound - new Vector2(v.Width, v.Height));
        }

        public void follow(Vector2 followPos, Viewport v)
        {
            _camPos = followPos - new Vector2(v.Width / 2, v.Height / 2);
            _camPos = Vector2.Clamp(_camPos, Vector2.Zero, _worldBound - new Vector2(v.Width, v.Height));
        }

    }
}
