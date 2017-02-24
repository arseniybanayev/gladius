using System;
using CoreGraphics;
using SpriteKit;
using UIKit;

namespace Gladius
{
	public class SKButtonNode : SKSpriteNode
	{
		private readonly Action _actionOnTouch;

		public SKButtonNode(string imageName, Action actionOnTouch) : base(SKTexture.FromImageNamed(imageName)) {
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