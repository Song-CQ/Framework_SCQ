using System;
using System.Collections;

namespace Endogine.Interpolation
{
	/// <summary>
	/// Summary description for Interpolated.
	/// </summary>
	public class Interpolator
	{
		private SortedList m_sorted;
		private double m_dMin = 0.0;
		private double m_dMax = 1.0;

		private InterpolationStrategy m_strategy;

		public Interpolator()
		{
			m_sorted = new SortedList();
            //double[] aVals = new double[]{0,0,  100,255,  200,50,  300,160,  400,0};
            //SetFromLinearList(aVals);
			InterpolationStrategy = new InterpolationLinearStrategy();
		}

		
		public void Dispose()
		{
			m_sorted.Clear();
			m_sorted = null;
			m_strategy = null; 
		}

		public InterpolationStrategy InterpolationStrategy
		{
			set {m_strategy = value;}
		}

        public void SetFromLinearList2(float[] a_aVals)
        {
            //TODO: this should be called SetFromLinearList, the others SetFromPairedLinearList

            double[] vals = new double[a_aVals.Length*2];
            for (int i = 0; i < a_aVals.Length; i++)
            {
                vals[i * 2] = (double)i;
                vals[i * 2 + 1] = (double)a_aVals[i];
            }
            this.SetFromLinearList(vals);    
        }

        /// <summary>
        /// In pairs: time, value, time, value...
        /// </summary>
        /// <param name="a_aVals"></param>
        public void SetFromLinearList(float[] a_aVals)
        {
            double[] vals = new double[a_aVals.Length];
            for (int i = 0; i < a_aVals.Length; i++)
			    vals[i] = (double)a_aVals[i];
            this.SetFromLinearList(vals);    
        }
        /// <summary>
        /// In pairs: time, value, time, value...
        /// </summary>
        /// <param name="a_aVals"></param>
		public void SetFromLinearList(double[] a_aVals)
		{
			m_sorted.Clear();

			int nCnt = a_aVals.GetLength(0);
			for (int i = 0; i < nCnt; i+=2)
				m_sorted.Add(a_aVals[i], a_aVals[i+1]);

			KeyFramesList = m_sorted;
		}

        public double Max
        {
            set { this.m_dMax = value; }
        }
		public SortedList KeyFramesList
		{
            get
            {
                return m_sorted;
            }
			set
			{
				m_sorted = value;
				int nCnt = m_sorted.Count;
				for (int i = 0; i < nCnt; i++)
				{
					double dVal = (double)m_sorted.GetByIndex(i);
					m_dMin = Math.Min(m_dMin, dVal);
					m_dMax = Math.Max(m_dMax, dVal);
				}
			}
		}

		/*public double GetSplinePoint(double dTime, double d1, double d2, double d3, double d4)
		{
			double dTime2 = dTime*dTime;
			double dTime3 = dTime2*dTime;

			return (((-d1 + 3.0*d2 - 3.0*d3 + d4)*dTime3)
				+ ((2.0*d1 - 5*d2 + 4*d3 - d4)*dTime2)
				+ ((-d1 + d3)*dTime)
				+ (2.0*d2))
				/2;
		}*/

		public double GetValueAtTime(double a_dTime)
		{
			double[,] aCachedInfo = CalcCachedInterpolationInfo(a_dTime, m_sorted);
			if (aCachedInfo.GetLength(0) > 1)
				return GetValueAtTime(a_dTime, aCachedInfo);
			return aCachedInfo[0,0];
		}

		public double GetValueAtTime(double a_dTime, double[,] a_aCachedInfo)
		{
			double dWhereBetween = (a_dTime-a_aCachedInfo[1,1])/(a_aCachedInfo[2,1]-a_aCachedInfo[1,1]);
			double dVal = m_strategy.GetValueAt(dWhereBetween, a_aCachedInfo[0,0], a_aCachedInfo[1,0], a_aCachedInfo[2,0], a_aCachedInfo[3,0]);
			return Math.Min(m_dMax, Math.Max(m_dMin, dVal));
		}

		public double[,] CalcCachedInterpolationInfo(double a_dTime, SortedList a_sorted)
		{
			double[,] aReturn = new double[,]{{1}};

			int nIndex = 0;
//			if (a_sorted.ContainsKey(a_dTime))
			nIndex = a_sorted.IndexOfKey(a_dTime);
			if (nIndex >= 0)
			{
				return new double[,]{{(double)a_sorted.GetByIndex(nIndex)}};
			}
			else
			{
				a_sorted.Add(a_dTime, -4711);
				nIndex = a_sorted.IndexOfKey(a_dTime);
				a_sorted.RemoveAt(nIndex);
			}
			
			//nIndex is meant to represent the next index after a_dTime. 
			//If a_dTime is the same as a key, nIndex should be that key's index - 1
  
			if (nIndex <= 0)
				return new double[,]{{(double)a_sorted.GetByIndex(0)}};

			if (nIndex >= a_sorted.Count)
				return new double[,]{{(double)a_sorted.GetByIndex(a_sorted.Count-1)}};

			double dTimeAtIndexBefore = (double)a_sorted.GetKey(nIndex-1);
			double dTimeAtIndexAfter = (double)a_sorted.GetKey(nIndex);

			if (a_dTime == dTimeAtIndexAfter)
			{
				/*    if (nPos < nCnt) then nPos = nPos+1
    fTimePosBefore = a_paList.getPropAt(nPos-1)
    fTimePosAfter = a_paList.getPropAt(nPos)*/
			}

			double dVal1 = 0;
			double dVal2 = (double)a_sorted.GetValueList()[nIndex-1];
			double dVal3 = (double)a_sorted.GetValueList()[nIndex];
			double dVal4 = 0;
			//TODO: support commands in the list!
			//if (ilk(mvVal2) = #List) then mvVal2 = mvVal3
			//if (ilk(mvVal3) = #List) then mvVal3 = mvVal2

			if (nIndex == 1)
				dVal1 = dVal2;
			else
				dVal1 = (double)a_sorted.GetValueList()[nIndex-2];
			//if (ilk(mvVal1) = #List) then mvVal1 = mvVal2

			if (nIndex == a_sorted.Count-1)
				dVal4 = dVal3;
			else
				dVal4 = (double)a_sorted.GetValueList()[nIndex+1];
			//TODO if (ilk(mvVal4) = #List) then mvVal4 = mvVal3

			aReturn = new double[,] {{dVal1, 0}, {dVal2, dTimeAtIndexBefore}, {dVal3, dTimeAtIndexAfter}, {dVal4,0}};
			return aReturn;
		}
	}
}
