using System;
using System.Collections;

namespace Endogine.Scripting.EScript
{
	/// <summary>
	/// Summary description for Expression.
	/// </summary>
	public class Expression
	{
		private class OperatorAndTerms
		{
			public Operator Op;
			public int Term1Index;
			public int Term2Index;
			public OperatorAndTerms(Operator op, int nTerm1Index, int nTerm2Index)//Term term1, Term term2)
			{
				Op = op;
				Term1Index = nTerm1Index;
				Term2Index = nTerm2Index;
			}
		}

		private ArrayList m_aChunks;
		private SortedList m_slCalcOrder;

		public Expression()
		{
		}

		/// <summary>
		/// Parse partial expression. Returns the remainder of the string (that wasn't part of the expression)
		/// </summary>
		/// <param name="sLine"></param>
		/// <returns></returns>
		public string Parse(string sLine)
		{
			//this should probably be done with regular expressions.
			//http://www.ultrapico.com/ExpressoDownload.htm

			m_aChunks = new ArrayList();
			//int nChunkStart = -1;
			int nCharIndex = 0;
			while (sLine.Length>0)
			{
				string sChar = sLine.Substring(nCharIndex,1);

				//check for quotes:
				if (sChar == "\"")
				{
					int nQuoteEnd = sLine.IndexOf("\"", nCharIndex+1);
					//note: sQuote includes quotation marks
					sLine = ExtractTerm(nQuoteEnd, sLine);
					nCharIndex = 0;
				}
				else if (sChar == " ")
				{
					sLine = sLine.Remove(nCharIndex,1);
					nCharIndex = 0;
				}
				else if (sChar == ",")
				{
					//TODO: check: this can only happen to separate function arguments

					//end of this argument.
					sLine = ExtractTerm(nCharIndex-1, sLine);
					return sLine; //NO: .Remove(0,1); //remove the , before returning
				}
				else
				{
					//is it an operator?
//					if (sChar == ".")
//					{
//						EH.Put("!");
//					}
					Operator op = Parser.GetOperator(sChar);
					if (op!=null)
					{
						//first check if it's a "." - it could be a decimal dot, not an operator:
						try
						{
							Term termTest = new Term(sLine.Substring(0,nCharIndex));
							Types.Number number = (Types.Number)termTest.Value;
							nCharIndex++;
							continue;
						}
						catch {}

						if (nCharIndex-1 >= 0)
							sLine = ExtractTerm(nCharIndex-1, sLine);

						//could be a two-token operator. Check that too:
						string sChars = sLine.Substring(0,2);
						Operator op2 = Parser.GetOperator(sChars);
						if (op2!=null)
						{
							op = op2;
							sLine = sLine.Remove(0,2);
						}
						else
							sLine = sLine.Remove(0,1);

						//if there either is no previous chunk, or it was an operator,
						//that means this must be a pre-op (-a, ++a, etc)
						if (this.m_aChunks.Count == 0 || this.GetLastChunk().GetType() == typeof(Operator))
						{
							op = Parser.GetOperator("pre"+op.InternalTokens);
							if (op == null || !op.IsPreOp)
								throw new Exception("Two operators in a row");
							//Two operators can be joined if
							//current op is a pre-op and last op was NOT (a=++b or a=-b)
							//TODO: if (!((Operator)lastChunk).CanBePreOp)
						}

						//TODO: consider a = b+++++c (in C# it must be written b++ + ++c)
//						object lastChunk = this.GetLastChunk();

						AddOperator(op);
						nCharIndex = 0;

						if(op.IsSettingOperator && (op.InternalTokens.IndexOf("=")>0))
						{
							//if an operator with "=" in it, everything on right side must be calculated first.
							//easiest way to accomplish this is to make one single term of it.
							Expression expression = new Expression();
							sLine = expression.Parse(sLine);
							Term term = this.AddTerm(expression);
						}
					}
					else if (Parser.m_aSeparators.IndexOf(sChar) >= 0 || nCharIndex == sLine.Length-1)
					{
						if (Parser.m_aSeparators.IndexOf(sChar) >= 0)
						{
							if (nCharIndex > 0)
							{
								sLine = ExtractTerm(nCharIndex-1, sLine);
								nCharIndex = 0;
							}
						}
						else //if (nCharIndex == sLine.Length-1)
						{
							sLine = ExtractTerm(nCharIndex, sLine);
							nCharIndex = 0;
						}

						bool bLastChunkWasTerm = false;
						object oLastChunk = null;
						if (this.m_aChunks.Count > 0)
						{
							oLastChunk = this.GetLastChunk();
							bLastChunkWasTerm = (oLastChunk.GetType() == typeof(Term));
						}

						if (sChar == ")" || sChar == "]" || nCharIndex == sLine.Length-1)
						{
							//we can't remove the final token - if we're parsing function arguments,
							//the caller must know that the end of the function has been reached.
							return sLine; //sLine.Remove(0,nCharIndex+1);
						}
						else if (sChar == "(" || sChar == "[")
						{
							if (sChar == "[")
							{
								//if this comes after another operator or as the first chunk in an expression,
								//then consider it a LingoList definition and parse it as such
								//otherwise, it's an index access function.
								//TODO: Convert this into an access function, ie object[x+1] -> object.IndexerAccess(x+1)
							}

							bool bIsFunction = false;
							if (sChar == "(" && bLastChunkWasTerm == true && ((Term)oLastChunk).CanBeMethod()) //.Value.GetType() == typeof(string))
								bIsFunction = true;

							if (bIsFunction)
							{
								//the term must be marked as a function...
								((Term)oLastChunk).ConvertToMethod();
								//								((Term)oLastChunk).IsFunction = true;
								Types.Method func = (Types.Method)((Term)oLastChunk).Value;

								//								sub.m_aChunks = new ArrayList();
								//NO!! changed this:
								//if it's a function, it may have several arguments
								//We'll express this as:
								//term.Value = function name
								//term.Expression = an expression with N terms (each argument is a term)
								while (true)
								{
									Expression arg = new Expression();
									sLine = arg.Parse(sLine.Remove(0,1)); //remove the , or ( first.
									//									Term term = new Term(arg);
									//									sub.m_aChunks.Add(term);
									if (arg.m_aChunks.Count > 0)
										func.AddArgument(arg);
									if (sLine.Length == 0 || sLine.Substring(0,1) == ")")
										break;
								}
							}
							else if (!bLastChunkWasTerm) //last term was an operator, 
								//which means this is just a parenthesized expression (not a function)
							{
								Expression sub = new Expression();
								sLine = sub.Parse(sLine.Remove(0,1)); //remove the (
								this.AddTerm(sub);
							}
							else //throw exception?
							{
							}
							if (sLine.Length > 0 && sLine.Substring(0,1) == ")")
								sLine = sLine.Remove(0,1);

							nCharIndex = 0;
						}
						else
						{
						}
					}
					else
					{
						nCharIndex++;
					}
				}
			}

			this.ArrangeOpsInExecutionOrder();
			return sLine;
		}

		public void ArrangeOpsInExecutionOrder()
		{
			this.m_slCalcOrder = new SortedList();

			object oLastChunk = null;

			if (this.m_aChunks.Count == 1)
			{
				OperatorAndTerms opt = new OperatorAndTerms(null, 0, -1);
				this.m_slCalcOrder.Add(1, opt);
			}
			else
			{
				for (int i = 0; i < this.m_aChunks.Count; i++)
				{
					object oChunk = this.m_aChunks[i];
					if (oChunk.GetType() == typeof(Operator))
					{
						Operator op = (Operator)oChunk;
						int t1 = -1;
						int t2 = -1;
						if (op.IsBinary)
						{
							t1 = i-1;
							t2 = i+1;
						}
						else
						{
							if (op.IsPreOp)
							{
								if (oLastChunk == null)  //like "-" in "-4" or "-var"
									t1 = i+1;
								else if (oLastChunk.GetType() == typeof(Operator)) // or "*-4"
									t1 = i+1;
							}
							if (t1 == -1)
								t1 = i-1;
						}
						oLastChunk = oChunk;
						OperatorAndTerms opt = new OperatorAndTerms(op, t1, t2);

						this.m_slCalcOrder.Add((float)i/1000+op.Priority, opt);
					}
				}
			}
		}

//		public void Restore()
//		{
//			foreach (object oChunk in this.m_aChunks)
//			{
//				if (oChunk.GetType() == typeof(Term))
//					((Term)oChunk).Restore();
//			}
//		}

		public string Print()
		{
			if (this.m_slCalcOrder == null) //don't why this should happen, though...
				this.ArrangeOpsInExecutionOrder();
			string sReturn = "(";

            foreach (OperatorAndTerms opt in this.m_slCalcOrder.Values)
			{
				Term t;
				//sReturn+="**"+i.ToString(); i++;

				t = (Term)this.m_aChunks[opt.Term1Index];
				sReturn+=t.ToString(); //.Expression.Print();

				if (opt.Op!=null)
					sReturn+=opt.Op.InternalTokens;

				if (opt.Term2Index>=0)
				{
					t = (Term)this.m_aChunks[opt.Term2Index];
					sReturn+=t.ToString(); //Expression.Print();
				}
				sReturn+=")";
			}
			return sReturn;
		}

		public Types.Object Evaluate(Executer exec)
		{
			//when an operator has been applied to two terms, other operators that involve
			//any of those terms must use the result of that operation.
			//So, keep a list of pointers that point to the resulting terms.
			Hashtable htTermPointers = new Hashtable();
			for (int i = 0; i < this.m_aChunks.Count; i++)
			{
				ArrayList a = new ArrayList();
				a.Add(i);
				htTermPointers.Add(i, a);
			}

			if (this.m_slCalcOrder == null) //don't why this should happen, though...
				this.ArrangeOpsInExecutionOrder();

			//ArrayList aChunksCopy = new ArrayList();
			ArrayList aChunksCopy = (ArrayList)this.m_aChunks.Clone();

			Term t1 = null;
			Term tNew = null;
			for (int i = 0; i < this.m_slCalcOrder.Count; i++)
			{
				OperatorAndTerms opt = (OperatorAndTerms)this.m_slCalcOrder.GetByIndex(i);
				ArrayList aTerm1Ptr = (ArrayList)htTermPointers[opt.Term1Index];
				ArrayList aTerm2Ptr = null;

				t1 = (Term)aChunksCopy[(int)aTerm1Ptr[0]]; //this.m_aChunks
				Term t2 = null;
				if (opt.Term2Index>0)
				{
					aTerm2Ptr = (ArrayList)htTermPointers[opt.Term2Index];
					t2 = (Term)aChunksCopy[(int)aTerm2Ptr[0]]; //this.m_aChunks
				}

				//now we've got the term(s). Perform the operation
				tNew = t1.PerformOperation(exec, opt.Op, t2);
				aChunksCopy[(int)aTerm1Ptr[0]] = tNew;

				//if there's a second term, change the pointer to it to the first term instead
				//so that successive operations use the result of this operation instead of the inital value
				if (t2!=null)
				{
					//aTerm2Ptr = (ArrayList)htTermPointers[opt.Term2Index];
					//aTerm2Ptr[0] = opt.Term1Index;
					//htTermPointers[opt.Term1Index] = aTerm2Ptr;
					htTermPointers[opt.Term2Index] = aTerm1Ptr;
				}
			}
			
			//NOPE - no restoring, we've still got the originals (after rewrite):
			//Always restore directly after execution, making it ready for next execution:
			Types.Object o = tNew.Value; // t1.Value;
			//this.Restore();

			return o;
		}

		public object EvaluateX()
		{
			return this.Evaluate(null).GetUnboxed(null);
		}

		public ArrayList GetTerms()
		{
			ArrayList terms = new ArrayList();
			foreach (object o in this.m_aChunks)
			{
				if (o.GetType() == typeof(Term))
					terms.Add(o);
			}
			return terms;
		}

		private string ExtractTerm(int nEndIndex, string sLine)
		{
			string sTerm = sLine.Substring(0, nEndIndex+1);
			AddTerm(sTerm);
			return sLine.Remove(0,nEndIndex+1);
		}

		private Term AddTerm(Expression expr)
		{
			Term term = new Term(expr);
			AddChunk(term);
			return term;
		}
		private Term AddTerm(string sTerm)
		{
			if (sTerm.Length == 0)
				return null;
			Term term = new Term(sTerm);
			AddChunk(term);
			return term;
		}

		private void AddOperator(Operator op)
		{
			AddChunk(op);
		}

		private object GetLastChunk()
		{
			return m_aChunks[m_aChunks.Count-1];
		}
		private void AddChunk(object oChunk)
		{
			m_aChunks.Add(oChunk);
		}
	}
}
