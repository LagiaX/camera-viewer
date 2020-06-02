using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Media3D;
using CV.UserControls;

namespace CV.CameraModes {

    public partial class FixedCM : CameraMode {

        protected List<Point3D> posList;
        protected Vector3D lastRotation;

        public FixedCM(PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) : base(cam, scene) {
            InitializeComponent();
            
            Init();
        }

        public void Init() {
            UpdateTargetPosition();
            fixedPos_FixedCM.Value = new Point3D(0, 1, -5);
            //System.Console.WriteLine(PitchAngle +" "+ YawAngle +" "+ RollAngle+"\n"+Utilities.GetAngleRotation(Camera.Transform));
            lastRotation = Utilities.GetAngleRotation(Camera.Transform);
            fixedRot_FixedCM.Value = (Point3D)Utilities.GetAngleRotation(Camera.Transform);//new Point3D(PitchAngle,YawAngle,RollAngle);
            posList = new List<Point3D>() { new Point3D(5, 5, 5) };
            posList_FixedCM.DataContext = posList;
            posList_FixedCM.SelectedIndex = 0;
            Position = fixedPos_FixedCM.Value;
            AssignTargetHandler();
            InstantUpdate();
        }

        public void AddPosition(Point3D point) {
            posList.Add(point);
        }

        public void DeletePosition(Point3D point) {
            posList.Remove(point);
        }

        public void AssignList(List<Point3D> list) {
            posList_FixedCM.DataContext = null;
            posList = list;
            posList_FixedCM.DataContext = posList;
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
            if (autoTracking_FixedCM.IsChecked.Value == false && closestCamera_FixedCM.IsChecked.Value == false) { //Fixed camera mode
                ChangeRotation((Vector3D)fixedRot_FixedCM.Value);
            }
            else if (autoTracking_FixedCM.IsChecked.Value == true && closestCamera_FixedCM.IsChecked.Value == false) { //Fixed Tracking camera mode
                ChangePosition(fixedPos_FixedCM.Value);
                Tracker();
            }
            else if (closestCamera_FixedCM.IsChecked.Value == true) { //Closest to Target camera mode
                if (Target != null && posList.Count > 0) {
                    double minDistance = double.MaxValue;
                    Point3D closestPos = posList[0];
                    int index = 0;
                
                    foreach (Point3D cameraPos in posList) {
                        double distance = Utilities.DistancePointPoint(cameraPos, targetPosition);
                        if (distance < minDistance) {
                            closestPos = cameraPos;
                            minDistance = distance;
                            index = posList.IndexOf(cameraPos);
                        }
                    }
                    ChangePosition(closestPos);
                    posList_FixedCM.SelectedIndex = index;
                    Tracker();
                }
            }
        }

        public override void UpdateScene() {
            InstantUpdate();
        }

        //HANDLERS
        public void Position_Handler(object sender, RoutedEventArgs args) {
            if (closestCamera_FixedCM.IsChecked == false)
                ChangePosition(fixedPos_FixedCM.Value);
        }

        public void Rotation_Handler(object sender, RoutedEventArgs args) {
            ChangeRotation((Vector3D)fixedRot_FixedCM.Value);
        }
        
        public void AutoTracking_Handler(object sender, RoutedEventArgs args) {
            if ((sender as CVCheckBox).IsChecked.Value == true) {
                fixedRot_FixedCM.IsEnabled = false;
                Tracker();
            }
            else if ((sender as CVCheckBox).IsChecked.Value == false)
                fixedRot_FixedCM.IsEnabled = true;
        }

        public void ClosestCamChecked_Handler(object sender, RoutedEventArgs args) {
            fixedRot_FixedCM.IsEnabled = false;
            autoTracking_FixedCM.IsChecked = true;
            autoTracking_FixedCM.IsEnabled = false;
        }

        public void ClosestCamUnchecked_Handler(object sender, RoutedEventArgs args) {
            fixedRot_FixedCM.IsEnabled = true;
            autoTracking_FixedCM.IsEnabled = true;
        }

        public void AddPosition_Handler(object sender, RoutedEventArgs args) {
            posList_FixedCM.DataContext = null;
            AddPosition(fixedPos_FixedCM.Value);
            posList_FixedCM.DataContext = posList;
            posList_FixedCM.SelectedIndex = posList.IndexOf(fixedPos_FixedCM.Value);
        }

        public void DeletePosition_Handler(object sender, RoutedEventArgs args) {
            if (posList_FixedCM.SelectedItem != null) {
                Point3D point = (Point3D)posList_FixedCM.SelectedItem;
                posList_FixedCM.DataContext = null;
                DeletePosition(point);
                posList_FixedCM.DataContext = posList;
                posList_FixedCM.SelectedIndex = 0;
            }
        }
    }
}
