using UnityEngine;

public class APIVertexBuffer {
	public Texture texture;
	public Vector3[] positions;
	public Vector3[] normals;
	public Vector2[] uvs;
	public Vector4[] colors;
}

public class APIDataBuffer {

	private APIVertexBuffer _VertexBuffer;

	public APIVertexBuffer vertexBuffer { get { return _VertexBuffer; } }

	public APIDataBuffer() {
		_VertexBuffer = new APIVertexBuffer();
	}
}