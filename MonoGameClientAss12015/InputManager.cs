using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InputEngineNS
{

    class InputEngine : GameComponent
    {
        private static GamePadState previousPadState;
        private static GamePadState currentPadState;

        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;

        private static Vector2 previousMousePos;
        private static Vector2 currentMousePos;

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;

        public static PlayerIndex CurrentPlayer = PlayerIndex.One;

        public static Keys[] PressedKeys { get; set; }


        public static bool RecordInput { get; set; }

        bool detectText = true;
        List<Keys> keys;
        
        private static List<Keys> keyPresses = new List<Keys>();
        private static List<Buttons> buttonPresses = new List<Buttons>();

        public InputEngine(Game game)
            : base(game)
        {
            currentPadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            currentKeyState = Keyboard.GetState();
            keys = new List<Keys>();
            Game.Components.Add(this);
        }

        /// <summary>
        /// Used when switching between screens and scenes
        /// </summary>
        public static void ClearState()
        {
            previousMouseState = Mouse.GetState();
            currentMouseState = Mouse.GetState();

        }

        public override void Update(GameTime gametime)
        {
            previousPadState = currentPadState;
            previousKeyState = currentKeyState;

            currentPadState = GamePad.GetState(CurrentPlayer, GamePadDeadZone.Circular );
            currentKeyState = Keyboard.GetState();
            currentKeyState = Keyboard.GetState();

#if WINDOWS
            previousMouseState = currentMouseState;
            currentMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            currentMouseState = Mouse.GetState();
#endif

            PressedKeys = currentKeyState.GetPressedKeys();

            if (detectText)
                DetectText();

            base.Update(gametime);
        }

        
        private Keys DetectText()
        {
            foreach (Keys k in keys)
            {
                if (IsKeyPressed(k))
                {
                    return k;
                }
            }
            return Keys.None;
        }

        public static bool IsButtonPressed(Buttons buttonToCheck)
        {
            if (currentPadState.IsButtonUp(buttonToCheck) && previousPadState.IsButtonDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsButtonHeld(Buttons buttonToCheck)
        {
            if (currentPadState.IsButtonDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsKeyHeld(Keys buttonToCheck)
        {
            if (currentKeyState.IsKeyDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static bool IsKeyPressed(Keys keyToCheck)
        {
            if (currentKeyState.IsKeyUp(keyToCheck) && previousKeyState.IsKeyDown(keyToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static GamePadState CurrentPadState
        {
            get { return currentPadState; }
            set { currentPadState = value; }
        }
        public static KeyboardState CurrentKeyState
        {
            get { return currentKeyState; }
        }

        public static MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }

        public static MouseState PreviousMouseState
        {
            get { return previousMouseState; }
        }

#if WINDOWS

        public static bool IsMouseLeftClick()
        {
            if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else 
                return false;
        }

        public static bool IsMouseLeftHeld()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static bool IsMouseRightClick()
        {
            if (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static bool IsMouseRightHeld()
        {
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static Vector2 MousePosition
        {
            get { return currentMousePos; }
        }
#endif

    }
}
