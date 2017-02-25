using SpriteKit;
using UIKit;
using CoreGraphics;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gladius
{
	public class GameScene : SKScene
	{
		public GameScene() {
			AnchorPoint = new CGPoint(0.5, 0.0);
		}

		private void AddAxes() {
			CGPath pathToDraw;
			var center = new CGPoint(0, 0);

			var xAxis = new SKShapeNode();
			pathToDraw = new CGPath();
			pathToDraw.MoveToPoint(center);
			pathToDraw.AddLineToPoint(new CGPoint(1000, 0));
			xAxis.Path = pathToDraw;
			xAxis.StrokeColor = UIColor.LightGray;
			xAxis.LineWidth = 2;
			AddChild(xAxis);

			var yAxis = new SKShapeNode();
			pathToDraw = new CGPath();
			pathToDraw.MoveToPoint(center);
			pathToDraw.AddLineToPoint(new CGPoint(0, 1000));
			yAxis.Path = pathToDraw;
			yAxis.StrokeColor = UIColor.LightGray;
			yAxis.LineWidth = 2;
			AddChild(yAxis);

			var zAxis = new SKShapeNode();
			pathToDraw = new CGPath();
			pathToDraw.MoveToPoint(center);
			pathToDraw.AddLineToPoint(new CGPoint(center.X - 1000, center.Y - 1000));
			zAxis.Path = pathToDraw;
			zAxis.StrokeColor = UIColor.LightGray;
			zAxis.LineWidth = 2;
			AddChild(zAxis);
		}

		private void AddButtons() {
			var buttonNode = new SKButtonNode("Rotate", () => Rotate(Plane.XY, -45));
			buttonNode.Size = new CGSize(40, 40);
			buttonNode.Position = new CGPoint(-200, -50);
			AddChild(buttonNode);

			buttonNode = new SKButtonNode("Rotate", () => Rotate(Plane.XY, 45));
			buttonNode.XScale = (nfloat)(-1.0);
			buttonNode.ZRotation = (nfloat)(-90.0).ToRadians();
			buttonNode.Size = new CGSize(40, 40);
			buttonNode.Position = new CGPoint(-150, -50);
			AddChild(buttonNode);

			buttonNode = new SKButtonNode("Rotate", () => Rotate(Plane.YZ, -45));
			buttonNode.ZRotation = (nfloat)135.0.ToRadians();
			buttonNode.Size = new CGSize(40, 40);
			buttonNode.Position = new CGPoint(-200, 0);
			AddChild(buttonNode);

			buttonNode = new SKButtonNode("Rotate", () => Rotate(Plane.YZ, 45));
			buttonNode.ZRotation = (nfloat)45.0.ToRadians();
			buttonNode.XScale = (nfloat)(-1.0);
			buttonNode.Size = new CGSize(40, 40);
			buttonNode.Position = new CGPoint(-150, 0);
			AddChild(buttonNode);

			buttonNode = new SKButtonNode("Rotate", () => Rotate(Plane.XZ, 45));
			buttonNode.ZRotation = (nfloat)(-135.0).ToRadians();
			buttonNode.Size = new CGSize(40, 40);
			buttonNode.Position = new CGPoint(-200, 50);
			AddChild(buttonNode);

			buttonNode = new SKButtonNode("Rotate", () => Rotate(Plane.XZ, -45));
			buttonNode.ZRotation = (nfloat)135.0.ToRadians();
			buttonNode.XScale = (nfloat)(-1.0);
			buttonNode.Size = new CGSize(40, 40);
			buttonNode.Position = new CGPoint(-150, 50);
			AddChild(buttonNode);
		}

		private void Rotate(Plane plane, double degrees) {
			foreach (var point in _points) {
				var vector = point.Subtract(Point3D.Zero).Rotate(plane, degrees);
				var zero = Point3D.Zero;
				zero.Add(vector);
				point.MoveTo(zero);
				point.Draw(this);
			}
		}

		private static readonly Random _random = new Random();

		private readonly List<Point3D> _points = new List<Point3D>();

		public override void DidMoveToView(SKView view) {
			BackgroundColor = UIColor.White;
			AddAxes();

			var x = 1 + 2;
			if (x == 2) {
				// Demo 1: Rotate 100 randomly scattered points around any of the three axes
				AddButtons();

				_points.AddRange(Enumerable.Repeat(1, 100)
					.Select(_ => new Point3D(
									 (_random.NextDouble() - 0.5) * 100,
									 (_random.NextDouble() - 0.5) * 100,
									 (_random.NextDouble() - 0.5) * 100)));
				foreach (var point in _points)
					point.Draw(this);
			} else {
				// Demo 2: Show the node graph
				var bvh = new BVH("Male1_A1_Stand");
				var roots = bvh.Roots.Select(Node.FromBVHNode).ToList();
				foreach (var root in roots)
					root.Draw(this);
			}
		}
	}
}