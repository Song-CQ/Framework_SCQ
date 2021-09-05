using System;

namespace Endogine.Interpolation
{
	public abstract class InterpolationStrategy
	{
		abstract public double GetValueAt(double a_dTime, params double[] a_args);
	}
}
