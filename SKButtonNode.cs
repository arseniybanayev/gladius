using System;
using SpriteKit;
using UIKit;

namespace Gladius
{
	public class SKButtonNode
	{
		public static SKNode WithImage(string imageName, Action actionOnTouch) {
			return new SKSpriteButtonNode(imageName, actionOnTouch);
		}

		public static SKNode WithText(string labelText, Action actionOnTouch) {
			return new SKLabelButtonNode(labelText, actionOnTouch);
		}
	}

	public class SKSpriteButtonNode : SKSpriteNode
	{
		private readonly Action _actionOnTouch;

		public SKSpriteButtonNode(string imageName, Action actionOnTouch) : base(SKTexture.FromImageNamed(imageName)) {
			_actionOnTouch = actionOnTouch;
			UserInteractionEnabled = true;
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt) {
			var touch = touches.AnyObject as UITouch;
			if (ContainsPoint(touch.LocationInNode(Parent)))
				_actionOnTouch();
		}
	}

	public class SKLabelButtonNode : SKLabelNode
	{
		private readonly Action _actionOnTouch;

		public SKLabelButtonNode(string labelText, Action actionOnTouch) {
			Text = labelText;
			FontColor = UIColor.Black;
			FontSize = 10;
			HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Left;
			_actionOnTouch = actionOnTouch;
			UserInteractionEnabled = true;
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt) {
			var touch = touches.AnyObject as UITouch;
			if (ContainsPoint(touch.LocationInNode(Parent)))
				_actionOnTouch();
		}
	}
}