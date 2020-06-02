using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;

namespace CV.UserControls {

    public static class Utilities {

        //Returns true if the string is a numeric key (minus, digit or dot), false otherwise (key filtering)
        public static bool IsNumericKey(string str) {
            return Regex.IsMatch(str, @"[\-\d\.]");
        }

        //Returns true if the string is a real number, false otherwise (text validation)
        public static bool IsNumeric(string str) {
            return Regex.IsMatch(str, @"^(\-?\d+\.?\d*)$");
        }

        //Returns the distance between two points
        public static double DistancePointPoint(Point3D a, Point3D b) {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
        }

        //Returns the closest distance between a plane and a point (distance between a perpendicular segment to the plane the point belongs to)
        public static double DistancePlanePoint(Plane plane, Point3D point) {
            double numerator = Math.Abs(plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D);
            return numerator / Math.Sqrt(Math.Pow(plane.Normal.X, 2) + Math.Pow(plane.Normal.Y, 2) + Math.Pow(plane.Normal.Z, 2));
        }

        //Returns the center of a given rectangular shape
        public static Point3D FindCenter(Rect3D bounds) {
            return new Point3D(bounds.X + bounds.SizeX / 2, bounds.Y + bounds.SizeY / 2, bounds.Z + bounds.SizeZ / 2);
        }

        //Returns the Point3D parsed from a string on this format: ('double', 'double', 'double')
        public static Point3D Str2Point3D(string str) {
            str = str.Trim(new char[] { '(', ')' });
            string[] coordinates = str.Split(new char[] { ',' });
            return new Point3D(double.Parse(coordinates[0]), double.Parse(coordinates[1]), double.Parse(coordinates[2]));
        }

        //Returns a list of Point3D parsed from a string on this format: ('double','double','double'), ('double','double','double'), . . .
        public static List<Point3D> Str2PointList(string str) {
            List<Point3D> list = new List<Point3D>();
            string pattern = @"\),";
            string[] elements = Regex.Split(str, pattern);
            foreach (var element in elements) {
                list.Add(Str2Point3D(element));
            }
            return list;
        }

        //Returns a CVPlaneItem parsed from a string on this format: Plane_1 | {Normal:<'double', 'double', 'double'> D:'double'}
        public static CVPlaneItem Str2PlaneItem(string str) {
            string[] parts = str.Split(new char[] { '|' });
            parts[1] = parts[1].Replace(" ","");
            Match temp = Regex.Match(parts[1], @"<(\-?\d+\.?\d*),(\-?\d+\.?\d*),(\-?\d+\.?\d*)>");
            //Replace < > for ( )
            string normal = temp.Value.Replace('<','('); 
            normal = normal.Replace('>',')');
            temp = Regex.Match(parts[1], @"D:(\-?\d+\.?\d*)");
            string distance = temp.Value.Trim(new char[] { 'D' , ':' , '}' });
            Console.WriteLine(normal+" "+distance);
            return new CVPlaneItem(parts[0], new Plane(MediaVec2NumVec(Str2Point3D(normal)), float.Parse(distance)));
        }

        //Returns a list of CVPlaneItem elements parsed from a string on this format: Plane_1 | {Normal:<'double', 'double', 'double'> D:'double'}, Plane_2 | {Normal:<'double', 'double', 'double'> D:'double'}, . . .
        public static List<CVPlaneItem> Str2PlaneList(string str) {
            List<CVPlaneItem> list = new List<CVPlaneItem>();
            string pattern = @"},";
            string[] elements = Regex.Split(str, pattern);
            foreach(var element in elements) {
                list.Add(Str2PlaneItem(element));
            }
            return list;
        }

        //Returns the corresponding Vector3 parsed from a Point3D
        public static Vector3 MediaVec2NumVec(Point3D vector) {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        //Returns the corresponding Point3D parsed from a Vector3
        public static Point3D NumVec2MediaVec(Vector3 vector) {
            return new Point3D(vector.X, vector.Y, vector.Z);
        }

        //Returns the Euler angle resulting from the difference in directions of two vectors
        public static double GetAngle(Vector3D vector1, Vector3D vector2) {
            return Vector3D.AngleBetween(vector1, vector2);
        }

        //Returns the Euler angle of a given axis and 3D element
        public static double GetAngleRotation(Vector3D axis, Transform3D matrix) {
            return Vector3D.AngleBetween(axis, matrix.Value.Transform(axis));
        }

        //Returns the Euler angle rotation, of a given 3D element, contained in a Vector3D
        public static Vector3D GetAngleRotation(Transform3D matrix) {
            double x = GetAngleRotation(new Vector3D(1, 0, 0), matrix);
            double y = GetAngleRotation(new Vector3D(0, 1, 0), matrix);
            double z = GetAngleRotation(new Vector3D(0, 0, 1), matrix);
            return new Vector3D(x, y, z);
        }

        //Returns the Vector3D transformed by the Quaternion
        public static Vector3D Transform(System.Windows.Media.Media3D.Quaternion qt, Vector3D vector) {
            double x2 = qt.X + qt.X;
            double y2 = qt.Y + qt.Y;
            double z2 = qt.Z + qt.Z;
            double wx2 = qt.W * x2;
            double wy2 = qt.W * y2;
            double wz2 = qt.W * z2;
            double xx2 = qt.X * x2;
            double xy2 = qt.X * y2;
            double xz2 = qt.X * z2;
            double yy2 = qt.Y * y2;
            double yz2 = qt.Y * z2;
            double zz2 = qt.Z * z2;
            double x = vector.X * (1.0 - yy2 - zz2) + vector.Y * (xy2 - wz2) + vector.Z * (xz2 + wy2);
            double y = vector.X * (xy2 + wz2) + vector.Y * (1.0 - xx2 - zz2) + vector.Z * (yz2 - wx2);
            double z = vector.X * (xz2 - wy2) + vector.Y * (yz2 + wx2) + vector.Z * (1.0 - xx2 - yy2);
            return new Vector3D(x, y, z);
        }

        //Returns the Vector3D result of the rotation
        public static Vector3D Rotate(Vector3D vector, Vector3D axis, double angle) {
            System.Windows.Media.Media3D.Quaternion qt = new System.Windows.Media.Media3D.Quaternion(axis, angle);
            return Transform(qt, vector);
        }

        //Modify the two parameters, lookDirection, upDirection, into vectors that makes the camera look at the target from the observer's point of view
        public static void LookAt(Point3D target, Point3D observer, out Vector3D lookDirection, out Vector3D upDirection) {
            lookDirection = target - observer;
            lookDirection.Normalize();

            double a = lookDirection.X;
            double b = lookDirection.Y;
            double c = lookDirection.Z;

            //--- Find the one and only up vector (x, y, z) which has a positive y value (1),
            //--- which is perpendicular to the look vector (2) and and which ensures that
            //--- the resulting roll angle is 0, i.e. the resulting left vector (up cross look)
            //--- lies within the xz-plane (or has a y value of 0) (3). In other words:
            //--- 1. y > 0 (e.g. 1)
            //--- 2. ax + by + cz = 0   ->  i.e. dot product is 0
            //--- 3. az - cx = 0
            //--- If the observer position is right above or below the target point, i.e. a = c = 0 and b != 0,
            //--- we set the up vector to (1, 0, 0) for b > 0 and to (-1, 0, 0) for b < 0.

            double length = (a * a + c * c);
            if (length > 1e-12) {
                upDirection = new Vector3D(-b * a / length, 1, -b * c / length);
                upDirection.Normalize();
            }
            else {
                if (b > 0) upDirection = new Vector3D(0, 0, -1);
                else upDirection = new Vector3D(0, 0, 1);
            }

        }

        //Returns the quaternion rotation of a given 3D element
        public static System.Windows.Media.Media3D.Quaternion GetQuaternionRotation(Transform3D matrix) {
            Vector3D temp = GetAngleRotation(matrix);
            System.Windows.Media.Media3D.Quaternion rotation = new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), temp.X);
            rotation *= new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0), temp.Y);
            rotation *= new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 0, 1), temp.Z);
            return rotation;
        }

        //Returns the quaternion rotation parsed from a Euler angle rotation
        public static System.Windows.Media.Media3D.Quaternion GetQuaternionRotation(Vector3D eulerAngles) {
            System.Windows.Media.Media3D.Quaternion rotation = new System.Windows.Media.Media3D.Quaternion(new Vector3D(1, 0, 0), eulerAngles.X);
            rotation *= new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 1, 0), eulerAngles.Y);
            rotation *= new System.Windows.Media.Media3D.Quaternion(new Vector3D(0, 0, 1), eulerAngles.Z);
            return rotation;
        }
    }
}
