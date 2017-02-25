using System.Collections.Generic;

namespace Gladius
{
	public enum MotionChannelType
	{
		Xposition,
		Yposition,
		Zposition,
		Zrotation,
		Xrotation,
		Yrotation
	}

	public class MotionChannel
	{
		public MotionChannelType Type;
		public List<double> Motions { get; } = new List<double>();
	}
}