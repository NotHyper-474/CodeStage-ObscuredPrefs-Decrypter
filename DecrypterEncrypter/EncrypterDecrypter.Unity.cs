using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStage_Decrypter
{
	public static partial class EncrypterDecrypter
	{
		public struct Vector2Int
		{
			public int x;
			public int y;

			public override string ToString()
			{
				return '(' + x + ", " + y + ')';
			}
		}

		public struct Vector3Int
		{
			public int x;
			public int y;
			public int z;

			public override string ToString()
			{
				return '(' + x + ", " + y + ", " + z + ')';
			}
		}

		public struct Vector2
		{
			public Vector2(float x, float y)
			{
				this.x = x;
				this.y = y;
			}

			public float x;
			public float y;

			public override string ToString()
			{
				return '(' + x + ", " + y + ')';
			}
		}

		public struct Vector3
		{
			public Vector3(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public float x;
			public float y;
			public float z;

			public override string ToString()
			{
				return '(' + x + ", " + y + ", " + z + ')';
			}
		}

		public struct Vector4
		{
			public Vector4(float x, float y, float z, float w)
			{
				this.x = x;
				this.y = y;
				this.z = z;
				this.w = w;
			}

			public float w;
			public float x;
			public float y;
			public float z;

			public override string ToString()
			{
				return '(' + x + ", " + y + ", " + z + ", " + w + ')';
			}
		}

		public struct Quaternion
		{
			public Quaternion(float x, float y, float z, float w)
			{
				this.x = x;
				this.y = y;
				this.z = z;
				this.w = w;
			}

			public float w;
			public float x;
			public float y;
			public float z;

			public override string ToString()
			{
				return '(' + x + ", " + y + ", " + z + ", " + w + ')';
			}
		}

		public struct Rect
		{
			public Rect(float x, float y, float width, float height)
			{
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;
			}

			public float x;
			public float y;
			public float width;
			public float height;

			public override string ToString()
			{
				return '(' + x + ", " + y + ", " + width + ", " + height + ')';
			}
		}

		public struct Color
		{
			public float r;
			public float g;
			public float b;
			public float a;

			public override string ToString()
			{
				return '(' + r + ", " + g + ", " + b + ", " + a + ')';
			}
		}

		public struct Color32
		{
			public Color32(byte r, byte g, byte b, byte a)
			{
				this.r = r;
				this.g = g;
				this.b = b;
				this.a = a;
			}

			public byte r;
			public byte g;
			public byte b;
			public byte a;

			public override string ToString()
			{
				return "(R:" + r + ", G:" + g + ", B:" + b + ", A:" + a + ')';
			}
		}
	}
}
