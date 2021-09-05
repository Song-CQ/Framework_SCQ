using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Endogine
{
	/// <summary>
	/// Summary description for KeysSteering.
	/// </summary>
	public class KeysSteering
	{
		public enum KeyPresets
		{
			ArrowsSpace,
			awsdCtrlShift
		}
		private class KeyPair
		{
			public Keys Key1;
			public Keys Key2;
			public List<Keys> PressedOrder;
			public KeyPair(Keys k1, Keys k2)
			{
				Key1 = k1; Key2 = k2;
                PressedOrder = new List<Keys>();
			}
        }

        #region EKeys enum, copy of System.Windows.Forms.Keys (simpler to use and prepared for x-platform)
        public enum EKeys
        {
			A = Keys.A,
			Add = Keys.Add,
            Alt = Keys.Alt,
			Apps = Keys.Apps,
			Attn = Keys.Attn,
			B = Keys.B,
			Back = Keys.Back,
			BrowserBack = Keys.BrowserBack,
			BrowserFavorites = Keys.BrowserFavorites,
			BrowserForward = Keys.BrowserForward,
			BrowserHome = Keys.BrowserHome,
			BrowserRefresh = Keys.BrowserRefresh,
			BrowserSearch = Keys.BrowserSearch,
            BrowserStop = Keys.BrowserStop,
			C = Keys.C,
			Cancel = Keys.Cancel,
			Capital = Keys.Capital,
            CapsLock = Keys.CapsLock, //
			Clear = Keys.Clear,
            Control = Keys.Control, //
			ControlKey = Keys.ControlKey,
			Crsel = Keys.Crsel,
			D = Keys.D,
			D0 = Keys.D0,
			D1 = Keys.D1,
			D2 = Keys.D2,
			D3 = Keys.D3,
			D4 = Keys.D4,
			D5 = Keys.D5,
			D6 = Keys.D6,
			D7 = Keys.D7,
			D8 = Keys.D8,
			D9 = Keys.D9,
			Decimal = Keys.Decimal,
			Delete = Keys.Delete,
			Divide = Keys.Divide,
			Down = Keys.Down,
			E = Keys.E,
			End = Keys.End,
            Enter = Keys.Enter, //
            EraseEof = Keys.EraseEof,
			Escape = Keys.Escape,
			Execute = Keys.Execute,
			Exsel = Keys.Exsel,
			F = Keys.F,
			F1 = Keys.F1,
			F10 = Keys.F10,
			F11 = Keys.F11,
			F12 = Keys.F12,
			F13 = Keys.F13,
			F14 = Keys.F14,
			F15 = Keys.F15,
			F16 = Keys.F16,
			F17 = Keys.F17,
			F18 = Keys.F18,
			F19 = Keys.F19,
			F2 = Keys.F2,
			F20 = Keys.F20,
			F21 = Keys.F21,
			F22 = Keys.F22,
			F23 = Keys.F23,
			F24 = Keys.F24,
			F3 = Keys.F3,
			F4 = Keys.F4,
			F5 = Keys.F5,
			F6 = Keys.F6,
			F7 = Keys.F7,
			F8 = Keys.F8,
			F9 = Keys.F9,
			FinalMode = Keys.FinalMode,
			G = Keys.G,
			H = Keys.H,
            HanguelMode = Keys.HanguelMode, //
            HangulMode = Keys.HangulMode, //
            HanjaMode = Keys.HanjaMode,
			Help = Keys.Help,
			Home = Keys.Home,
			I = Keys.I,
            IMEAccept = Keys.IMEAccept, //
            IMEAceept = Keys.IMEAceept,
			IMEConvert = Keys.IMEConvert,
			IMEModeChange = Keys.IMEModeChange,
			IMENonconvert = Keys.IMENonconvert,
			Insert = Keys.Insert,
			J = Keys.J,
			JunjaMode = Keys.JunjaMode,
			K = Keys.K,
			KanaMode = Keys.KanaMode,
            KanjiMode = Keys.KanjiMode, //
            KeyCode = Keys.KeyCode, //
            L = Keys.L,
			LaunchApplication1 = Keys.LaunchApplication1,
			LaunchApplication2 = Keys.LaunchApplication2,
			LaunchMail = Keys.LaunchMail,
			LButton = Keys.LButton,
			LControlKey = Keys.LControlKey,
			Left = Keys.Left,
			LineFeed = Keys.LineFeed,
			LMenu = Keys.LMenu,
			LShiftKey = Keys.LShiftKey,
			LWin = Keys.LWin,
			M = Keys.M,
			MButton = Keys.MButton,
			MediaNextTrack = Keys.MediaNextTrack,
			MediaPlayPause = Keys.MediaPlayPause,
			MediaPreviousTrack = Keys.MediaPreviousTrack,
			MediaStop = Keys.MediaStop,
			Menu = Keys.Menu,
			Modifiers = Keys.Modifiers, //
			Multiply = Keys.Multiply,
			N = Keys.N,
			Next = Keys.Next,
			NoName = Keys.NoName,
			None = Keys.None,
			NumLock = Keys.NumLock,
			NumPad0 = Keys.NumPad0,
			NumPad1 = Keys.NumPad1,
			NumPad2 = Keys.NumPad2,
			NumPad3 = Keys.NumPad3,
			NumPad4 = Keys.NumPad4,
			NumPad5 = Keys.NumPad5,
			NumPad6 = Keys.NumPad6,
			NumPad7 = Keys.NumPad7,
			NumPad8 = Keys.NumPad8,
			NumPad9 = Keys.NumPad9,
			O = Keys.O,
			Oem1 = Keys.Oem1,
            Oem102 = Keys.Oem102, //
            Oem2 = Keys.Oem2, //
            Oem3 = Keys.Oem3, //
            Oem4 = Keys.Oem4, //
            Oem5 = Keys.Oem5,
			Oem6 = Keys.Oem6,
			Oem7 = Keys.Oem7,
			Oem8 = Keys.Oem8,
			OemBackslash = Keys.OemBackslash,
			OemClear = Keys.OemClear,
            OemCloseBrackets = Keys.OemCloseBrackets,
            Oemcomma = Keys.Oemcomma,
			OemMinus = Keys.OemMinus,
			OemOpenBrackets = Keys.OemOpenBrackets,
			OemPeriod = Keys.OemPeriod,
            OemPipe = Keys.OemPipe,
            Oemplus = Keys.Oemplus,
			OemQuestion = Keys.OemQuestion,
            OemQuotes = Keys.OemQuotes,
            OemSemicolon = Keys.OemSemicolon,
            Oemtilde = Keys.Oemtilde,
			P = Keys.P,
			Pa1 = Keys.Pa1,
			Packet = Keys.Packet,
            PageDown = Keys.PageDown,
            PageUp = Keys.PageUp,
			Pause = Keys.Pause,
			Play = Keys.Play,
			Print = Keys.Print,
			PrintScreen = Keys.PrintScreen,
            Prior = Keys.Prior,
            ProcessKey = Keys.ProcessKey,
			Q = Keys.Q,
			R = Keys.R,
			RButton = Keys.RButton,
			RControlKey = Keys.RControlKey,
			Return = Keys.Return,
			Right = Keys.Right,
			RMenu = Keys.RMenu,
			RShiftKey = Keys.RShiftKey,
			RWin = Keys.RWin,
			S = Keys.S,
			Scroll = Keys.Scroll,
			Select = Keys.Select,
			SelectMedia = Keys.SelectMedia,
			Separator = Keys.Separator,
            Shift = Keys.Shift,
            ShiftKey = Keys.ShiftKey,
			Sleep = Keys.Sleep,
            Snapshot = Keys.Snapshot,
            Space = Keys.Space,
			Subtract = Keys.Subtract,
			T = Keys.T,
			Tab = Keys.Tab,
			U = Keys.U,
			Up = Keys.Up,
			V = Keys.V,
			W = Keys.W,
			VolumeDown = Keys.VolumeDown,
			VolumeMute = Keys.VolumeMute,
			VolumeUp = Keys.VolumeUp,
			X = Keys.X,
			XButton1 = Keys.XButton1,
			XButton2 = Keys.XButton2,
			Y = Keys.Y,
			Z = Keys.Z,
			Zoom = Keys.Zoom
        }
        #endregion

        EndogineHub m_endogine;
		SortedList<Keys, KeyPair> m_slPairs;
        List<Keys> m_aOtherPressed;
        PropList _plActionsAndKeys;

		public event KeyEventHandler KeyEvent;

        public KeysSteering(System.Collections.Hashtable a_htActionsAndKeys)
        {
            this.Init();
            this.AddActionsAndKeysAsHashtable(a_htActionsAndKeys);
        }
        public KeysSteering()
        {
            this.Init();
        }
        public KeysSteering(KeyPresets k)
        {
            this.Init();
            this.AddKeyPreset(k);
        }

        private void Init()
        {
            this.m_slPairs = new SortedList<Keys, KeyPair>();
            this.m_aOtherPressed = new List<Keys>();
            this._plActionsAndKeys = new PropList();
            this.ReceiveEndogineKeys(null);
        }

        private string GetKeysEnumAsString()
        {
            List<string> lst = new List<string>();
            for (int i = 256; i < 512; i++)
            {
                Keys k = (Keys)i;
                string sk = k.ToString();
                lst.Add(sk);
            }
            lst.Sort();
            string s = "";
            foreach (string sKey in lst)
                s += "\t\t\t" + sKey + " = Keys." + sKey + ",\r\n";
            return s;
        }


        private void AddActionsAndKeysAsHashtable(System.Collections.Hashtable ht)
        {
            foreach (System.Collections.DictionaryEntry de in ht)
                this.AddActionAndKey((string)de.Key, (Keys)de.Value);
        }

        public void AddActionAndKey(string action, Keys key)
        {
            this._plActionsAndKeys.Add(action, key);
        }

        public void AddKeyPreset(KeyPresets k)
        {
			switch (k)
			{
				case KeyPresets.ArrowsSpace:
                    this._plActionsAndKeys.Add("left", System.Windows.Forms.Keys.Left);
                    this._plActionsAndKeys.Add("right", System.Windows.Forms.Keys.Right);
                    this._plActionsAndKeys.Add("up", System.Windows.Forms.Keys.Up);
                    this._plActionsAndKeys.Add("down", System.Windows.Forms.Keys.Down);
                    this._plActionsAndKeys.Add("action", System.Windows.Forms.Keys.Space);

					this.AddPair("left", "right");
					this.AddPair("up", "down");
					break;

                case KeyPresets.awsdCtrlShift:
                    this._plActionsAndKeys.Add("left", System.Windows.Forms.Keys.A);
                    this._plActionsAndKeys.Add("right", System.Windows.Forms.Keys.D);
                    this._plActionsAndKeys.Add("up", System.Windows.Forms.Keys.W);
                    this._plActionsAndKeys.Add("down", System.Windows.Forms.Keys.S);
                    this._plActionsAndKeys.Add("action", System.Windows.Forms.Keys.Space);

                    this.AddPair("left", "right");
                    this.AddPair("up", "down");
                    break;
			}
		}

        /// <summary>
        /// Define opposing actions, such as "left" and "right". Only the latest pressed key will be considered as active
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
		public void AddPair(string s1, string s2)
		{
			Keys k1 = this.GetKeyForAction(s1);
			Keys k2 = this.GetKeyForAction(s2);
			AddPair(k1, k2);
		}

		public void AddPair(Keys k1, Keys k2)
		{
			KeyPair kp = new KeyPair(k1, k2);
            //TODO: allow multiple identical keys (PropList)
            if (!this.m_slPairs.ContainsKey(k1))
    			this.m_slPairs.Add(k1, kp);
            if (!this.m_slPairs.ContainsKey(k2))
                this.m_slPairs.Add(k2, kp);
		}

        public void NoReceiveEndogineKeys()
        {
            if (m_endogine != null)
                m_endogine.KeyEvent -= new KeyEventHandler(m_endogine_KeyEvent);
            m_endogine = null;
        }
		public void ReceiveEndogineKeys(EndogineHub a_endogine)
		{
            if (m_endogine != null)
                m_endogine.KeyEvent -= new KeyEventHandler(m_endogine_KeyEvent);

            if (a_endogine == null)
                a_endogine = EH.Instance;

			m_endogine = a_endogine;
			m_endogine.KeyEvent+=new KeyEventHandler(m_endogine_KeyEvent);
		}
		public void ReceiveFormsControlKeys(System.Windows.Forms.Control ctrl)
		{
            //TODO: make sure this doesn't happen more than once
			ctrl.KeyDown+=new System.Windows.Forms.KeyEventHandler(ctrl_KeyDown);
			ctrl.KeyUp+=new System.Windows.Forms.KeyEventHandler(ctrl_KeyUp);
		}

		private void m_endogine_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
			bool bPressedBefore = false;

			int nPos = m_slPairs.IndexOfKey(e.KeyCode);
			if (nPos < 0)
			{
				//The key is not part of a key pair
				if (bDown)
				{
					if (!m_aOtherPressed.Contains(e.KeyCode))
						m_aOtherPressed.Add(e.KeyCode);
					else
						bPressedBefore = true;
				}
				else
					m_aOtherPressed.Remove(e.KeyCode);
				//return;
			}
			else
			{
				KeyPair kp = m_slPairs.Values[nPos];
				if (bDown)
				{
					if (!kp.PressedOrder.Contains(e.KeyCode))
						kp.PressedOrder.Add(e.KeyCode);
					else
						bPressedBefore = true;
				}
				else 
					kp.PressedOrder.Remove(e.KeyCode);
			}

			 //don't send event if it was pressed before
			if (bDown && bPressedBefore)
				return;

			if (KeyEvent!=null)
				KeyEvent(e, bDown);
		}

		public string GetActionForKey(Keys k)
		{
            if (!this._plActionsAndKeys.ContainsValue(k))
				return "";

            for (int i = 0; i < this._plActionsAndKeys.Count; i++)
            {
                if ((Keys)this._plActionsAndKeys.GetByIndex(i) == k)
                    return (string)this._plActionsAndKeys.GetKey(i);
            }
			return "";
		}

		public Keys GetKeyForAction(string a_sAction)
		{
            if (this._plActionsAndKeys.ContainsKey(a_sAction))
                return (Keys)this._plActionsAndKeys[a_sAction];
			return Keys.None;
		}
        public List<Keys> GetKeysForAction(string a_sAction)
        {
            List<Keys> lstReturn = new List<Keys>();
            
            if (!this._plActionsAndKeys.ContainsKey(a_sAction))
                return lstReturn;

            System.Collections.ArrayList lst = this._plActionsAndKeys.GetAllWithKey(a_sAction);
            foreach (Keys k in lst)
                lstReturn.Add(k);
            return lstReturn;
        }

		public bool GetKeyActive(string a_sAction)
		{
            List<Keys> keys = this.GetKeysForAction(a_sAction);
            foreach (Keys k in keys)
                if (this.GetKeyActive(k))
                    return true;
            return false;
		}

		public bool GetKeyActive(Keys k)
		{
			int nPos = m_slPairs.IndexOfKey(k);
			if (nPos < 0)
			{
				return m_aOtherPressed.Contains(k);
			}
			
			KeyPair kp = (KeyPair)m_slPairs.Values[nPos];
			if (kp.PressedOrder.Count == 0)
				return false;

			return ((Keys)kp.PressedOrder[kp.PressedOrder.Count-1] == k);
		}

		public List<Keys> KeysPressed
		{
			get
			{
                List<Keys> keys = new List<Keys>();

				foreach (Keys key in m_aOtherPressed)
					keys.Add(key);

				for (int i = 0; i < m_slPairs.Count; i++)
				{
					KeyPair kp = (KeyPair)m_slPairs.Values[i];
					foreach (Keys key2 in kp.PressedOrder)
						keys.Add(key2);
				}
				return keys;
			}
		}

		private void ctrl_KeyDown(object sender, KeyEventArgs e)
		{
			m_endogine_KeyEvent(e, true);
		}

		private void ctrl_KeyUp(object sender, KeyEventArgs e)
		{
			m_endogine_KeyEvent(e, false);
		}
	}
}
