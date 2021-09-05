using System;
using System.Collections;
using System.Text.RegularExpressions;

using Endogine.Scripting.EScript.Nodes;

namespace Endogine.Scripting.EScript
{
	
	public class Operator
	{
		public string Tokens;
		public string InternalTokens;
		public int Priority;
		public bool IsBinary;
		public bool TermsOrderLeftToRight;
		public bool IsSettingOperator;
		public bool IsPreOp;
		public Operator(
			string sTokens,
			int nPriority,
			bool bIsBinaryNotUnary,
			bool bTermsOrderLeftToRight,
			bool bSettingOperator,
			bool bIsPreOp)
		{
			Tokens = sTokens;
			Priority = nPriority;
			IsBinary = bIsBinaryNotUnary;
			TermsOrderLeftToRight = bTermsOrderLeftToRight;
			IsSettingOperator = bSettingOperator;
			//if (sInternalTokens == null)
			InternalTokens = sTokens;
			//else
			//	InternalTokens = sInternalTokens;
			IsPreOp = bIsPreOp;
		}
	}

	/// <summary>
	/// Summary description for Parser.
	/// </summary>
	public class Parser
	{
		public static Hashtable m_htOperators = new Hashtable();
		public static ArrayList m_aSeparators = new ArrayList();
		
		private ArrayList m_aChunks = new ArrayList();
		private ArrayList m_aChunkTypesOrder = new ArrayList();//term, op, term, op, op etc...

		private static Functions m_functions;

		public Parser()
		{
			Init();
		}

		public static void Init()
		{
			if (Functions.Instance == null)
				m_functions = new Functions();

			if (m_htOperators.Count == 0)
			{
				CreateOperator(new Operator("[",1,true,true,false,false));
				CreateOperator(new Operator(".",1,true,true,false,false));
				CreateOperator(new Operator("*",3,true,true,false,false));
				CreateOperator(new Operator("/",3,true,true,false,false));
				CreateOperator(new Operator("+",4,true,true,false,false));
				CreateOperator(new Operator("-",4,true,true,false,false));
				CreateOperator(new Operator("%",9,true,true,false,false));
				CreateOperator(new Operator("&",8,true,true,false,false));
				CreateOperator(new Operator("|",10,true,true,false,false));
				CreateOperator(new Operator("&&",11,true,true,false,false));
				CreateOperator(new Operator("||",12,true,true,false,false));
				CreateOperator(new Operator("==",7,true,true,false,false));
				CreateOperator(new Operator("!=",7,true,true,false,false));
				CreateOperator(new Operator("!",88,false,true,false,false)); // this isn't really used, just for identifying "!pre" below
				CreateOperator(new Operator(">=",6,true,true,false,false));
				CreateOperator(new Operator("<=",6,true,true,false,false));
				CreateOperator(new Operator(">",6,true,true,false,false));
				CreateOperator(new Operator("<",6,true,true,false,false));
				CreateOperator(new Operator(":",100,true,true,false,false));

				CreateOperator(new Operator("++",2,false,true,true,false));
				CreateOperator(new Operator("--",2,false,true,true,false));
				CreateOperator(new Operator("=",14,true,false,true,false));
				CreateOperator(new Operator("+=",14,true,false,true,false));
				CreateOperator(new Operator("-=",14,true,false,true,false));
				CreateOperator(new Operator("*=",14,true,false,true,false));
				CreateOperator(new Operator("/=",14,true,false,true,false));

				//TODO: pre-operators:
				CreateOperator(new Operator("pre++",1,false,false,true,true));
				CreateOperator(new Operator("pre--",1,false,false,true,true));
				CreateOperator(new Operator("pre-",1,false,false,false,true));
				CreateOperator(new Operator("pre!",1,false,false,false,true));
//				CreateOperator(new Operator("*",-1,true,false,false,"ptr*"));
//				CreateOperator(new Operator("&",-1,true,false,false,"ptr&"));

				m_aSeparators.Add("(");
				m_aSeparators.Add(")");
				m_aSeparators.Add("[");
				m_aSeparators.Add("]");
				m_aSeparators.Add(",");
			}
		}

		public static object ParseAndExecuteSimple(string sScript)
		{
			sScript = @"
on main()
{"+sScript+@"
}";
			return ParseAndExecute(sScript);
		}
		public static object ParseAndExecute(string sScript)
		{
			Init();
			ClassNode classNode = Parse(sScript);
			Functions.ThisNode = classNode;
			MethodNode main = classNode.GetMethod("main");
			Executer exe = new Executer(main);
			return exe.Run();
		}

		public static ClassNode Parse(string sScript)
		{
			ArrayList aAllLines = new ArrayList();
			string[] aKeywords = new string[]{"for", "else if", "if", "else", "while", "goto", "gosub", "return"};
			//TODO: I don't handle keyword and a statement on the same line right now-
			//e.g. "if (true) blabla();" or "for (;;) put(1);"

			BaseNode rootNode = new BaseNode();
			ClassNode classNode = new ClassNode();
			rootNode.AppendChild("Class_01", classNode);
			BaseNode currentNode = classNode;

			//easier parsing if \r\n is replaced by \n
			sScript = sScript.Replace("\r\n", "\n");
			//also a little easier if script always ends with a \n
			if (!sScript.EndsWith("\n"))
				sScript+="\n";


			//structure will be [ScriptName:[MethodName:[LabelName:[clause:[clause]]]]]
			//Labels must be on top level in a method (e.g. not inside an if(){} clause)
			while (sScript.Length > 0)
			{

				//first divide into "lines" - EndOfLine represented by: return ; { or }
				string sLine = "";
				string sDividerChar = "";
				MatchCollection matches = Regex.Matches(sScript, "[^{};\\n]*[{};\\n]");
				foreach (Match m in matches)
				{
					sLine = sScript.Substring(0, m.Index+m.Length);
					sDividerChar = m.Value.Substring(m.Value.Length-1,1);
					if (sDividerChar == ";")
					{
						//must check if the ";" is inside parenthesis - if so, it doesn't count (e.g. "for(;;)")
						//only when the number of "(" is the same as the number of ")", the ";" is outside
						string sCheck = sLine.Replace("(", "");
						int nNumRight = sLine.Length-sCheck.Length;
						sCheck = sLine.Replace(")", "");
						int nNumLeft = sLine.Length-sCheck.Length;
						if (nNumRight != nNumLeft)
							continue;
					}
					sScript = sScript.Remove(0, sLine.Length);
					sLine = sLine.Remove(sLine.Length-1,1);
					break;
				}

				sLine = sLine.Trim();

				//handle if the line only consists of a { or }
				if (sLine.Length == 0)
				{
					if (HandleNodeBorderChars(sDividerChar, ref currentNode))
						continue;
				}

				//empty and remmed lines are ignored:
				if (sLine.Length == 0 || sLine.IndexOf("//") == 0)
				{
					continue;
				}

				if (currentNode.GetType() == typeof(ClassNode)) //.Depth == 1) //class level, i.e. where methods are defined
				{
					//search for "on method([args])"
					Match m = Regex.Match(sLine, "on\\s*\\w*\\s*[(][^()]*[)]");
					if (m.Success)
					{
						string sMethod = m.Value.Replace("on ", "").Trim();
						string sArgs = sMethod.Remove(0,sMethod.IndexOf("("));
						sMethod = sMethod.Substring(0,sMethod.IndexOf("("));

						MethodNode methodNode = new MethodNode();
						methodNode.DefineArguments(sArgs.Substring(1,sArgs.Length-2));
						currentNode.AppendChild(sMethod, methodNode);
						currentNode = methodNode;
						continue;
					}
				}
				//separate code flow keywords/tokens from expressions
				if (sLine.IndexOf(":") == sLine.Length-1)
				{
					//it's a label. Labels can currently only occur on Method level
					if (currentNode.GetType() != typeof(MethodNode))
						throw new Exception("Labels must be defined in Method scope!");
					//TODO: Labels can't be nodes - they're not defined with {}'s... More like an HTML anchor
					//sLine.Substring(0,sLine.Length-2)
				}

				#region Check for keywords

				foreach (string sKeyword in aKeywords)
				{
					//[^\\w]+*[( ]*
					Match m = Regex.Match(sLine+" ", sKeyword+"[^\\w]+[( ]*"); //"\\s*[(]*");
					if (m.Success)
					{
						ChunkNode newNode = null;

						string sArgs = GetStringWithinParenthesis(sLine);

						switch (sKeyword)
						{
							case "for":
								ForNode forNode = new ForNode();
								forNode.SetConditions(sArgs);
								newNode = forNode;
								//With the current line seek method, the ";" in for(;;) forces us to look ahead in script
								//TODO: lookahead for(;;)
								break;
							case "if":
								if (sArgs == null)
									throw new Exception("If statement incomplete");
								IfNode ifNode = new IfNode();
								ifNode.SetIfStatement(sArgs);
								newNode = ifNode;
								break;
							case "else if":
							case "else":
								//is the current node an IfNode? 
								if (currentNode.GetType() != typeof(IfNode))
									throw new Exception("\""+sKeyword+"\" must come after an \"if\" statement");
								IfNode elseifNode = new IfNode();
								if (sKeyword == "else if")
								{
									//TODO: don't understand C# scopes... Why can't I define sArgs here too? Can't see any risk for confusion or mistakes!?
									//And sometimes it's too slack:
									//a member variable can have the same name as a variable in a method with no complaits,
									//although that is clearly a mistake in most cases?
									if (sArgs == null)
										throw new Exception("Else if statement incomplete");
									elseifNode.SetIfStatement(sArgs);
								}
								IfNode oldIfNode = (IfNode)currentNode;
								oldIfNode.SetNextIfNode(elseifNode);
								newNode = elseifNode;
								break;
						}
						if (newNode!=null)
						{
							newNode.CloseNodeAfterNextLine = true;
							currentNode.AppendChild(sKeyword, newNode);
							currentNode = newNode;
						}
						break;
					}
				}
				#endregion


				if (HandleNodeBorderChars(sDividerChar, ref currentNode))
					continue;

				string sStatement = sLine;
				ExpressionNode expNode = new ExpressionNode();
				expNode.SetExpression(sStatement);
				currentNode.AppendChild(sStatement, expNode);

				//TODO: how to check nicely if ChunkNode is one of the base classes of currentNode?? This is ugly (?):
				try
				{
					ChunkNode chunkNode = (ChunkNode)currentNode;
					if (chunkNode.CloseNodeAfterNextLine)
						currentNode = (BaseNode)currentNode.ParentNode;
				}
				catch
				{}
			}

//			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
//			rootNode.AddToXml(doc);
//			doc.Save("Testing.xml");

			return classNode;
		}

		private static bool HandleNodeBorderChars(string sDividerChar, ref BaseNode currentNode)
		{
			if (sDividerChar == "{" || sDividerChar == "}")
			{
				if (sDividerChar == "{")
				{
					bool bCreateNewUndefinedNode = true;
					//if there's a { in the middle of the code, with nothing special preceding it, (if,for etc)
					//let's just create a chunkNode for it.

					//					if (currentNode.GetType() == typeof(MethodNode))
					//						//Metho
					//						bCreateNewUndefinedNode = false;
					//					else
					//					{
					try
					{
						//if we're in a chunk node which is supposed to close automatically after one line, like:
						// if(true) x=1;
						//and instead we encounter a {, like:
						// if(true) {
						//that means it should NOT close automatically.
						ChunkNode chunkNode = (ChunkNode)currentNode;
						if (chunkNode.CloseNodeAfterNextLine)
						{
							chunkNode.CloseNodeAfterNextLine = false;
							bCreateNewUndefinedNode = false;
						}
					}
					catch
					{}
//					}

					if (bCreateNewUndefinedNode)
					{
						ChunkNode newChunkNode = new ChunkNode();
						currentNode.AppendChild("undef", newChunkNode);
						currentNode = newChunkNode;
					}
				}
				else // "}"
					currentNode = (BaseNode)currentNode.ParentNode;
				return true;
			}
			return false;
		}

		private static string GetStringWithinParenthesis(string s)
		{
			int nIndex = s.IndexOf("(");
			if (nIndex < 0)
				return null;
			s = s.Remove(0,nIndex+1);
			if (!s.EndsWith(")"))
				throw new Exception("Expression must end with \")\"");
			return s.Substring(0,s.Length-1);
		}


		private static void CreateOperator(Operator op)
		{
			m_htOperators.Add(op.Tokens, op);
		}
		public static Operator GetOperator(string s)
		{
			if (m_htOperators.Count == 0)
				Init();
			return (Operator)m_htOperators[s];
		}
	}
}
