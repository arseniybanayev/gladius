using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Gladius
{
	/// <summary>
	/// Helper class that parses BVH files and constructs nodes with motion data.
	/// </summary>
	public class BVH
	{
		/// <summary>
		/// Opens the specified BVH file and constructs nodes with motion data,
		/// accessible by getting the Roots property.
		/// </summary>
		public BVH(string bvhName) {
			if (bvhName.EndsWith(".bvh", StringComparison.InvariantCultureIgnoreCase))
				bvhName = bvhName.Substring(0, bvhName.Length - 4);
			var path = NSBundle.MainBundle.PathForResource(bvhName, "bvh");
			var fm = new NSFileManager();
			if (path == null || !fm.FileExists(path))
				throw new Exception($"Could not find file '{bvhName}.bvh'.");
			var data = fm.Contents(path);
			var contents = new NSString(data, NSStringEncoding.UTF8).ToString();
			var lines = contents.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			var lineCounter = 1; // start with first line of hierarchy, which should be ROOT

			while (true) {
				Roots.Add(ParseNode(lines, ref lineCounter));
				lineCounter++;
				if (!lines[lineCounter].TrimStart(' ').StartsWith("ROOT", StringComparison.InvariantCulture))
					break;
			}

			if (!string.Equals(lines[lineCounter], "MOTION"))
				throw new Exception($"Expected MOTION but encountered invalid line: {lines[lineCounter]}");

			lineCounter += 2; // skip the "Frames: x" line and get "Frame Time: y"
			FrameTimeSecs = double.Parse(lines[lineCounter].Split('\t')[1]);

			lineCounter++;
			for (; lineCounter < lines.Length; lineCounter++) {
				var motions = lines[lineCounter]
					.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(double.Parse).ToList();
				for (var i = 0; i < motions.Count; i++)
					_channels.ElementAt(i).Key.Motions.Add(motions[i]);
			}
		}

		public List<Node> Roots { get; } = new List<Node>();

		private Dictionary<MotionChannel, Node> _channels = new Dictionary<MotionChannel, Node>();

		public double FrameTimeSecs { get; }

		private Node ParseNode(string[] lines, ref int lineCounter, Node parent = null) {
			// first line, e.g. "ROOT Hips" or "JOINT Spine"
			var line = lines[lineCounter];
			var trimmed = line.TrimStart(' ');

			var endSite = false;
			string name = null;
			if (trimmed.StartsWith("ROOT", StringComparison.InvariantCulture)) {
				if (trimmed.Length > 5)
					name = trimmed.Substring(5);
			} else if (trimmed.StartsWith("JOINT", StringComparison.InvariantCulture)) {
				if (trimmed.Length > 6)
					name = trimmed.Substring(6);
			} else if (line.Contains("End Site"))
				endSite = true;
			else
				throw new Exception($"Encountered invalid line: {line}");

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
			var offsetParts = trimmed
				.Split(' ')
				.Skip(1)
				.Select(double.Parse)
				.ToArray();

			var node = parent == null ? Node.NewRoot(new DenseVector(offsetParts)) : parent.CreateAndAddChild(new DenseVector(offsetParts));
			
			if (!endSite) {
				// fourth line, "CHANNELS ..."
				lineCounter++;
				line = lines[lineCounter];
				trimmed = line.TrimStart(' ');
				if (!trimmed.StartsWith("CHANNELS", StringComparison.InvariantCulture))
					throw new Exception($"Encountered invalid line: {line}");

				var channels = trimmed
					.Split(' ').Skip(2)
					.Select(str => (MotionChannelType)Enum.Parse(typeof(MotionChannelType), str))
					.Select(t => new MotionChannel { Type = t }).ToList();
				foreach (var channel in channels)
					_channels[channel] = node;
			}

			while (true) {
				lineCounter++;
				line = lines[lineCounter];
				trimmed = line.TrimStart(' ');
				if (!string.Equals(trimmed, "}", StringComparison.InvariantCulture))
					ParseNode(lines, ref lineCounter, node);
				else
					return node;
			}
		}
	}
}