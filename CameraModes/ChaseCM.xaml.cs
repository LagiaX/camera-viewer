using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using CV.UserControls;

namespace CV.CameraModes {
    
    public partial class ChaseCM : CameraMode {

        protected Stopwatch chrono;
        protected Vector3D fixedYawAxis;
        protected Vector3D lastRotation;

        public ChaseCM(PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) : base(cam, scene) {
            InitializeComponent();

            Init();
        }

        public void Init() {
            UpdateTargetPosition();
            rPos_ChaseCM.Value = new Point3D(0, 2, 3);
            fixedYawAxis = new Vector3D(0, 1, 0);
            fixedAxis_ChaseCM.Value = (Point3D)fixedYawAxis;
            rOffset_ChaseCM.Value = new Point3D(0, 0, 0);
            chrono = new Stopwatch();
            Position = rPos_ChaseCM.Value;
            lastRotation = (Vector3D)rOffset_ChaseCM.Value;
            AssignTargetHandler();
            InstantUpdate();
        }

        //SCENE UPDATERS
        public void ChangePosition(Point3D point) {
            Position = point;
        }

        public void ChangeRotation(Vector3D angleRotation, bool fixedAxis = true) {
            if (fixedYawAxis != new Vector3D(0, 0, 0)) {
                ChangePitch(angleRotation.X - lastRotation.X);
                ChangeRoll(angleRotation.Z - lastRotation.Z);

                if (!fixedAxis) ChangeYaw(angleRotation.Y - lastRotation.Y); //Rotate(fixedYawAxis, angleRotation.Y - lastRotation.Y); 

                lastRotation = angleRotation;
            }
        }

        public override void InstantUpdate() {
            try {
                Point3D rPos = rPos_ChaseCM.Value;
                Point3D pointTransformed = rPos * Target.Transform.Value;
                ChangePosition(targetPosition + (Vector3D)pointTransformed);
                if (freeAxis_ChaseCM.IsChecked.Value == false) { //Strict chase
                    Tracker();
                }
                else { //Free axis chase
                    //Tracker();
                    ChangeRotation((Vector3D)rOffset_ChaseCM.Value, false);
                }
            } catch (Exception e) { Console.WriteLine(e.Message+"\n"+e.StackTrace); }
            
        }

        public override void UpdateScene() {
            Point3D rPos = rPos_ChaseCM.Value;
            Point3D pointTransformed = rPos * Target.Transform.Value;
            Point3D cameraFinalPosNoTightness = targetPosition + (Vector3D)pointTransformed;

            if (true/*fStep_ChaseCM.IsChecked.Value == false*/) {
                Vector3D diff = (cameraFinalPosNoTightness - Position) * tightness_ChaseCM.Value;
                ChangePosition(Position + diff);
            }
            else {
                /* EXPERIMENTAL CALCULATION TO AVOID JITTERING, APPARENTLY
                rTime_ChaseCM.Value += chrono.ElapsedMilliseconds;
                int steps = (int)(rTime_ChaseCM.Value / deltaV_ChaseCM.Value);
                double finalTime = steps * deltaV_ChaseCM.Value;
                Quaternion currentRotation = new Quaternion();
                Matrix3DConverter matrixConverter = new Matrix3DConverter();
                currentRotation = matrixConverter.ConvertTo((AxisAngleRotation3D)camera.Transform.Value, typeof(Quaternion));
                double finalPercentage = finalTime / rTime_ChaseCM.Value;
                Point3D finalPos = camera.Position + ((cameraFinalPosNoTightness - camera.Position) * finalPercentage);
                Quaternion finalRotation = Quaternion.Slerp(currentRotation, cameraTarget.Transform.Value, finalPercentage);
                Point3D intermediatePos = camera.Position;
                Quaternion intermediateRot = new Quaternion(camera.Transform.Value);
                */
            }
            //ChangeRotation(Utilities.GetAngleRotation(Target.Transform));
            if (freeAxis_ChaseCM.IsChecked.Value == false) { //Strict chase
                Tracker();
            }
            else { //Free axis chase
                ChangeRotation((Vector3D)rOffset_ChaseCM.Value, false);
            }
            chrono.Restart();
        }

        //HANDLERS
        public void Position_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }

        public void Rotation_Handler(object sender, RoutedEventArgs args) {
            ChangeRotation((Vector3D)rOffset_ChaseCM.Value, false);
        }

        public void SetYawAxis_Handler(object sender, RoutedEventArgs args) {
            fixedYawAxis = (Vector3D)fixedAxis_ChaseCM.Value;
        }

        public void FreeAxisChecked_Handler(object sender, RoutedEventArgs args) {
            fixedAxis_ChaseCM.IsEnabled = false;
            rOffset_ChaseCM.IsEnabled = true;
        }

        public void FreeAxisUnchecked_Handler(object sender, RoutedEventArgs args) {
            fixedAxis_ChaseCM.IsEnabled = true;
            rOffset_ChaseCM.IsEnabled = false;
        }
    }
}
