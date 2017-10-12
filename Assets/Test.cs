using UnityEngine;
using System.Collections;

public class LambertShader : APIShader {

    public APITextureBuffer mainTexture;

    public override VertData Vertex(AppData i) {
        VertData o = new VertData();

        o.pos = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, Time.time * 10, 0), Vector3.one).MultiplyVector(i.pos);
        o.uv = i.uv;
        o.color = i.color;
        o.normal = i.normal;

        return o;
    }

    public override Color Fragment(VertData i) {
        float att = Vector3.Dot(i.normal, new Vector3(0, -1f, 0));

        return att * APIMath.Tex2D(mainTexture, i.uv);
    }
}

public class Test : MonoBehaviour {

    [SerializeField] private Texture2D _Texture;

    private Texture2D _RenderTarget;
    private LambertShader _LambertShader;
	private APICommandBuffer _CommandBuffer = new APICommandBuffer();

	public void TestCommandBuffer() {
		Vector3[] positions = {
								  new Vector3(0.2f, 0.4f, 0.1f),
								  new Vector3(0.5f, 0.8f, 0.5f),
								  new Vector3(0.8f, 0.2f, 0.5f),
							  };
		Vector3[] normals = {
								  new Vector3(1f, 0, 0f),
								  new Vector3(0f, -1f, 0f),
								  new Vector3(-1f, 0, 0f),
							  };
		Vector4[] colors = {
								  Color.red,
								  Color.blue,
								  Color.green,
							  };
		Vector2[] uvs = {
								  new Vector2(0f, 0f),
								  new Vector2(0.5f, 1f),
								  new Vector2(1f, 0f),
							  };

		_CommandBuffer = new APICommandBuffer();

		_CommandBuffer.AddCommand("SetShader", _LambertShader);
		_CommandBuffer.AddCommand("AntiAliasing", APIAntiAliasing.None);
		_CommandBuffer.AddCommand("SetPositions", positions);
		_CommandBuffer.AddCommand("SetColors", colors);
		_CommandBuffer.AddCommand("SetNormals", normals);
		_CommandBuffer.AddCommand("SetUVs", uvs);
		_CommandBuffer.AddCommand("Draw", APIDraw.TRIANGLES, 3, 0);
	}

    internal void Awake() {
        _LambertShader = new LambertShader();
        _RenderTarget = new Texture2D(256, 256);

        GetComponent<Renderer>().material.mainTexture = _RenderTarget;

        APITextureBuffer texture = new APITextureBuffer(_Texture);
        texture.sampleFilter = APISampleFilter.Billnear;

        _LambertShader.mainTexture = texture;

        API.Create(256, 256);
        API.Color(Color.blue);
        API.Viewport(0, 0, 256, 256);
        API.RenderTarget(_RenderTarget);
        API.DepthTest(false);

		TestCommandBuffer();
    }

    internal void Update() {
        API.Clear(APIClear.COLOR | APIClear.DEPTH);

        Vector3[] positions = {
                                  new Vector3(0.2f, 0.4f, 0.1f),
                                  new Vector3(0.5f, 0.8f, 0.5f),
                                  new Vector3(0.8f, 0.2f, 0.5f),
                              };
        Vector3[] normals = {
                                  new Vector3(1f, 0, 0f),
                                  new Vector3(0f, -1f, 0f),
                                  new Vector3(-1f, 0, 0f),
                              };
        Vector4[] colors = {
                                  Color.red,
                                  Color.blue,
                                  Color.green,
                              };
        Vector2[] uvs = {
                                  new Vector2(0f, 0f),
                                  new Vector2(0.5f, 1f),
                                  new Vector2(1f, 0f),
                              };
        API.Begin();
		/*API.AntiAliasing(APIAntiAliasing.None);
        API.SetShader(_LambertShader);
        API.SetPositions(positions);
        API.SetColors(colors);
        API.SetNormals(normals);
        API.SetUVs(uvs);
        API.Draw(APIDraw.TRIANGLES, 3);*/

		_CommandBuffer.InvokeCommands();

        positions[0] = new Vector3(0.2f, 0.2f, 0.5f);
        positions[1] = new Vector3(0.5f, 0.9f, 0.5f);
        positions[2] = new Vector3(0.8f, 0.4f, 0.1f);

        API.SetShader(_LambertShader);
        API.AntiAliasing(APIAntiAliasing.MSAA4);
        API.SetPositions(positions);
        API.SetColors(colors);
        API.SetNormals(normals);
        API.SetUVs(uvs);
		API.Draw(flags: APIDraw.TRIANGLES, count: 3);
        API.End();
    }
}