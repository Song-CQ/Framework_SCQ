using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Endogine.Scripting.FlowScript
{
	/// <summary>
	/// ListPlayer accepts a list with commands, which are executed sequentially.
	/// A command can be to start another ListPlayer, to jump in the list, to wait etc
	/// Useful for complex animations and complex sound environments
	/// </summary>
	public class ListPlayer
	{
		//TODO: this is not right; one listplayer can start many thread (executers) - so
		//we can't have just one timer and one executer member... Will do for now.
		private System.Timers.Timer m_timer;
		private Endogine.Scripting.EScript.Executer m_exec;

		private Hashtable m_htCurrentAnimators;

		private ArrayList m_subPlayers;
		private System.Globalization.CultureInfo m_cultInfo;

		private System.Timers.Timer m_shameTimer; //named so because I shouldn't use this solution, see below.

		//accessed by script executor:
		public string Callmode;

		public ListPlayer()
		{
			this.Init();
			//do = "call" a label - returns at first empty line.
			//goto = go to a label. Execution continues down the script, regardless of empty lines.
			//rwd = go to start of the current label
			//start = create a new player that executes simultaneously, starting from specified label

			//TODO: analyze if FlowScript can be used efficiently to script a whole game
			//like the Excel chart in Mulle.
		}

		private void Init()
		{
			this.m_subPlayers = new ArrayList();
			this.m_htCurrentAnimators = new Hashtable();
			this.m_cultInfo = new System.Globalization.CultureInfo("en-US");
		}

		public void Run(string s)
		{
			string sConverted = ParseScript(s);
			Endogine.Scripting.EScript.Parser.Init();

			Endogine.Scripting.EScript.Functions.SetUserValue("Flow", this);
			Endogine.Scripting.EScript.MemberSearch.SearchObjects = new ArrayList();
			Endogine.Scripting.EScript.MemberSearch.SearchObjects.Add(this);

			Endogine.Scripting.EScript.Parser.ParseAndExecute(sConverted);
		}

		public static string ParseScript(string sScript)
		{
			//converts the script to "real" language

			//some keywords that need special treatment
			ArrayList aKeywords = new ArrayList();
			aKeywords.Add("do");
			aKeywords.Add("goto");
			aKeywords.Add("waitms");
			aKeywords.Add("waitframe");
			aKeywords.Add("waitframes");
			aKeywords.Add("interpolate");
			aKeywords.Add("rwd");
			//TODO: generic waitforevent(object, event)
			//will probably need some boolean stuff, like
			//waitforevent("object,event" || ("object,event" && "object,event") )

			string sThisObjectName = "Flow";
			string sNewScript = "";
			sScript = sScript.Replace("\r","");
			sScript = sScript + "\n";
			bool bLastLineEmpty = true;
			while (sScript.Length > 0)
			{
				int nEOL = sScript.IndexOf("\n");
				string sLine = "";
				if (nEOL >= 0)
				{
					sLine = sScript.Substring(0,nEOL);
					sScript = sScript.Remove(0,sLine.Length+1);
				}
				else
				{
					sLine = sScript;
					sScript = "";
				}
				sLine = sLine.Trim();

				if (sLine.Length == 0)
				{
					if (!bLastLineEmpty)
					{
						bLastLineEmpty = true;
						sNewScript+="}\n";
					}
					continue;
				}
				if (bLastLineEmpty)
				{
					sLine = sLine.Substring(0,sLine.Length-1);
					sNewScript+="\non "+sLine+"()\n{\n";//
					bLastLineEmpty = false;
					continue;
				}

				if (sLine.StartsWith("//"))
					continue;

				if (sLine.StartsWith("*"))
					sLine = "do defaultCall("+sLine.Remove(0,1)+")";

				//TODO: find which params are used - declare in function header

				int nRandomOr = sLine.IndexOf("?");
				if (nRandomOr>=0)
				{
					ArrayList aRandomExpressions = new ArrayList();
					string sRandom = sLine;
					//first expression before "?"
					Match m2 = Regex.Match(sRandom, "[( ,][\"\\w]*[?]");
					if (m2.Success)
					{
						sRandom = sRandom.Remove(0,m2.Index+m2.Length);
						aRandomExpressions.Add(m2.Value.Substring(1,m2.Length-2));
						sLine = sLine.Substring(0,m2.Index);

						//following expressions:
						while (true)
						{
							m2 = Regex.Match(sRandom, "[\"\\w]*[?]"); //[ ,]?[\"\\w]*[?]
							if (!m2.Success)
								break;
							sRandom = sRandom.Remove(0,m2.Index+m2.Length);
							aRandomExpressions.Add(m2.Value.Substring(0,m2.Length-1));
						}
						//final expression
						m2 = Regex.Match(sRandom, "[\"\\w]*[\\s\\n);]?");
						if (m2.Success)
						{
							int nEndIndex = m2.Length;
							if (m2.Value.EndsWith(";"))
								nEndIndex--;
							aRandomExpressions.Add(m2.Value.Substring(0,nEndIndex));
						}
					}
					//do x?y  translates to:  int _rnd = rnd(2); if (_rnd==0) do x; if (_rnd==1) do y;
					//o.Play("x"?"y");   to: int _rnd = rnd(2); if (_rnd==0) o.Play("x"); if (_rnd==1) do o.Play("y");
					string sBlock = "_rnd = rnd("+aRandomExpressions.Count.ToString()+")\n";
					for (int nRnd = 0; nRnd < aRandomExpressions.Count; nRnd++)
					{
						string s = (string)aRandomExpressions[nRnd];
						sBlock+="if (_rnd == "+nRnd.ToString()+") {\n" + sLine+ " "+s+"\n}\n";
					}
					sScript = sBlock+sScript;
					continue;
				}


				Match m = Regex.Match(sLine, "\\w+"); //\\w+    \\w*[\\s^.]
				if (m.Success && sLine.IndexOf("=")<0 && m.Index == 0)
				{
					string sKeyword = m.Value.Trim(); //Remove(m.Length-1,1);
					if (aKeywords.Contains(sKeyword))
					{
						bool bAddThisObjectName = true;
						switch (sKeyword)
						{
							case "do":
							case "goto":
								sLine = sLine.Remove(0,m.Index+m.Length).Trim();
								if (sLine.IndexOf("(") == -1)
									sLine+="()";
								if (sKeyword == "goto")
									sLine = "_goto("+sLine+")";
								else
								{
									sLine = "this."+sLine;
									bAddThisObjectName = false;
								}
								break;
							case "start":
								//TODO: new player: new(); player.PlayLabel("main")
								break;
							case "interpolate":
								//first value is a property: obj.Vol
								//change that to GetPropertyInfo(obj, Vol)
								string sArgs = sLine.Remove(0,m.Index+m.Length);
								string sProp = sArgs.Substring(1,sArgs.IndexOf(",")-1);
								sArgs = sArgs.Remove(0,sProp.Length+2);
								sArgs = sArgs.Remove(sArgs.Length-1,1);

								//divide sProp into object and property (e.g. o, "pitch")
								//Note that Property argument is a string.
								int nLastIndex = sProp.LastIndexOf(".");
								if (nLastIndex == -1)
									throw new Exception("Need an object and a property for interpolate");
								sProp = sProp.Substring(0,nLastIndex)
									+", \"" +sProp.Remove(0,nLastIndex+1)+"\"";

								//sLine = m.Value+"(GetPropertyInfo("+sProp+"),"+sArgs+")";
								sLine = m.Value + "("+sProp+","+sArgs+")";
								break;
							default:
								sLine = m.Value+"("+sLine.Remove(0,m.Index+m.Length)+")";
								break;
						}
						if (bAddThisObjectName)
						{
							//executer should be sent as argument with all these calls
							int nIndex = sLine.IndexOf("(");
							sLine = sLine.Insert(nIndex+1, "_exec, ");

							sLine = sThisObjectName+"."+sLine;
						}
					}
				}

				sNewScript+=sLine+";\n";
			}

			System.IO.StreamWriter wr = new System.IO.StreamWriter("script.txt");
			wr.Write(sNewScript.Replace("\n", "\r\n"));
			wr.Flush();
			wr.Close();

			return sNewScript;
		}



		//called by script executor:
		public System.Reflection.PropertyInfo GetPropertyInfo(object o, string sName)
		{
			//TODO: move to Functions
			return Serialization.Access.GetPropertyInfoNoCase(o, sName);
		}

		public float rnd(float from, float to) //Endogine.Scripting.EScript.Executer exec
		{
			Random rnd = new Random();
			return (float)(rnd.NextDouble()*(to-from)+from);
		}
		public float rnd(int max) //Endogine.Scripting.EScript.Executer exec
		{
			Random rnd = new Random();
			return rnd.Next(max);
		}
		public void waitms(Endogine.Scripting.EScript.Executer exec, int ms)
		{
			exec.Paused = true;
			this.m_exec = exec;

			this.m_timer = new System.Timers.Timer(ms);
			this.m_timer.Elapsed+=new System.Timers.ElapsedEventHandler(timer_Elapsed);
			this.m_timer.Start();
		}
		public void waitframe(Endogine.Scripting.EScript.Executer exec)
		{
		}
		public void waitframes(Endogine.Scripting.EScript.Executer exec, int frames)
		{
		}
		public void waitforevent(Endogine.Scripting.EScript.Executer exec, object o, string sEvent)
		{
			//can be called for objects that implement a specific IWait or something - 
			//e.g. when is sound finished (not playing)
		}
		public void interpolate(Endogine.Scripting.EScript.Executer exec, object o, string sPropName, float fTo, int ms)
		{
			Animation.Animator anim = new Animation.Animator(o, sPropName);

			Animation.Animator oldAnim = (Animation.Animator)this.m_htCurrentAnimators[sPropName];
			if (oldAnim!=null)
			{
				oldAnim.Dispose();
				this.m_htCurrentAnimators.Remove(sPropName);
			}
			this.m_htCurrentAnimators.Add(sPropName, anim);

			anim.TimeInterval = 50;
			object oCurrent = Serialization.Access.GetProperty(o, sPropName);
			anim.AddKey(new Animation.AnimationKey(0,Convert.ToSingle(oCurrent)));
			anim.AddKey(new Animation.AnimationKey((float)ms/anim.TimeInterval, fTo));
		}
		public void _goto(Endogine.Scripting.EScript.Executer exec, string sMethod)
		{
			//stop script execution and start from the designated method
			//when that method has finished, jump directly to next method in order!
		}
		public void startplay(Endogine.Scripting.EScript.Executer exec, string sMethod)
		{
			//start a new executer!
		}
		public void rwd(Endogine.Scripting.EScript.Executer exec)
		{
			//stop script execution and restart the method

			//TODO: how do I solve this in an elegant way?
			//Call still comes from exec, in the Step() method... So it kinda eats itself.
			//if it runs forever, the call stack will be infinitely long. Start a new thread?
			//Temporary, ugly solution: A timer that restarts it.
			exec.Paused = true;
			this.m_exec = exec;

			this.m_shameTimer = new System.Timers.Timer(1);
			this.m_shameTimer.Elapsed+=new System.Timers.ElapsedEventHandler(shameTimer_Elapsed);
			this.m_shameTimer.Start();
		}
		
		private void subPlayer_Finished(object sender)
		{
		}

		private void shameTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.m_shameTimer.Stop();
			this.m_shameTimer.Dispose();
			this.m_shameTimer = null;

			this.m_exec.Paused = false;
			this.m_exec.Rewind();
			this.m_exec.Run();
		}

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			m_timer.Stop();
			m_timer.Dispose();
			m_timer = null;

			this.m_exec.Paused = false;
			this.m_exec.Run();
			//this.m_exec = null;
		}

		public void Put(object o)
		{
			//TODO: instead we should be able to add classes to searchobjects
			//most methods will be static anyway!
			//Endogine.Scripting.EScript.MemberSearch.SearchObjects.Add(typeof(Functions));
			Endogine.Scripting.EScript.Functions.Put(o);
		}
	}
}
