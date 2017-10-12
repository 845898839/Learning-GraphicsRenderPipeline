public class APIFrameBuffer {

	private APITextureBuffer _ColorBuffer;
	private APITextureBuffer _DepthBuffer;

	public APITextureBuffer colorBuffer { get { return _ColorBuffer; } }
	public APITextureBuffer depthBuffer { get { return _DepthBuffer; } }

	public APIFrameBuffer(int width, int height) {
		_ColorBuffer = new APITextureBuffer(width, height);
		_DepthBuffer = new APITextureBuffer(width, height);
	}
}
