using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using CV.UserControls;

namespace CV.CameraModes {
    
    public partial class AttachedCM : CameraMode {

        protected double fieldOfView;
        protected Vector3D lastRotation;

        public AttachedCM(PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) : base(cam, scene) {
            InitializeComponent();
            
            Init();
        }

        public void Init() {
            UpdateTargetPosition();
            rPos_AttachedCM.Value = new Point3D(0, 0, 0);
            rRot_AttachedCM.Value = new Point3D(0, 0, 0);
            AssignObjects(Scene);
            objAtt_AttachedCM.ItemsSource = Scene;
            objAtt_AttachedCM.SelectedIndex = 0;
            fieldOfView = Camera.FieldOfView;
            Position = targetPosition + (Vector3D)rPos_AttachedCM.Value;
            lastRotation = (Vector3D)rRot_AttachedCM.Value;
            AssignTargetHandler();
            InstantUpdate();
        }

        public void AssignObjects(Collection<CV3DElement> list) {
            Scene = list as ObservableCollection<CV3DElement>;
        }

        //SCENE UPDATERS
        public void ChangePosition(Point3D point) {
            Position = point;
        }

        public void ChangeRotation(Vector3D angleRotation) {
            ChangePitch(angleRotation.X - lastRotation.X);
            ChangeRoll(angleRotation.Z - lastRotation.Z);
            ChangeYaw(angleRotation.Y - lastRotation.Y);
            lastRotation = angleRotation;
        }

        public override void InstantUpdate() {
            ChangePosition(targetPosition + (Vector3D)rPos_AttachedCM.Value);
            ChangeRotation(Utilities.GetAngleRotation(Target.Transform) + (Vector3D)rRot_AttachedCM.Value);
        }

        public override void UpdateScene() {
            InstantUpdate();
        }

        //HANDLERS
        public void Position_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }

        public void Rotation_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }

        public void ObjectChanged_Handler(object sender, SelectionChangedEventArgs args) {
            Target = ((objAtt_AttachedCM.SelectedItem as CV3DElement).Object as ModelVisual3D).Content as GeometryModel3D;
            targetPosition = Utilities.FindCenter(Target.Bounds);
            InstantUpdate();
        } 

        public void CharVision_Handler(object sender, RoutedEventArgs args) {
            if (Camera.FieldOfView == 0) Camera.FieldOfView += fieldOfView;
            else if (Camera.FieldOfView > 0) Camera.FieldOfView -= fieldOfView;
        }
    }
}
