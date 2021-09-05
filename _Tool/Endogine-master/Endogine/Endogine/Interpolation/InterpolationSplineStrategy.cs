using System;

namespace Endogine.Interpolation
{
	public class InterpolationSplineStrategy : InterpolationStrategy
	{
		public override double GetValueAt(double a_dTime, params double[] args)
		{
			double dTime2 = a_dTime*a_dTime;
			double dTime3 = dTime2*a_dTime;

			double d1 = args[0];
			double d2 = args[1];
			double d3 = args[2];
			double d4 = args[3];

			return ((-d1 + 3.0*d2 - 3.0*d3 + d4)*dTime3
				+ (2.0*d1 - 5*d2 + 4*d3 - d4)*dTime2
				+ (-d1 + d3)*a_dTime
				+ 2.0*d2)
				/2;
		}
	}
}
