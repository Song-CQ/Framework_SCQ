using System;
using System.Collections;
using Endogine.Midi;
using Endogine;
using Endogine.Scripting;

namespace MusicGame.Midi
{
	/// <summary>
	/// Summary description for Main.
	/// </summary>
	public class Main
	{
		private SequencerBase _sequencer;
		private SequencerBase _sequencerPre;
		private static Main _instance;
		private ArrayList _spritesToStart;
		private InteractorFactory _factory;

		private ScripterBase _scripter;
		private ArrayList _interactorClassNames;
		private int _readAheadMsecs = 1000;

		private Interactor.Base _latestNoteOnInteractor;

		public Endogine.Text.FontGenerator FontGenerator;
		public Score Score;
		public Main()
		{
			_instance = this;

			this.FontGenerator = new Endogine.Text.FontGenerator();
			Endogine.Text.FontGenerator fg = this.FontGenerator;
			fg.UseStyleTemplate("Test1");
			fg.FontSize = 20;
			fg.DefinedCharacters = Endogine.Text.FontGenerator.GetCharSet(Endogine.Text.FontGenerator.CharSet.Default, false, false);

			this.Score = new Score();
			this.Score.FontGenerator = fg;
			this.Score.Value = 0;
			this.Score.Loc = new EPointF(30,30);

//			Endogine.Forms.Label lbl = new Endogine.Forms.Label();
//			lbl.FontGenerator = fg;
//			lbl.Text = "AbrakadagvAsk49Å";
//			lbl.Loc = new EPointF(100,100);


			this._scripter = ScriptingProvider.CreateScripter("boo");
			Hashtable htScripts = new Hashtable();
			htScripts.Add("Test", Endogine.Files.FileReadWrite.Read(Endogine.AppSettings.Instance.FindFile("test.boo")));
			this._interactorClassNames = new ArrayList();
			this._interactorClassNames.Add("Test");
			this._scripter.CompileMultiple(htScripts);


			//Hashtable htKeys = new Hashtable();
			//Endogine.KeysSteering _keys = new KeysSteering(htKeys);
			EH.Instance.KeyEvent+=new KeyEventHandler(Instance_KeyEvent);

			Node tracks = new Node();
			string sFile = "Muppet_Show";
			sFile = "Flourish";
			float fSpeed = 1f;
			switch (sFile)
			{
				case "Flourish":
					tracks.GetOrCreate("Drums.Interactor").Text = "X";
					tracks.GetOrCreate("Drums.LocSetter").Text = "Default";
					tracks.GetOrCreate("Kalimba.Interactor").Text = "Shake";
					tracks.GetOrCreate("Kalimba.LocSetter").Text = "Swirl";
					tracks.GetOrCreate("Piano.Interactor").Text = "Default";
					sFile = @"C:\WINDOWS\Media\"+sFile;
					fSpeed = 0.8f;
					break;
				case "Muppet_Show":
					tracks.GetOrCreate("HONKY TONK PIAN.Interactor").Text = "X";
					tracks.GetOrCreate("HONKY TONK PIAN.LocSetter").Text = "Default";
					tracks.GetOrCreate("SAX.Interactor").Text = "Shake";
					tracks.GetOrCreate("SAX.LocSetter").Text = "Swirl";
					tracks.GetOrCreate("TUBA.Interactor").Text = "Default";
					break;
			}

			this._factory = new InteractorFactory(tracks);
			this._factory.ReadAheadMsecs = this._readAheadMsecs;

			sFile = Endogine.AppSettings.Instance.FindFile(sFile+".mid");
			//sFile = @"C:\WINDOWS\Media\Flourish.MID"; //ONESTOP
			MidiFileReader reader = new MidiFileReader(sFile);

			OutputDevice output = new OutputDevice(0);
			output.Open(0);
			this._sequencer = new SequencerBase(null, output);
			this._sequencer.Sequence = reader.Sequence;

			this._sequencer.PlaybackSpeed = fSpeed;
			this._sequencer.Start();
			//			this._sequencer.Tempo = 40;

			Endogine.Midi.Devices.EmptyMidiSender midiSender = new Endogine.Midi.Devices.EmptyMidiSender();
			//ScreenOutputDevice midiSender = new ScreenOutputDevice(this.progressBar1);
			this._sequencerPre = new SequencerBase(null, midiSender);
			this._sequencerPre.Sequence = reader.Sequence;

			TrackPlayer tp;
			for (int i=0; i<tracks.ChildNodes.Count; i++)
			{
				tp = this._sequencerPre.Player.GetTrackPlayer(tracks[i].Name); //Drums
				tp.TrackMessage+=new Endogine.Midi.TrackPlayer.TrackMessageDelegate(tp_TrackMessage);
			}

			this._sequencerPre.Start();
			this._sequencerPre.Position = (int)(this._sequencer.PlaybackTempo*this._readAheadMsecs/1000*6); //PlaybackTempo
			this._sequencerPre.PlaybackSpeed = this._sequencer.PlaybackSpeed;
			this._sequencerPre.Tempo = this._sequencer.Tempo;

		}

		public void Dispose()
		{
			if (this._sequencerPre!=null)
				this._sequencerPre.Dispose();
			if (this._sequencer!=null)
				this._sequencer.Dispose();
		}

		public static Main Instance
		{
			get {return _instance;}
		}


		private void tp_TrackEvent(object sender, MidiEvent anEvent)
		{
			//Console.WriteLine(this._sequencer.Position.ToString());
			//			anEvent.Message.Accept((TrackPlayer)sender);

			//((Endogine.Midi.MetaMessage)anEvent.Message).Type == Endogine.Midi.MetaType.TrackName
			if (anEvent.GetType() == typeof(Endogine.Midi.ChannelMessage))
			{
				//				Endogine.Midi.ChannelMessage msg = (Endogine.Midi.ChannelMessage)anEvent;
				//				if (msg.Command == Endogine.Midi.ChannelCommand.NoteOn)
				//					this.checkBox1.Checked = !(this.checkBox1.Checked);
			}
		}

		private void tp_TrackMessage(object sender, IMidiMessage message)
		{
			if (!(message is Endogine.Midi.ChannelMessage)) //message.GetType() != typeof(Endogine.Midi.ChannelMessage))
				return;

			TrackPlayer tp = (TrackPlayer)sender;
			ChannelMessage cm = (ChannelMessage)message;
			if (cm.Command != ChannelCommand.NoteOn)
				return;
			if (cm.Data2 == 0)
				return;

			this._factory.CreateInteractor(tp, cm);
		}

		private void Instance_KeyEvent(System.Windows.Forms.KeyEventArgs e, bool bDown)
		{
//			string sKey = Enum.GetName(typeof(System.Windows.Forms.Keys), e.KeyCode);
//			EH.Put(sKey + " " + EH.Instance.MouseLoc.ToString());
		}


		public Interactor.Base LatestNoteOnInteractor
		{
			get {return this._latestNoteOnInteractor;}
			set {this._latestNoteOnInteractor = value;}
		}
		
	}
}
