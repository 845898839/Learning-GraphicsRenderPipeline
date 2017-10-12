using UnityEngine;

public class APIStates {

	private APIAntiAliasing _AntiAliasing = APIAntiAliasing.None;
	private bool _DepthTest = false;

	public APIAntiAliasing antiAliasing { get { return _AntiAliasing; } set { _AntiAliasing = value; } }
	public bool depthTest { get { return _DepthTest; } set { _DepthTest = value; } }
}
