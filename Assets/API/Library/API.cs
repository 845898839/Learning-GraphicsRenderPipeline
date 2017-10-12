using UnityEngine;

// 1.Class API, 1
// 2 Struct VertData 1
// 3 Enum APIClear 0.5
// 5 Optional API.DrawTriangle, Named Test 1
// 6 Abstract APIShader, used in Test 1
// 8, 11 APICommandBuffer 2
// 13 APITextureBuffer 3

public enum APIClear {
    COLOR = 0x0001,
    DEPTH = 0x0002,
}

public enum APIDraw {
    TRIANGLES,
}

public enum APIAntiAliasing {
    None,
    MSAA4,
}

public enum APISampleFilter {
    Point,
    Billnear,
}

public struct VertData {
    public Vector4 pos;
    public Vector3 normal;
    public Vector2 uv;
    public Vector4 color;
}

public struct AppData {
    public Vector4 pos;
    public Vector3 normal;
    public Vector2 uv;
    public Vector4 color;
}

public static class API {

    private static APIContext _Context;

    public static void DepthTest(bool value) {
        APIContext.states.depthTest = value;
    }

    public static void AntiAliasing(APIAntiAliasing aa) {
        APIContext.states.antiAliasing = aa;
    }

    public static void RenderTarget(Texture2D texture) {
        APIContext.monitor.SetRenderTarget(texture);
    }

    public static void SetShader(APIShader shader) {
        APIContext.pipeline.SetShader(shader);
    }

    public static void SetPositions(Vector3[] positions) {
        APIContext.dataBuffer.vertexBuffer.positions = positions;
    }

    public static void SetColors(Vector4[] colors) {
        APIContext.dataBuffer.vertexBuffer.colors = colors;
    }

    public static void SetUVs(Vector2[] uvs) {
        APIContext.dataBuffer.vertexBuffer.uvs = uvs;
    }

    public static void SetNormals(Vector3[] normals) {
        APIContext.dataBuffer.vertexBuffer.normals = normals;
    }

    public static void Clear(APIClear flags) {
        APIContext.frameBufferManager.Clear(flags);
    }

    public static void Color(Color color) {
        APIContext.frameBufferManager.defaultColor = color;
    }

    public static void Begin() {
    }

    public static void End() {
        APIContext.monitor.Draw();
        APIContext.frameBufferManager.ChangeBuffer();
    }

    public static void Viewport(int x, int y, int width, int height) {
        APIContext.viewport.Set(x, y, width, height);
    }

    public static void Draw(APIDraw flags, int count, int offset = 0) {
        switch (flags) {
            case APIDraw.TRIANGLES:
                APIContext.pipeline.DrawTriangles(offset, count);
                break;
        }
    }

    public static void Create(int width, int height) {
        _Context = new APIContext(width, height);
    }
}
