using Godot;
using System;
public class SphereCoords
{
	// x = r, z = θ, y = ϕ
	public static Vector3 ConvertToCartesian(Vector3 sphere) {
		var vector = new Vector3();
		vector.x = sphere.x * (float)Math.Cos((double)sphere.z) * (float)Math.Sin((double)sphere.y);
		vector.z = sphere.x * (float)Math.Sin((double)sphere.z) * (float)Math.Sin((double)sphere.y);
		vector.y = sphere.x * (float)Math.Cos((double)sphere.y);
		return vector;
	}

	public static Vector3 ConvertToSphere(Vector3 cartesian) {
		var vector = new Vector3();
		vector.x = (float)Math.Sqrt(Math.Pow((double)cartesian.x, 2) + Math.Pow((double)cartesian.y, 2) + Math.Pow((double)cartesian.z, 2));
		vector.y = (float)Math.Atan((double)cartesian.y / (double)cartesian.x);
		vector.z = (float)Math.Acos((double)cartesian.z / (double)vector.x);
		return vector;
	}
	
	public static float Map(float x, float a, float b, float c, float d) {
		return (x - a) / (b - a) * (d - c) + c;
	}
}
