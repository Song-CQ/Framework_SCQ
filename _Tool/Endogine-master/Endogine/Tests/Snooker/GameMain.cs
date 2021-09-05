using System;
using System.Collections;
using Endogine;
namespace Snooker
{
	/// <summary>
	/// Summary description for GameMain.
	/// </summary>
	public class GameMain
	{
		private Table table;
		private static GameMain instance;

		public GameMain()
		{
			instance = this;

			this.table = new Table();

			//EH.Instance.EnterFrameEvent+=new EnterFrame(Instance_EnterFrameEvent);
		}

		public static GameMain Instance
		{
			get {return instance;}
		}

		public Table Table
		{
			get {return table;}
		}


		public void Dispose()
		{
			if (this.table!=null)
			{
				this.table.Dispose();
				this.table = null;
			}
		}
	}
}
