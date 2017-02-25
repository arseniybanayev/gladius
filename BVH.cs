using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Gladius
{
	public class BVH
	{
		public BVH(string bvhName) {
			var path = NSBundle.MainBundle.PathForResource(bvhName, "bvh");
			var fm = new NSFileManager();
			if (path == null || !fm.FileExists(path))
				throw new Exception($"Could not find file '{bvhName}.bvh'.");
			var data = fm.Contents(path);
			var contents = new NSString(data, NSStringEncoding.UTF8).ToString();
			var lines = contents.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			var lineCounter = 1; // start with first line of hierarchy, which should be ROOT
			Root = ParseNode(lines, ref lineCounter);

			lineCounter++; // now we're at "MOTION", and let's skip that
			lineCounter++;

			// TODO: parse MOTION
		}

		public BVHNode Root { get; }

		private static BVHNode ParseNode(string[] lines, ref int lineCounter) {
			var node = new BVHNode();

			// first line, e.g. "ROOT Hips" or "JOINT Spine"
			var line = lines[lineCounter];
			var trimmed = line.TrimStart(' ');

			string name = null;
			if (trimmed.StartsWith("ROOT", StringComparison.InvariantCulture)) {
				node.Type = BVHNodeType.Root;
				if (trimmed.Length > 5)
					name = trimmed.Substring(5);
			} else if (trimmed.StartsWith("JOINT", StringComparison.InvariantCulture)) {
				node.Type = BVHNodeType.Joint;
				if (trimmed.Length > 6)
					name = trimmed.Substring(6);
			} else if (line.Contains("End Site"))
				node.Type = BVHNodeType.EndSite;
			else
				throw new Exception($"Encountered invalid line: {line}");
			node.Name = name;

			// second line, "{"
			lineCounter++;
			line = lines[lineCounter];
			trimmed = line.TrimStart(' ');
			if (!string.Equals(trimmed, "{", StringComparison.InvariantCulture))
				throw new Exception($"Encountered invalid line: {line}");

			// third line, "OFFSET ..."
			lineCounter++;
			line = lines[lineCounter];
			trimmed = line.TrimStart(' ');
			if (!trimmed.StartsWith("OFFSET", StringComparison.InvariantCulture))
				throw new Exception($"Encountered invalid line: {line}");
			var offsetParts = trimmed.Split(' ').Skip(1).Select(double.Parse).ToArray();
			node.Offset = new Point3D(offsetParts[0], offsetParts[1], offsetParts[2]);

			if (node.Type != BVHNodeType.EndSite) {
				// fourth line, "CHANNELS ..."
				lineCounter++;
				line = lines[lineCounter];
				trimmed = line.TrimStart(' ');
				if (!trimmed.StartsWith("CHANNELS", StringComparison.InvariantCulture))
					throw new Exception($"Encountered invalid line: {line}");
				node.Channels = trimmed.Split(' ').Skip(2).Select(str => (BVHChannel)Enum.Parse(typeof(BVHChannel), str)).ToArray();
			}

			var children = new List<BVHNode>();
			while (true) {
				lineCounter++;
				line = lines[lineCounter];
				trimmed = line.TrimStart(' ');
				if (!string.Equals(trimmed, "}", StringComparison.InvariantCulture))
					children.Add(ParseNode(lines, ref lineCounter));
				else {
					node.Children = children;
					return node;
				}
			}
		}

		public enum BVHNodeType
		{
			Root,
			Joint,
			EndSite
		}

		public enum BVHChannel
		{
			Xposition,
			Yposition,
			Zposition,
			Zrotation,
			Xrotation,
			Yrotation
		}

		public struct BVHNode
		{
			public BVHNodeType Type;
			public string Name;
			public Point3D Offset;
			public IReadOnlyList<BVHChannel> Channels;
			public IReadOnlyList<BVHNode> Children;
		}
	}
}