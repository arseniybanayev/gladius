using SpriteKit;
using UIKit;
using CoreGraphics;
using Foundation;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gladius
{
	public class GameScene : SKScene
	{
		public GameScene() {
			AnchorPoint = new CGPoint(0.5, 0.5);
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

		private void PresentBVHMenu() {
			var bvhFiles = NSBundle
				.MainBundle
				.PathsForResources("bvh", "BVHs")
				.ToDictionary(Path.GetFileNameWithoutExtension);
			for (var i = 0; i < bvhFiles.Count; i++) {
				var bvhFile = bvhFiles.ElementAt(i);
				var label = SKButtonNode.WithText(bvhFile.Key, () => {
					LoadBvh(bvhFile.Value);
				});

				label.Position = new CGPoint(-200, 75 - (i * 20));
				AddChild(label);
			}
		}

		private void LoadBvh(string bvhName) {
			if (_root != null)
				RemoveChildren(_root.SkNodes.ToArray());

			_bvh = new BVH(bvhName);
			_root = _bvh.Roots.First();
			_root.Draw(this, _bvh.FrameTimeSecs);
			NSTimer.CreateRepeatingScheduledTimer(_bvh.FrameTimeSecs, t => {
				if (_bvh.PlayOneFrame())
					_root.Draw(this, _bvh.FrameTimeSecs);
			});
		}

		public override void DidMoveToView(SKView view) {
			BackgroundColor = UIColor.White;
			//AddAxes();

			PresentBVHMenu();

		}

		private BVH _bvh;
		private Node _root;
	}
}