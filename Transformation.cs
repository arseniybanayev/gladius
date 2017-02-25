using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Gladius
{
	/// <summary>
	/// Provides matrix representations of common transformations in 3D space.
	/// </summary>
	public class Transformation
	{
		/// <summary>
		/// Returns a matrix representing the rotation of a vector about its origin
		/// along the specified plane by the specified number of degrees.
		/// </summary>
		public static Matrix<double> Rotate(Plane plane, double degrees) {
			var radians = DegreesToRadians(degrees);
			double[] elements;
			switch (plane) {
				case Plane.XY:
					elements = new[] {
						Math.Cos(radians), Math.Sin(radians), 0,
						-Math.Sin(radians), Math.Cos(radians), 0,
						0, 0, 1
					};
					break;
				case Plane.XZ:
					elements = new[] {
						Math.Cos(radians), 0, -Math.Sin(radians),
						0, 1, 0,
						Math.Sin(radians), 0, Math.Cos(radians)
					};
					break;
				case Plane.YZ:
					elements = new[] {
						1, 0, 0,
						0, Math.Cos(radians), Math.Sin(radians),
						0, -Math.Sin(radians), Math.Cos(radians)
					};
					break;
				default:
					throw new NotSupportedException($"Plane '{plane}' is not supported");
			}

			return new DenseMatrix(3, 3, elements);
		}

		private static double DegreesToRadians(double degrees) {
			return Math.PI * degrees / 180.0;
		}
	}
}