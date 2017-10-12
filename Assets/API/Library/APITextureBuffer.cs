using UnityEngine;
using System.Collections;
using System;

public class APITextureBuffer : IEnumerable, IEquatable<APITextureBuffer>, ICloneable {

    private APISampleFilter _SampleFilter = APISampleFilter.Point;
    private Color[] _Pixels;
    private bool _Mipmap;
    private int _Width;
    private int _Height;
    private APITextureBuffer[] _Mipmaps;
    private int _Dimension;

    public APISampleFilter sampleFilter { get { return _SampleFilter; } set { _SampleFilter = value; } }
    public int width { get { return _Width; } }
    public int height { get { return _Height; } }

    private bool CalculateMipmap() {
        if (!Mathf.IsPowerOfTwo(_Width) || !Mathf.IsPowerOfTwo(_Height))
            return false;

        int dimension = Mathf.Min(_Width, _Height);
        int size = APIMath.Log2(dimension);

        _Mipmaps[0] = this;
        _Dimension = dimension;

        for (int i = 1; i < size; i++) {
            dimension >>= 1;
            _Mipmaps[i] = new APITextureBuffer(_Mipmaps[i - 1], dimension, dimension);
        }

        return true;
    }

    public APITextureBuffer(APITextureBuffer texture, int width, int height) {
        Color[] texels = new Color[width * height];

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                texels[j * height + i] = texture.Sample((float)i / width, (float)j / height);
            }
        }

        SetPixels(0, 0, width, height, texels);
        _SampleFilter = texture.sampleFilter;
    }

    public APITextureBuffer(Texture2D texture) {
        SetPixels(0, 0, texture.width, texture.height, texture.GetPixels());
    }

    public APITextureBuffer(int width, int height) {
        _Width = width;
        _Height = height;
        _Pixels = new Color[width * height];
    }

    public void SetColor(Color color) {
        int size = _Pixels.Length;

        for (int i = 0; i < size; i++)
            _Pixels[i] = color;
    }

    public Color SampleMipmap(float x, float y, int width, int height) {
        int size = Mathf.Max(width, height);
        int pf2 = Mathf.ClosestPowerOfTwo(size);
        int index = Mathf.Clamp(_Dimension / pf2, 0, _Mipmaps.Length - 1);

        return _Mipmaps[index].Sample(x, y);
    }

    public Color Sample(float x, float y) {
        //TODO: Use pointer function
        switch (_SampleFilter) {
            case APISampleFilter.Billnear:
                return SampleBilinear(x, y);
            default:
                return SamplePoint(x, y);
        }
    }

    public Color SamplePoint(float x, float y) {
        float fx = (x * _Width);
        float fy = (y * _Height);

        int kx = (int)fx;
        int ky = (int)fy;

        int dx = Mathf.Clamp(kx, 0, _Width);
        int dy = Mathf.Clamp(ky, 0, _Width);

        return _Pixels[dy * _Width + dx];
    }

    //TODO: Optimaize it
    public Color SampleBilinear(float x, float y) {
        float fx = (x * _Width);
        float fy = (y * _Height);

        int kx = (int)fx;
        int ky = (int)fy;

        float tx = fx - kx;
        float ty = fy - ky;

        int w = _Width - 1;
        int h = _Height - 1;

        int dx0 = Mathf.Clamp(kx, 0, w);
        int dy0 = Mathf.Clamp(ky, 0, h);
        int dx1 = Mathf.Clamp(kx + 1, 0, w);
        int dy1 = Mathf.Clamp(ky, 0, h);
        int dx2 = Mathf.Clamp(kx, 0, w);
        int dy2 = Mathf.Clamp(ky + 1, 0, h);
        int dx3 = Mathf.Clamp(kx + 1, 0, w);
        int dy3 = Mathf.Clamp(ky + 1, 0, h);

        Color t0 = _Pixels[dy0 * _Width + dx0];
        Color t1 = _Pixels[dy1 * _Width + dx1];
        Color t2 = _Pixels[dy2 * _Width + dx2];
        Color t3 = _Pixels[dy3 * _Width + dx3];

        return Color.Lerp(Color.Lerp(t0, t1, tx), Color.Lerp(t2, t3, tx), ty);
    }

    //TODO: Add offset x and y
    public void SetPixels(int x, int y, int width, int height, Color[] pixels) {
        _Width = width;
        _Height = height;
        _Pixels = pixels;
    }

    public void SetPixel(int x, int y, Color color) {
        int w = _Width - 1;
        int h = _Height - 1;

        int dx = Mathf.Clamp(x, 0, w);
        int dy = Mathf.Clamp(y, 0, h);

        _Pixels[dy * _Width + dx] = color;
    }

    public Color[] GetPixels() {
        return _Pixels;
    }

    public Color GetPixel(int x, int y) {
        return _Pixels[y * _Width + x];
    }

	public IEnumerator GetEnumerator() {
		return _Pixels.GetEnumerator();
	}

	public bool Equals(APITextureBuffer other) {
		if (_Width != other.width || _Height != other.height)
			return false;

		for (int i = 0; i < _Pixels.Length; i++) {
			if (_Pixels[i] != other._Pixels[i])
				return false;
		}

		return true;
	}

	public object Clone() {
		APITextureBuffer buffer = new APITextureBuffer(this, _Width, _Height);

		return buffer;
	}
}
