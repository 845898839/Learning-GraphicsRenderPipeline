using UnityEngine;
using System.Collections;

public static class APIMath {

    private static int[] _MultiplyDeBruijnBitPosition2 = 
                                                {
                                                  0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 
                                                  31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
                                                };

    public static float Cross(Vector2 vector1, Vector2 vector2) {
        return vector1.x * vector2.y - vector1.y * vector2.x;
    }

    public static float Max(float value1, float value2, float value3) {
        return Mathf.Max(value1, Mathf.Max(value2, value3));
    }

    public static float Min(float value1, float value2, float value3) {
        return Mathf.Min(value1, Mathf.Min(value2, value3));
    }

    public static void MaxMinFast(float value1, float value2, float value3, ref float max, ref float min) {
        //This algorithm uses Transitive Property into a count a > b && b > c => a > c
        //It takes maximum three steps to find min max O(3) and minimum O(2), with normal min max it takes O(2) + O(2) steps
        //For my specific CPU I seen 4 ms drop

        if (value1 >= value2) {
            if (value1 >= value3) {
                if (value3 >= value2) {
                    max = value1;
                    min = value2;
                }
                else {
                    max = value1;
                    min = value3;
                }
            }
            else {
                max = value3;
                min = value2;
            }
        }
        else {
            if (value2 >= value3) {
                if (value3 >= value1) {
                    max = value2;
                    min = value1;
                }
                else {
                    max = value2;
                    min = value3;
                }
            }
            else {
                max = value3;
                min = value1;
            }
        }
    }

    public static int Log2(int i) {
        //Source http://graphics.stanford.edu/~seander/bithacks.html#IntegerLog
        return _MultiplyDeBruijnBitPosition2[(uint)(i * 0x077CB531U) >> 27];
    }

    public static Color Tex2D(APITextureBuffer textureBuffer, Vector2 uv) {
        return textureBuffer.Sample(uv.x, uv.y);
    }
}
