using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Gladius
{
	/// <summary>
	/// Represents a node with a sequence of motion data.
	/// </summary>
	public class NodeWithMotionData : Node
	{
		public enum ChannelType {
			Xposition,
			Yposition,
			Zposition,
			Zrotation,
			Xrotation,
			Yrotation
		}

		public class Channel
		{
			public ChannelType Type;
			public List<double> Motions { get; } = new List<double>();
		}

		public IReadOnlyList<Channel> Channels { get; }

		public string Name { get; }

		public NodeWithMotionData(NodeWithMotionData parent, Vector<double> offset, IReadOnlyList<Channel> channels, string name)
			: base(parent, offset) {

			Channels = channels;
			Name = name;
		}

		public void AddChild(NodeWithMotionData child) {
			child.Parent = this;
			_children.Add(child);
		}
	}
}