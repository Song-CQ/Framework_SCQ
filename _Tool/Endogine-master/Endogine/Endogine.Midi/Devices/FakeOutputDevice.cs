using System;

namespace Endogine.Midi.Devices
{
	/// <summary>
	/// Summary description for FakeOutputDevice.
	/// </summary>
	public class EmptyMidiSender : IMidiSender
	{
		public EmptyMidiSender()
		{
		}

		public virtual void Send(ChannelMessage message){}
		public virtual void Send(SysRealtimeMessage message){}
		public virtual void Send(SysCommonMessage message){}
		public virtual void Send(SysExMessage message){}
		public virtual void Reset(){}

		/// <summary>
		/// Gets or sets a value indicating whether or not to use a running
		/// status.
		/// </summary>
		public virtual bool RunningStatusEnabled
		{
			get{return true;}
			set{}
		}

	}
}
