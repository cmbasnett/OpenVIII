﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OpenVIII
{
    public class InputMouse : Input2
    {
        #region Fields

        private static MouseState last_state;
        private static MouseState state;
        private static int last_vscrollwheel = 0;
        private static object last_hscrollwheel = 0;
        private static int vscrollwheel = 0;
        private static object hscrollwheel = 0;

        public InputMouse(bool skip = true) : base(skip)
        {
        }

        #endregion Fields

        #region Properties

        public static MouseLockMode Mode { get; set; } = MouseLockMode.Screen;
        protected static MouseState Last_State => last_state;
        protected static MouseState State
        {
            get => state; set
            {
                last_vscrollwheel = vscrollwheel;
                last_hscrollwheel = hscrollwheel;
                last_state = state;
                state = value;
                vscrollwheel = state.ScrollWheelValue - last_state.ScrollWheelValue;
                hscrollwheel = state.HorizontalScrollWheelValue - last_state.HorizontalScrollWheelValue;
            }
        }

        #endregion Properties

        #region Methods

        public Point MouseLocation => new Point(state.X, state.Y);
        public Point Last_MouseLocation => new Point(last_state.X, last_state.Y);

        public static Point Location => new Point(state.X, state.Y);

        private static ButtonState Translate_Buttons(MouseButtons k, MouseState _state)
        {
            switch (k)
            {
                case MouseButtons.XButton1:
                    return _state.XButton1;

                case MouseButtons.XButton2:
                    return _state.XButton2;

                case MouseButtons.LeftButton:
                    return _state.LeftButton;

                case MouseButtons.MiddleButton:
                    return _state.MiddleButton;

                case MouseButtons.RightButton:
                    return _state.RightButton;

                case MouseButtons.MouseWheelup:
                    return vscrollwheel < 0 ? ButtonState.Pressed : ButtonState.Released;

                case MouseButtons.MouseWheeldown:
                    return vscrollwheel > 0 ? ButtonState.Pressed : ButtonState.Released;
            }
            return ButtonState.Released;
        }

        public static Vector2 Distance(MouseButtons mouseToStick, float speed)
        {
            return Translate_Stick(mouseToStick,state) * (float)Distance(speed);
        }

        private static Vector2 Translate_Stick(MouseButtons k, MouseState _state)
        {
            switch (k)
            {
                case MouseButtons.MouseToStick:
                    if (Mode == MouseLockMode.Center)
                    {
                        float tmpX = MathHelper.Clamp((_state.X - Memory.graphics.GraphicsDevice.Viewport.Bounds.Width / 2) / (50f), -1f, 1f);
                        float tmpY = MathHelper.Clamp((Memory.graphics.GraphicsDevice.Viewport.Bounds.Height / 2 - _state.Y) / (50f), -1f, 1f);
                        return new Vector2(tmpX, tmpY);
                    }
                    break;
            }
            return Vector2.Zero;
        }

        private bool Press(MouseButtons k, MouseState _state)
        {
            ButtonState bs = Translate_Buttons(k, _state);
            if (bs == ButtonState.Pressed)
                return true;
            return false;
        }

        private bool Release(MouseButtons k, MouseState _state) => !Press(k, _state);

        private bool OnPress(MouseButtons k) => Release(k, last_state) && Press(k, state);

        private bool OnRelease(MouseButtons k) => Press(k, last_state) && Release(k, state);

        protected override bool ButtonTriggered(InputButton test)
        {
            if (test != null && test.MouseButton != MouseButtons.None)
            {
                return ((test.Combo == null || base.ButtonTriggered(test.Combo)) &&
                    ((test.Trigger & ButtonTrigger.OnPress) != 0 && OnPress(test.MouseButton)) ||
                    ((test.Trigger & ButtonTrigger.OnRelease) != 0 && OnRelease(test.MouseButton)) ||
                    ((test.Trigger & ButtonTrigger.Press) != 0 && Press(test.MouseButton, state)));
            }
            return false;
        }

        public void LockMouse()
        {
            if (Memory.IsActive && Mode != MouseLockMode.Disabled) // check for focus to allow for tabbing out with out taking over mouse.
            {
                if (Mode == MouseLockMode.Center) //center mouse in screen after grabbing state, release mouse if alt tabbed out.
                {
                    Microsoft.Xna.Framework.Input.Mouse.SetPosition(Memory.graphics.GraphicsDevice.Viewport.Bounds.Width / 2, Memory.graphics.GraphicsDevice.Viewport.Bounds.Height / 2);
                }
                else if (Mode == MouseLockMode.Screen) //alt lock that clamps to viewport every frame. would be useful if using mouse to navigate menus and stuff.
                {
                    //there is a better way to clamp as if you move mouse fast enough it will escape for a short time.
                    Microsoft.Xna.Framework.Input.Mouse.SetPosition(
                        MathHelper.Clamp(state.X, 0, Memory.graphics.GraphicsDevice.Viewport.Bounds.Width),
                        MathHelper.Clamp(state.Y, 0, Memory.graphics.GraphicsDevice.Viewport.Bounds.Height));
                }
            }
        }

        protected override bool UpdateOnce()
        {
            State = Microsoft.Xna.Framework.Input.Mouse.GetState();
            LockMouse();
            return false;
        }

        #endregion Methods
    }
}