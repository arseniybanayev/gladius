﻿using System;
using SpriteKit;
using CoreGraphics;
using UIKit;

namespace Gladius
{
	public class Point3D
	{
		public static Point3D Zero => new Point3D(0, 0, 0);

		public Point3D(double x, double y, double z) {
			X = x;
			Y = y;
			Z = z;
		}

		public void Add(Vector3D vector) {
			X += vector.X;
			Y += vector.Y;
			Z += vector.Z;
		}

		public void Subtract(Vector3D vector) {
			Add(-vector);
		}

		public Vector3D Subtract(Point3D point) {
			return new Vector3D(X - point.X, Y - point.Y, Z - point.Z);
		}

		public void MoveTo(Point3D point) {
			X = point.X;
			Y = point.Y;
			Z = point.Z;
		}

		public double X { get; private set; }
		public double Y { get; private set; }
		public double Z { get; private set; }

		protected SKNode SkNode;

		public virtual void Draw(GameScene scene) {
			if (SkNode == null) {
				SkNode = new SKSpriteNode(SKTexture.FromImageNamed("Ball"), UIColor.Clear, new CGSize(5, 5));
				scene.AddChild(SkNode);
			}

			SkNode.RunAction(SKAction.MoveTo(new CGPoint(X - (Z / Math.Sqrt(2.0)), Y - (Z / Math.Sqrt(2.0))), 0.5));
		}

		public void Undraw() {
			SkNode?.RemoveFromParent();
		}

		public static Point3D operator +(Point3D point, Vector3D vector) {
			return new Point3D(point.X + vector.X,
							   point.Y + vector.Y,
							   point.Z + vector.Z);
		}

		public static Point3D operator -(Point3D point, Vector3D vector) {
			return point + -vector;
		}
	}

	public enum Plane
	{
		XY,
		XZ,
		YZ
	}

	public class Vector3D
	{
		public static Vector3D Zero => new Vector3D(0, 0, 0);

		public static Vector3D operator -(Vector3D vector) {
			return new Vector3D(-vector.X, -vector.Y, -vector.Z);
		}

		public static Vector3D operator +(Vector3D a, Vector3D b) {
			return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public Vector3D(double x, double y, double z) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3D Add(Vector3D vector) {
			return new Vector3D(
				X + vector.X,
				Y + vector.Y,
				Z + vector.Z);
		}

		public Vector3D Subtract(Vector3D vector) {
			return Add(-vector);
		}

		public Vector3D Rotate(Plane plane, double degrees) {
			switch (plane) {
				case Plane.XY:
					return RotateXY(degrees);
				case Plane.XZ:
					return RotateXZ(degrees);
				case Plane.YZ:
					return RotateYZ(degrees);
				default:
					throw new Exception($"Not a valid plane: {plane}.");
			}
		}

		private Vector3D RotateXY(double degrees) {
			var radians = degrees.ToRadians();
			return new Vector3D(
				Math.Cos(radians) * X - Math.Sin(radians) * Y,
				Math.Sin(radians) * X + Math.Cos(radians) * Y,
				Z);
		}

		private Vector3D RotateXZ(double degrees) {
			var radians = degrees.ToRadians();
			return new Vector3D(
				Math.Cos(radians) * X + Math.Sin(radians) * Z,
				Y,
				-Math.Sin(radians) * X + Math.Cos(radians) * Z);
		}

		private Vector3D RotateYZ(double degrees) {
			var radians = degrees.ToRadians();
			return new Vector3D(
				X,
				Math.Cos(radians) * Y - Math.Sin(radians) * Z,
				Math.Sin(radians) * Y + Math.Cos(radians) * Z);
		}

		public Vector3D Scale(Vector3D scalingVector) {
			return new Vector3D(
				X * scalingVector.X,
				Y * scalingVector.Y,
				Z * scalingVector.Z);
		}

		public virtual double X { get; }
		public virtual double Y { get; }
		public virtual double Z { get; }
	}

	public static class MathExtensions
	{
		public static double ToRadians(this double degrees) {
			return Math.PI * degrees / 180.0;
		}
	}
}