using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using CV.UserControls;

namespace CV.CameraModes {

    public partial class ThroughTargetCM : CameraMode {

        protected GeometryModel3D focus;
        protected Point3D focusPosition;
        //protected Vector3D fixedYawAxis;

        public ThroughTargetCM(PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) : base(cam, scene) {
            InitializeComponent();

            Init();
        }

        public void Init() {
            UpdateTargetPosition();
            //fixedYawAxis = new Vector3D(0, 1, 0);
            //fAxis_ThroughTargetCM.Value = (Point3D)fixedYawAxis;
            fPos_ThroughTargetCM.Value = new Point3D(0, 1, -1);
            margin_ThroughTargetCM.Value = 3.0;
            Position = Camera.Position;
            ////////////
            if ((Scene as ObservableCollection<CV3DElement>).Count > 1) {
                UpdateFocus();
                UpdateFocusPosition();
                focus.Changed += CameraFocus_Handler;
            }
            ////////////
            Scene.CollectionChanged += ListChanged_Handler;
            AssignTargetHandler();
            InstantUpdate();
        }

        public void UpdateFocusPosition() {
            if (focus != null) focusPosition = Utilities.FindCenter(focus.Bounds);
        }

        public void UpdateFocus() {
            focus = (Scene[1].Object as ModelVisual3D).Content as GeometryModel3D;
        }

        //SCENE UPDATERS
        public void ChangePosition(Point3D point) {
            Position = point;
        }

        public void ChangeRotation() {
            Tracker();
        }

        public override void Tracker() {
            Vector3D lookDirection = ZAxis;
            Vector3D upDirection = YAxis;
            if (iControl_ThroughTargetCM.IsChecked.Value == false)
                Utilities.LookAt(focusPosition, targetPosition, out lookDirection, out upDirection);
            else
                Utilities.LookAt(targetPosition, focusPosition, out lookDirection, out upDirection);
            ZAxis = lookDirection;
            YAxis = upDirection;
        }

        public override void InstantUpdate() {
            Vector3D focusToTarget = targetPosition - focusPosition;
            focusToTarget.Normalize();
            if (iControl_ThroughTargetCM.IsChecked.Value == false) { //Inverse off
                ChangePosition(targetPosition + focusToTarget * margin_ThroughTargetCM.Value);
            }
            else { //Inverse on
                ChangePosition(focusPosition + focusToTarget * -margin_ThroughTargetCM.Value);
            }
            ChangeRotation();
        }

        public override void UpdateScene() {
            if (Target != null) {
                InstantUpdate();
            }
        }

        //HANDLERS
        public void CameraFocus_Handler(object sender, EventArgs args) {
            UpdateFocusPosition();
        }

        public void ListChanged_Handler(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
            try {
                if ((sender as ObservableCollection<CV3DElement>).Count > 1) {
                    UpdateFocus();
                    UpdateFocusPosition();
                    focus.Changed += CameraFocus_Handler;
                }
                else focus.Changed -= CameraFocus_Handler;
            } catch (Exception e) { Console.WriteLine(e.Message + "\n" + e.StackTrace); }
            
        }

        /*public void YawAxis_Handler(object sender, RoutedEventArgs args) {
            fixedYawAxis = (Vector3D)fAxis_ThroughTargetCM.Value;
        }*/

        public void Focus_Handler(object sender, RoutedEventArgs args) {
            focusPosition = fPos_ThroughTargetCM.Value;
            InstantUpdate();
        }

        public void Margin_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }

        public void InvToggle_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }
    }
}
