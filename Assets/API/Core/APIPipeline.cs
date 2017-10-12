using UnityEngine;

public class APIPipeline {

	private APIShader _Shader;

	private VertData _VertData0;
	private VertData _VertData1;
	private VertData _VertData2;
	private Vector2 _Position0;
	private Vector2 _Position1;
	private Vector2 _Position2;
	private float _XMAXF;
	private float _YMAXF;
	private float _XMINF;
	private float _YMINF;
	private int _XMAX;
	private int _YMAX;
	private int _XMIN;
	private int _YMIN;

	private float _AreaFull;
	private float _Area0;
	private float _Area1;
	private float _Area2;

	private float _Alpha;

	public void SetShader(APIShader shader) {
		_Shader = shader;
	}

	private bool CullFrustrumVertex(Vector2 position) {
		float x = position.x;
		float y = position.y;

		return x >= 0 && x <= 1f && y >= 0 && y <= 1f;
	}

	private bool CullFrustrum() {
		return true;
		return CullFrustrumVertex(_Position0) || CullFrustrumVertex(_Position1) || CullFrustrumVertex(_Position2);
	}

	private bool CullBackface() {
		Vector2 edge1 = _Position1 - _Position0;
		Vector2 edge2 = _Position2 - _Position1;

		_AreaFull = APIMath.Cross(edge2, edge1);

		return _AreaFull > 0;
	}

	private bool DepthTest(Vector3 point, int x, int y) {
		if (!APIContext.states.depthTest)
			return true;

		APITextureBuffer depthBuffer = APIContext.frameBufferManager.frameBufferTarget.depthBuffer;
		float depth = point.z;

		if (APIMath.Tex2D(depthBuffer, point).r <= depth) {
			depthBuffer.SetPixel(x, y, Color.red * depth);

			return true;
		}

		return false;
	}

	private bool CullPoint(Vector2 point) {
		_Area0 = APIMath.Cross(point - _Position0, _Position1 - _Position0) / _AreaFull;
		_Area1 = APIMath.Cross(point - _Position1, _Position2 - _Position1) / _AreaFull;
		_Area2 = APIMath.Cross(point - _Position2, _Position0 - _Position2) / _AreaFull;

		return _Area0 >= 0 && _Area1 >= 0 && _Area2 >= 0;
	}

	private void CalculateBounds() {
		APIMath.MaxMinFast(_Position0.x, _Position1.x, _Position2.x, ref _XMAXF, ref _XMINF);
		APIMath.MaxMinFast(_Position0.y, _Position1.y, _Position2.y, ref _YMAXF, ref _YMINF);

		_XMAX = (int)(APIContext.viewport.widht * _XMAXF);
		_YMAX = (int)(APIContext.viewport.height * _YMAXF);
		_XMIN = (int)(APIContext.viewport.widht * _XMINF);
		_YMIN = (int)(APIContext.viewport.height * _YMINF);
	}

	private void RasterizeFragment(Vector2 point, int x, int y) {
		VertData o = new VertData();

		o.pos = point;
		o.pos.z = _VertData0.pos.z * _Area1 + _VertData1.pos.z * _Area2 + _VertData2.pos.z * _Area0;

		if (DepthTest(o.pos, x, y)) {
			APITextureBuffer colorBuffer = APIContext.frameBufferManager.frameBufferTarget.colorBuffer;
			Color bColor = colorBuffer.GetPixel(x, y);

			o.uv = _VertData0.uv * _Area1 + _VertData1.uv * _Area2 + _VertData2.uv * _Area0;
			o.normal = _VertData0.normal * _Area0 + _VertData1.normal * _Area1 + _VertData2.normal * _Area2;
			o.color = _VertData0.color * _Area1 + _VertData1.color * _Area2 + _VertData2.color * _Area0;

			Color color = _Shader.Fragment(o);

			colorBuffer.SetPixel(x + APIContext.viewport.x, y + APIContext.viewport.y, Color.Lerp(bColor, color, _Alpha));
		}
	}

	private void Rasterize() {
		float width = (float)APIContext.viewport.widht;
		float height = (float)APIContext.viewport.height;

		_Alpha = 1f;

		for (int y = _YMIN; y < _YMAX; y++) {
			for (int x = _XMIN; x < _XMAX; x++) {
				Vector2 point = new Vector2(x / width, y / height);

				if (CullPoint(point)) {
					RasterizeFragment(point, x, y);
				}
			}
		}
	}

	private void RasterizeMSAA4() {
		float width = (float)APIContext.viewport.widht;
		float height = (float)APIContext.viewport.height;

		for (int y = _YMIN; y < _YMAX; y++) {
			for (int x = _XMIN; x < _XMAX; x++) {
				Vector2 point = new Vector2(x / width, y / height);
				Vector2 point2 = new Vector2(point.x + 0.3f / width, point.y + 0.3f / height);
				Vector2 point3 = new Vector2(point.x + 0.6f / width, point.y + 0.6f / height);
				Vector2 point4 = new Vector2(point.x + 0.6f / width, point.y + 0.3f / height);
				Vector2 point5 = new Vector2(point.x + 0.3f / width, point.y + 0.6f / height);

				bool p1 = CullPoint(point2);
				bool p2 = CullPoint(point3);
				bool p3 = CullPoint(point4);
				bool p4 = CullPoint(point5);

				_Alpha = 0f;

				if (p1)
					_Alpha += 0.25f;

				if (p2)
					_Alpha += 0.25f;

				if (p3)
					_Alpha += 0.25f;

				if (p4)
					_Alpha += 0.25f;

				if (_Alpha > 0) {
					RasterizeFragment(point, x, y);
				}
			}
		}
	}

	private void DrawTriangle() {
		_Position0 = _VertData0.pos;
		_Position1 = _VertData1.pos;
		_Position2 = _VertData2.pos;

		if (!CullFrustrum())
			return;

		if (!CullBackface())
			return;

		CalculateBounds();

		switch (APIContext.states.antiAliasing) {
			case APIAntiAliasing.MSAA4:
				RasterizeMSAA4();
				break;
			default:
				Rasterize();
				break;
		}
	}

	public void DrawTriangles(int offset, int count) {
		APIVertexBuffer vertexBuffer = APIContext.dataBuffer.vertexBuffer;

		for (int i = offset; i < count;) {
			AppData appData = new AppData();

			appData.pos = vertexBuffer.positions[i];
			appData.pos.w = 1f;
			appData.color = vertexBuffer.colors[i];
			appData.uv = vertexBuffer.uvs[i];
			appData.normal = vertexBuffer.normals[i];
			_VertData0 = _Shader.Vertex(appData);
			i++;

			appData.pos = vertexBuffer.positions[i];
			appData.pos.w = 1f;
			appData.color = vertexBuffer.colors[i];
			appData.uv = vertexBuffer.uvs[i];
			appData.normal = vertexBuffer.normals[i];
			_VertData1 = _Shader.Vertex(appData);
			i++;

			appData.pos = vertexBuffer.positions[i];
			appData.pos.w = 1f;
			appData.color = vertexBuffer.colors[i];
			appData.uv = vertexBuffer.uvs[i];
			appData.normal = vertexBuffer.normals[i];
			_VertData2 = _Shader.Vertex(appData);
			i++;

			DrawTriangle();
		}
	}
}
