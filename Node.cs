using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using SpriteKit;
using UIKit;

namespace Gladius
{
	public class Body
	{
		public Node Root { get; private set; }

		public Body(BVH bvh) {
			
		}
	}

	public class Node : Point3D
	{
		public static new Node Zero => new Node(null, Vector3D.Zero);

		public Node(Node parent, Vector3D distance) : base(0, 0, 0) {
			parent?.AddChild(this);
			MoveTo((parent ?? this) + distance);
		}

		public IEnumerable<Node> Children => _children.Keys;
		private readonly Dictionary<Node, SKShapeNode> _children = new Dictionary<Node, SKShapeNode>();

		public Node Parent { get; private set; }

		public void AddChild(Node child) {
			_children[child] = null;
			child.Parent = this;
		}

		public override void Draw(GameScene scene) {
			base.Draw(scene);
			foreach (var child in Children.ToList()) {
				child.Draw(scene);
				DrawLineSegment(child, scene);
			}
		}

		private void DrawLineSegment(Node child, GameScene scene) {
			if (_children[child] != null) {
				_children[child].RemoveFromParent();
				_children[child] = null;
			}

			var line = _children[child] = new SKShapeNode();
			var path = new CGPath();
			path.MoveToPoint(SkNode.Position);
			path.AddLineToPoint(child.SkNode.Position);
			line.Path = path;
			line.StrokeColor = UIColor.LightGray;
			line.LineWidth = 2;
			scene.AddChild(line);
		}
	}
}