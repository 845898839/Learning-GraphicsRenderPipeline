using UnityEngine;

public class APIMonitor {

	private Texture2D _RenderTarget;

	public void SetRenderTarget(Texture2D renderTarget) {
		_RenderTarget = renderTarget;
	}

	public void Draw() {
		if (_RenderTarget != null) {
			_RenderTarget.SetPixels(APIContext.frameBufferManager.GetPixels());
			_RenderTarget.Apply();
		}
	}
}
