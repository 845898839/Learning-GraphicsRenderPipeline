using UnityEngine;

public class APIFrameBufferManager {

	private APIFrameBuffer _FrameBufferTarget;
	private APIFrameBuffer[] _FrameBuffers;
	private int _FrameBufferTargetID;
	private Color _DefaultColor;
	private int _Width;
	private int _Height;

	public APIFrameBuffer frameBufferTarget { get { return _FrameBufferTarget; } }
	public Color defaultColor { get { return _DefaultColor; } set { _DefaultColor = value; } }
	public int width { get { return _Width; } }
	public int height { get { return _Height; } }

	public APIFrameBufferManager(int width, int height, int count = 2) {
		_FrameBuffers = new APIFrameBuffer[count];

		for (int i = 0; i < count; i++)
			_FrameBuffers[i] = new APIFrameBuffer(width, height);

		_Width = width;
		_Height = height;

		ChangeBuffer();
	}

	public Color[] GetPixels() {
		return _FrameBufferTarget.colorBuffer.GetPixels();
	}

	public void ChangeBuffer() {
		_FrameBufferTargetID = (_FrameBufferTargetID + 1) % _FrameBuffers.Length;

		_FrameBufferTarget = _FrameBuffers[_FrameBufferTargetID];
	}

	public void Clear(APIClear flags) {
		if ((flags & APIClear.COLOR) != 0)
			_FrameBufferTarget.colorBuffer.SetColor(_DefaultColor);

		if ((flags & APIClear.DEPTH) != 0)
			_FrameBufferTarget.depthBuffer.SetColor(Color.black);
	}
}
