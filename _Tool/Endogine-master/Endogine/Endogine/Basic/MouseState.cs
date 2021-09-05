using System;
using System.Collections.Generic;
using System.Text;

namespace Endogine
{
    public delegate void EMouseEventHandler(object sender, MouseState e);

    public class MouseState
    {
        public event EMouseEventHandler MouseDown;
        public event EMouseEventHandler MouseMove;
        public event EMouseEventHandler MouseUp;
        public event EMouseEventHandler MouseScroll;

        public enum MouseButtons
        {
            Left, Middle, None, Right, XButton1, XButton2
        }


        MouseButtons _button;
        public MouseButtons EventButton
        {
            get { return _button; }
            set { _button = value; }
        }

        List<MouseButtons> _pressedButtons = new List<MouseButtons>();

        int _clicks;
        public int Clicks
        {
            get { return _clicks; }
            set { _clicks = value; }
        }

        int _delta;
        public int WheelDelta
        {
            get { return _delta; }
            set { _delta = value; }
        }

        EPoint _downLocation;
        public EPoint DownLocation
        {
            get { return _downLocation; }
            set { _downLocation = value; }
        }

        EPoint _lastLocation;
        public EPoint LastLocation
        {
            get { return _lastLocation; }
            set { _lastLocation = value; }
        }

        EPoint _location;
        public EPoint Location
        {
            get { return _location; }
            set { _location = value; }
        }

        bool _down;
        /// <summary>
        /// Is the indicated button down (otherwise up)
        /// </summary>
        public bool Down
        {
            get { return _down; }
            set { _down = value; }
        }

        bool _changedButtonStatus;
        /// <summary>
        /// If the button's status changed (to down or up)
        /// </summary>
        public bool ChangedButtonStatus
        {
            get { return _changedButtonStatus; }
            set { _changedButtonStatus = value; }
        }

        public MouseState()
        {
        }

        //public MouseState(MouseButtons button, EPoint location, bool down, bool changedButtonStatus, int clicks, int delta)
        //{
        //    this._button = button;
        //    this._location = location;
        //    this._down = down;
        //    this._changedButtonStatus = changedButtonStatus;
        //    this._clicks = clicks;
        //    this._delta = delta;
        //}

        //public MouseState(MouseButtons button, EPoint location, bool down, bool changedButtonStatus)
        //{
        //    this._button = button;
        //    this._location = location;
        //    this._down = down;
        //    this._changedButtonStatus = changedButtonStatus;
        //}

        public void PressedButton(MouseButtons button)
        {
            this._pressedButtons.Add(button);
            if (this.MouseDown != null)
                this.MouseDown(null, this);
        }
        public void ReleasedButton(MouseButtons button)
        {
            this._pressedButtons.Remove(button);
            if (this.MouseUp != null)
                this.MouseUp(null, this);
        }
        public void Move(EPoint newLocation)
        {
            this._lastLocation = this._location;
            this._location = newLocation;
            if (this.MouseMove != null)
                this.MouseMove(null, this);
        }


        public void SetButtonsDown(MouseButtons[] buttons)
        {
            this._pressedButtons.Clear();
            foreach (MouseButtons b in buttons)
                this._pressedButtons.Add(b);
        }

        public bool GetIsButtonDown(MouseButtons button)
        {
            return _pressedButtons.Contains(button);
        }
    }
}
