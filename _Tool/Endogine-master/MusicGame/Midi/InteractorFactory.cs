using System;
using System.Collections;
using Endogine.Midi;
using Endogine;

namespace MusicGame.Midi
{
	/// <summary>
	/// Summary description for InteractorFactory.
	/// </summary>
	public class InteractorFactory
	{
		private ArrayList _spritesToStart;
		private int _readAheadMsecs;
		private Endogine.Node _trackSettings;

		public InteractorFactory(Node trackSettings)
		{
			this._spritesToStart = new ArrayList();
			EH.Instance.EnterFrameEvent+=new EnterFrame(Instance_EnterFrameEvent);

			this._trackSettings = trackSettings; //new Node();
//			this._trackSettings.GetOrCreate("Drums.Interactor").Text = "X";
//			this._trackSettings.GetOrCreate("Drums.LocSetter").Text = "Default";
//			this._trackSettings.GetOrCreate("Kalimba.Interactor").Text = "Shake";
//			this._trackSettings.GetOrCreate("Kalimba.LocSetter").Text = "Swirl";
		}

		public int ReadAheadMsecs
		{
			get {return this._readAheadMsecs;}
			set {this._readAheadMsecs = value;}
		}

		public virtual Interactor.Base CreateInteractor(TrackPlayer tp, ChannelMessage cm)
		{
			EPointF ptLoc = new EPointF();
			int note = cm.Data1; 
			int strength = cm.Data2;
			int duration = 10;

			Interactor.Base interactor = null;
			LocSetter.Base locSetter = null;

			Endogine.Node node = this._trackSettings[tp.Track.Name];
			if (node != null)
			{
				if (node["Interactor"]!=null)
				{
					string sType = "MusicGame.Midi.Interactor."+node["Interactor"].Text;
					Type type = Type.GetType(sType);
					System.Reflection.ConstructorInfo cons = type.GetConstructor(new Type[]{});
					interactor = (Interactor.Base)cons.Invoke(new object[]{});
				}
				if (node["LocSetter"]!=null)
				{
                    string sType = "MusicGame.Midi.LocSetter." + node["LocSetter"].Text;
					Type type = Type.GetType(sType);
					System.Reflection.ConstructorInfo cons = type.GetConstructor(new Type[]{});
					locSetter = (LocSetter.Base)cons.Invoke(new object[]{});
				}
			}

			if (interactor==null)
				interactor = new Interactor.Default();
//			if (locSetter==null)
//				locSetter = new LocSetter.Default();

			interactor.Prepare(tp.Track.Name, cm.MidiChannel, this._readAheadMsecs, note, strength, duration, ptLoc);
			if (locSetter!=null)
				locSetter.Parent = interactor;

			this._spritesToStart.Add(interactor);
			return interactor;
		}

		private void Instance_EnterFrameEvent()
		{
			for (int i=this._spritesToStart.Count-1; i>=0; i--)
			{
				Interactor.Base interactor = (Interactor.Base)this._spritesToStart[i];
				interactor.ConnectToParent();
				interactor.Start();
				LocSetter.Base ls = (LocSetter.Base)interactor.GetChildByName("LocSetter");
				if (ls!=null)
					ls.Start();
			}
			this._spritesToStart.Clear();
		}

	}
}
