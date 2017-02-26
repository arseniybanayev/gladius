using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using SpriteKit;
using UIKit;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Gladius
{
	/// <summary>
	/// Represents a member in a tree definition of a body, specifying an offset from its parent
	/// and propagating any transformations to its children.
	/// </summary>
	public class Node
	{
		/// <summary>
		/// Creates an orphan/root node at the origin.
		/// </summary>
		public static Node NewZeroRoot => new Node(null, new DenseVector(new double[] { 0, 0, 0 }));

		/// <summary>
		/// Creates an orphan/root node with the specified offset from the origin.
		/// </summary>
		public static Node NewRoot(Vector<double> offsetFromZero, string name) => new Node(null, offsetFromZero, name);

		/// <summary>
		/// Creates a new node with the specified offset from this node.
		/// All transformations applied to this node will be applied to the
		/// new child node, too.
		/// </summary>
		public Node CreateAndAddChild(Vector<double> offset, string name) {
			var newChild = new Node(this, offset, name);
			_children.Add(newChild);
			return newChild;
		}

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public Node Parent { get; protected set; }

		/// <summary>
		/// Children to which all transformations are applied recursively.
		/// </summary>
		public IEnumerable<Node> Children => _children;

		/// <summary>
		/// Optional identifier or name. No constraints; can be null.
		/// </summary>
		public string Name { get; }

		private Node(Node parent, Vector<double> offset, string name = null) {
			Parent = parent;
			Offset = offset;
			Name = name;
		}

		private readonly List<Node> _children = new List<Node>();

		/// <summary>
		/// Offset from its parent.
		/// </summary>
		public Vector<double> Offset { get; private set; }

		/// <summary>
		/// Total offset from the origin, accounting for parents.
		/// </summary>
		public Vector<double> OffsetFromZero {
			get {
				var node = this;
				var offset = Offset;
				while (node.Parent != null) {
					node = node.Parent;
					offset += node.Offset;
				}

				return offset / 1.5;
			}
		}

		/// <summary>
		/// Recursively applies the supplied transformation to itself and its children.
		/// </summary>
		public void ApplyTransformation(Matrix<double> transformation, bool append = false) {
			Offset *= transformation;
			foreach (var child in Children)
				child.ApplyTransformation(transformation);
		}

		public IEnumerable<SKNode> SkNodes => Children.SelectMany(c => c.SkNodes).Concat(new[] { _skNode });

		private SKNode _skNode;
		public void Draw(SKScene scene, double sec) {
			if (_skNode == null) {
				_skNode = new SKSpriteNode(SKTexture.FromImageNamed("Ball"), UIColor.Clear, new CGSize(5, 5));
				scene.AddChild(_skNode);
			}

			_skNode.RunAction(SKAction.MoveTo(new CGPoint(
				OffsetFromZero[0] - (OffsetFromZero[2] / Math.Sqrt(2.0)),
				OffsetFromZero[1] - (OffsetFromZero[2] / Math.Sqrt(2.0))), sec));

			foreach (var child in Children)
				child.Draw(scene, sec);
		}

		//private static Node FromBVHNodeImpl(BVH.BVHNode bvhNode, Node parent = null) {
		//	var node = new Node(parent, bvhNode.Offset);
		//	foreach (var bvhChild in bvhNode.Children)
		//		FromBVHNodeImpl(bvhChild, node);
		//	return node;
		//}

		//public static Node FromBVHNode(BVH.BVHNode bvhNode) {
		//	return FromBVHNodeImpl(bvhNode);
		//}

		//public override void Draw(GameScene scene) {
		//	base.Draw(scene);
		//	foreach (var child in Children.ToList()) {
		//		child.Draw(scene);
		//		DrawLineSegment(child, scene);
		//	}
		//}

		//private void DrawLineSegment(Node child, GameScene scene) {
		//	if (_children[child] != null) {
		//		_children[child].RemoveFromParent();
		//		_children[child] = null;
		//	}

		//	var line = _children[child] = new SKShapeNode();
		//	var path = new CGPath();
		//	path.MoveToPoint(SkNode.Position);
		//	path.AddLineToPoint(child.SkNode.Position);
		//	line.Path = path;
		//	line.StrokeColor = UIColor.LightGray;
		//	line.LineWidth = 2;
		//	scene.AddChild(line);
		//}
	}
}