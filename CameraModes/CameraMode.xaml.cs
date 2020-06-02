using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using CV.UserControls;
using System;

namespace CV.CameraModes {

    public abstract class CameraMode : Page {

        protected Point3D targetPosition;

        public CameraMode(PerspectiveCamera cam = null, ObservableCollection<CV3DElement> scene = null) {
            Scene = scene;
            Camera = cam;
            Target = (Scene[0].Object as ModelVisual3D).Content as GeometryModel3D;
            Position = new Point3D(0, 0, 0);
            ZAxis = Camera.LookDirection;
        }

        public ObservableCollection<CV3DElement> Scene { get; set; }

        public PerspectiveCamera Camera { get; set; }

        public static GeometryModel3D Target { get; set; }

        public Point3D Position {
            get { return Camera.Position; }
            set { Camera.Position = value; }
        }

        public Vector3D XAxis {
            get { return Vector3D.CrossProduct(YAxis, ZAxis); }
        }

        public Vector3D YAxis {
            get { return Camera.UpDirection; }
            set {
                value.Normalize();
                Camera.UpDirection = value;
            }
        }

        public Vector3D ZAxis {
            get { return Camera.LookDirection; }
            set {
                value.Normalize();
                Camera.LookDirection = value;
            }
        }

        public double PitchAngle {
            get { return Utilities.GetAngle(new Vector3D(1, 0, 0), XAxis); }
        }

        public double YawAngle {
            get { return Utilities.GetAngle(new Vector3D(0, 1, 0), YAxis); }
        }

        public double RollAngle {
            get { return Utilities.GetAngle(new Vector3D(0, 0, 1), ZAxis); }
        }

        public void ChangeYaw(double angle) { ZAxis = Utilities.Rotate(ZAxis, YAxis, angle); }

        public void ChangeRoll(double angle) { YAxis = Utilities.Rotate(YAxis, ZAxis, angle); }

        public void ChangePitch(double angle) {
            Quaternion qt = new Quaternion(XAxis, angle);
            qt.Normalize();
            YAxis = Utilities.Transform(qt, YAxis);
            ZAxis = Utilities.Transform(qt, ZAxis);
        }

        //Rotate about an axis through the world's origin
        public void Rotate(Vector3D axis, double angle) {
            Quaternion qt = new Quaternion(axis, angle);
            qt.Normalize();
            Position = (Point3D)Utilities.Transform(qt, (Vector3D)Position);
            YAxis = Utilities.Transform(qt, YAxis);
            ZAxis = Utilities.Transform(qt, ZAxis);
        }

        //Rotate about an axis with arbitrary rotation center
        public void Rotate(Vector3D axis, double angle, Point3D center) {
            Position = Point3D.Subtract(Position, (Vector3D)center);
            Rotate(axis, angle);
            Position = Point3D.Add(Position, (Vector3D)center);
        }

        public virtual void UpdateTargetPosition() {
            targetPosition = Utilities.FindCenter(Target.Bounds);
        }

        public virtual void AssignTargetHandler() {
            Target.Changed += CameraTarget_Handler;
        }

        public virtual void Tracker() {
            Vector3D lookDirection = ZAxis;
            Vector3D upDirection = YAxis;
            Utilities.LookAt(targetPosition, Position, out lookDirection, out upDirection);
            ZAxis = lookDirection;
            YAxis = upDirection;
        }

        public abstract void InstantUpdate();

        public virtual void UpdateScene() { }

        public virtual void CameraTarget_Handler(object sender, EventArgs args) {
            UpdateTargetPosition();
        }
    }
}
