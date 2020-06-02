using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using CV.UserControls;

namespace CV.CameraModes {

    public partial class FixedDirectionCM : CameraMode {

        //protected Vector3D fixedYawAxis;

        public FixedDirectionCM(PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) : base(cam, scene) {
            InitializeComponent();

            Init();
        }

        public void Init() {
            UpdateTargetPosition();
            fDir_FixedDirectionCM.Value = new Point3D(7, 7, 7);
            //fixedYawAxis = new Vector3D(0, 1, 0);
            //fAxis_FixedDirectionCM.Value = (Point3D)fixedYawAxis;
            dist_FixedDirectionCM.Value = 3;
            Position = targetPosition - (Vector3D)fDir_FixedDirectionCM.Value * dist_FixedDirectionCM.Value;
            AssignTargetHandler();
            InstantUpdate();
        }

        //SCENE UPDATERS
        public void ChangePosition(Point3D point) {
            Position = point;
        }

        public void ChangeRotation() {
            Tracker();
        }

        public override void InstantUpdate() {
            ChangePosition(targetPosition - (Vector3D)fDir_FixedDirectionCM.Value * dist_FixedDirectionCM.Value);
            ChangeRotation();
        }

        public override void UpdateScene() {
            Point3D currentCameraPos = Position;
            Point3D finalCameraPosNoTightness = targetPosition - (Vector3D)fDir_FixedDirectionCM.Value * dist_FixedDirectionCM.Value;
            Vector3D diff = (finalCameraPosNoTightness - currentCameraPos) * tightness_FixedDirectionCM.Value;
            ChangePosition(currentCameraPos + diff);
            ChangeRotation();
        }

        //HANDLERS
        public void Direction_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }

        /*public void YawAxis_Handler(object sender, RoutedEventArgs args) {
            fixedYawAxis = (Vector3D)fAxis_FixedDirectionCM.Value;
        }*/

        public void Distance_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }
    }
}
