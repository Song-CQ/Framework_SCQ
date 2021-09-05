using System;

namespace Endogine.Interpolation
{
	public class InterpolationLinearStrategy : InterpolationStrategy
	{
		public override double GetValueAt(double a_dTime, params double[] args)
		{
			return a_dTime*(args[2]-args[1])+args[1];
		}
	}
}
