using UIKit;
using SpriteKit;

namespace Gladius
{
	public class GameViewController : UIViewController
	{
		public override void LoadView() {
			View = new SKView();
		}

		public override void ViewDidLoad() {
			base.ViewDidLoad();
			var scene = new GameScene();
			var skView = View as SKView;
			skView.IgnoresSiblingOrder = true;
			scene.ScaleMode = SKSceneScaleMode.ResizeFill;
			skView.PresentScene(scene);
		}

		public override bool PrefersStatusBarHidden() => true;
	}
}