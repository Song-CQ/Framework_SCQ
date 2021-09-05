using System;
using System.Windows.Forms;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for CheckBox.
	/// </summary>
	public class CheckBox : Button
	{
		private bool m_bChecked;

		public CheckBox()
		{
		}

		public bool Checked
		{
			get {return m_bChecked;}
			set {m_bChecked = value; base.SetState(value==true?MouseEventType.Enter:MouseEventType.Leave);}
		}
		protected override void OnMouse(MouseEventArgs e, MouseEventType t)
		{
			if (t == MouseEventType.Click)
			{
				m_bChecked=!m_bChecked;
				if (m_bChecked)
                    base.SetState(MouseEventType.Enter);
				else
					base.SetState(MouseEventType.Leave);
			}
			//base.base.OnMouse(e,t);
		}
	}
}
