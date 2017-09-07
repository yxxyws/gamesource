using System;
namespace Verse.Noise
{
	public class Spheres : ModuleBase
	{
		private double m_frequency = 1.0;
		public double Frequency
		{
			get
			{
				return this.m_frequency;
			}
			set
			{
				this.m_frequency = value;
			}
		}
		public Spheres() : base(0)
		{
		}
		public Spheres(double frequency) : base(0)
		{
			this.Frequency = frequency;
		}
		public override double GetValue(double x, double y, double z)
		{
			x *= this.m_frequency;
			y *= this.m_frequency;
			z *= this.m_frequency;
			double num = Math.Sqrt(x * x + y * y + z * z);
			double num2 = num - Math.Floor(num);
			double val = 1.0 - num2;
			double num3 = Math.Min(num2, val);
			return 1.0 - num3 * 4.0;
		}
	}
}
