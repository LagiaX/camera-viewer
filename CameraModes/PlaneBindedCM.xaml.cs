using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using CV.UserControls;

namespace CV.CameraModes {
    
    public partial class PlaneBindedCM : CameraMode {

        protected Stopwatch chrono;
        protected ObservableCollection<CVPlaneItem> planeList;
        protected Plane plan;
        protected double lastPitch;
        protected Vector3D LeftAxis;
        protected Vector3D UpAxis;
        protected Vector3D HeightAxis;
        protected double heightDisp;
        protected double lateralDisp;
        protected double verticalDisp;

        public PlaneBindedCM(PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) : base(cam, scene) {
            InitializeComponent();
            
            Init();
        }

        public void Init() {
            chrono = new Stopwatch();
            UpdateTargetPosition();
            planeList = new ObservableCollection<CVPlaneItem>();
            AddPlane("From above", new Vector3(0, 10, 0), new Vector3(5, 10, 0), new Vector3(0, 10, 5));
            AddPlane("From below", new Vector3(0, -10, 0), new Vector3(-5, -10, 0), new Vector3(0, -10, 5));
            AddPlane("Across view", new Vector3(2, 0, -7), new Vector3(-1, 0, -2), new Vector3(-1, 5, -2));
            plane_PlaneBindedCM.ItemsSource = planeList;
            plane_PlaneBindedCM.SelectedIndex = 0;
            plan = Plane.CreateFromVertices(new Vector3(1, 20, 0), new Vector3(-1, 20, 0), new Vector3(1, 20, -1));
            Position = Camera.Position;
            LeftAxis = new Vector3D();
            lastPitch = pitch_PlaneBindedCM.Value = 0;
            AssignTargetHandler();
            InstantUpdate();
        }

        public void AddPlane(string name, Vector3 p1, Vector3 p2, Vector3 p3) {
            planeList.Add(new CVPlaneItem(name, Plane.CreateFromVertices(p1, p2, p3)));
        }

        public void AssignList(List<CVPlaneItem> list) {
            plane_PlaneBindedCM.SelectedItem = null;
            plane_PlaneBindedCM.ItemsSource = planeList = new ObservableCollection<CVPlaneItem>(list);;
            plane_PlaneBindedCM.SelectedIndex = 0;
        }

        public void GoLeft(double percentage = 1.0) {
            lateralDisp += moveF_PlaneBindedCM.Value * percentage;
        }

        public void GoRight(double percentage = 1.0) {
            GoLeft(-percentage);
        }

        public void GoUp(double percentage = 1.0) {
            verticalDisp += moveF_PlaneBindedCM.Value * percentage;
        }

        public void GoDown(double percentage = 1.0) {
            GoUp(-percentage);
        }

        //SCENE UPDATERS
        public void ChangePosition(Point3D point) {
            Position = Camera.Position = point;
        }

        public void SetPitch(Vector3D axis, double angle) { //90 Pitch (maximum) means the camera is facing perpendicularly to the game floor, 0 Pitch would be the minimum
            Rotate(axis, angle - lastPitch, Position);
            lastPitch = angle;
        }

        public override void InstantUpdate() {
            if (plane_PlaneBindedCM.SelectedItem != null) {
                if (pView_PlaneBindedCM.IsChecked.Value == false && rtsMode_PlaneBindedCM.IsChecked.Value == false) { //Plane binded mode
                    Plane plane = (plane_PlaneBindedCM.SelectedItem as CVPlaneItem).Plane;
                    double distance = Utilities.DistancePlanePoint(plane, targetPosition);
                    Vector3 perpendicular = plane.Normal * (float)distance;
                    ChangePosition(targetPosition - (Vector3D)Utilities.NumVec2MediaVec(perpendicular));
                    Tracker();
                }
                else if (pView_PlaneBindedCM.IsChecked.Value == true && rtsMode_PlaneBindedCM.IsChecked.Value == false) { //Plan view mode (bird view)
                    double distance = Utilities.DistancePlanePoint(plan, targetPosition);
                    Vector3 perpendicular = Vector3.Normalize(plan.Normal) * (float)distance;
                    Camera.Position = targetPosition - (Vector3D)Utilities.NumVec2MediaVec(perpendicular);
                    Point3D zoomedPos = new Point3D(Camera.Position.X, Camera.Position.Y * zoom_PlaneBindedCM.Value, Camera.Position.Z);
                    ChangePosition(zoomedPos);
                    Tracker();
                }
                else if (rtsMode_PlaneBindedCM.IsChecked.Value == true) { //RTS mode
                    Vector3D displacement = LeftAxis * lateralDisp + UpAxis * verticalDisp + HeightAxis * heightDisp;
                    Camera.Position += displacement;
                    lateralDisp = 0.0;
                    verticalDisp = 0.0;
                    heightDisp = 0.0;
                }
            }
        }

        public override void UpdateScene() {
            try {
                if (plane_PlaneBindedCM.SelectedItem != null) {
                    if (pView_PlaneBindedCM.IsChecked.Value == false && rtsMode_PlaneBindedCM.IsChecked.Value == false) { //Plane binded mode
                        //Normal plane calculation: camera look direction following the target perpendicular to the plane
                        Plane plane = (plane_PlaneBindedCM.SelectedItem as CVPlaneItem).Plane;
                        double distance = Utilities.DistancePlanePoint(plane, targetPosition);
                        Vector3 perpendicular = Vector3.Normalize(plane.Normal) * (float)distance;
                        Point3D cameraFinalPositionNoTightness = targetPosition - (Vector3D)Utilities.NumVec2MediaVec(perpendicular);
                        Vector3D posDiffNoTightness = (cameraFinalPositionNoTightness - Camera.Position);
                        Vector3D diff = new Vector3D(posDiffNoTightness.X * tightness_PlaneBindedCM.Value, posDiffNoTightness.Y, posDiffNoTightness.Z * tightness_PlaneBindedCM.Value);
                        ChangePosition(Camera.Position + diff);
                        //Tracker();
                    }
                    else if (pView_PlaneBindedCM.IsChecked.Value == true && rtsMode_PlaneBindedCM.IsChecked.Value == false) { //Plan view mode (bird view)
                        double distance = Utilities.DistancePlanePoint(plan, targetPosition);
                        Vector3 perpendicular = Vector3.Normalize(plan.Normal) * (float)distance;
                        Point3D cameraFinalPositionNoTightness = targetPosition - (Vector3D)Utilities.NumVec2MediaVec(perpendicular);
                        Vector3D posDiffNoTightness = (cameraFinalPositionNoTightness - Camera.Position);
                        Vector3D diff = new Vector3D(posDiffNoTightness.X * tightness_PlaneBindedCM.Value, posDiffNoTightness.Y, posDiffNoTightness.Z * tightness_PlaneBindedCM.Value);
                        Camera.Position += diff;
                        Point3D zoomedPos = new Point3D(Camera.Position.X, Camera.Position.Y * zoom_PlaneBindedCM.Value, Camera.Position.Z);
                        ChangePosition(zoomedPos);
                        //Tracker();
                    }
                    else if (rtsMode_PlaneBindedCM.IsChecked.Value == true) { //RTS mode
                        if (leftAxis_PlaneBindedCM.IsEnabled == false && upAxis_PlaneBindedCM.IsEnabled == false) { //Ref system choosen and confirmed
                            Vector3D displacement = (LeftAxis * lateralDisp + UpAxis * verticalDisp + HeightAxis * heightDisp) * chrono.ElapsedMilliseconds * moveF_PlaneBindedCM.Value;
                            disp_PlaneBindedCM.Value = Camera.Position += displacement;
                            lateralDisp = 0.0;
                            verticalDisp = 0.0;
                            heightDisp = 0.0;
                        }
                    }
                }
            } catch (Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
            chrono.Restart();
        }

        //HANDLERS
        public void PlaneChange_Handler(object sender, SelectionChangedEventArgs args) {
            if (plane_PlaneBindedCM.HasItems && plane_PlaneBindedCM.SelectedItem != null) {
                Plane plane = (plane_PlaneBindedCM.SelectedItem as CVPlaneItem).Plane;
                Camera.UpDirection = (Vector3D)Utilities.NumVec2MediaVec(plane.Normal); //What's the neutral position for a plane binded camera again?
                InstantUpdate();
            }
        }

        public void AddPlane_Handler(object sender, RoutedEventArgs args) {
            UserControls.Dialogues.AddPlane inputDialog = new UserControls.Dialogues.AddPlane();
            if (inputDialog.ShowDialog() == true) {
                planeList.Add(new CVPlaneItem(inputDialog.PlaneName, inputDialog.Plane));
            }
        }

        public void RemovePlane_Handler(object sender, RoutedEventArgs args) {
            try {
                if (plane_PlaneBindedCM.Items.Count > 1) {
                    planeList.Remove(plane_PlaneBindedCM.SelectedItem as CVPlaneItem);
                    plane_PlaneBindedCM.SelectedIndex = 0;
                }
            } catch (Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void PViewChecked_Handler(object sender, RoutedEventArgs args) {
            plane_PlaneBindedCM.IsEnabled = false;
            addPlane_PlaneBindedCM.IsEnabled = false;
            removePlane_PlaneBindedCM.IsEnabled = false;
            zoom_PlaneBindedCM.IsEnabled = true;
            InstantUpdate();
        }

        public void PViewUnchecked_Handler(object sender, RoutedEventArgs args) {
            plane_PlaneBindedCM.IsEnabled = true;
            addPlane_PlaneBindedCM.IsEnabled = true;
            pitch_PlaneBindedCM.IsEnabled = false;
            zoom_PlaneBindedCM.IsEnabled = false;
        }

        public void Zoom_Handler(object sender, RoutedEventArgs args) {
            InstantUpdate();
        }

        public void Pitch_Handler(object sender, RoutedEventArgs args) {
            try {
                if (pView_PlaneBindedCM.IsChecked.Value == true) {
                    SetPitch(LeftAxis, pitch_PlaneBindedCM.Value);
                }
            } catch (Exception e) { Console.WriteLine(e.Message+"\n"+e.StackTrace); }
        }

        public void RTSModeChecked_Handler(object sender, RoutedEventArgs args) {
            pView_PlaneBindedCM.IsChecked = true;
            pView_PlaneBindedCM.IsEnabled = false;
            leftAxis_PlaneBindedCM.IsEnabled = true;
            upAxis_PlaneBindedCM.IsEnabled = true;
            confirmSys_PlaneBindedCM.IsEnabled = true;
        }

        public void RTSModeUnchecked_Handler(object sender, RoutedEventArgs args) {
            resetSys_PlaneBindedCM.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); //Reset reference system
            pView_PlaneBindedCM.IsEnabled = true;
            leftAxis_PlaneBindedCM.IsEnabled = false;
            upAxis_PlaneBindedCM.IsEnabled = false;
            confirmSys_PlaneBindedCM.IsEnabled = false;
            resetSys_PlaneBindedCM.IsEnabled = false;
            pitch_PlaneBindedCM.IsEnabled = false;
            disp_PlaneBindedCM.IsEnabled = false;
            resetDisp_PlaneBindedCM.IsEnabled = false;
            moveF_PlaneBindedCM.IsEnabled = false;
        }

        public void RefSystemConfirm_Handler(object sender, RoutedEventArgs args) {
            if (leftAxis_PlaneBindedCM.Value != new Vector3D(0, 0, 0) && upAxis_PlaneBindedCM.Value != new Vector3D(0, 0, 0)) {
                Vector3D temp = new Vector3D(1, 1, 1);
                temp = temp - leftAxis_PlaneBindedCM.Value - upAxis_PlaneBindedCM.Value;
                LeftAxis = leftAxis_PlaneBindedCM.Value;
                UpAxis = upAxis_PlaneBindedCM.Value;
                HeightAxis = temp;
                leftAxis_PlaneBindedCM.IsEnabled = false;
                upAxis_PlaneBindedCM.IsEnabled = false;
                confirmSys_PlaneBindedCM.IsEnabled = false;
                resetSys_PlaneBindedCM.IsEnabled = true;
                disp_PlaneBindedCM.IsEnabled = true;
                resetDisp_PlaneBindedCM.IsEnabled = true;
                pitch_PlaneBindedCM.IsEnabled = true;
                moveF_PlaneBindedCM.IsEnabled = true;
                YAxis = HeightAxis;
                ZAxis = UpAxis;
                SetPitch(LeftAxis, 90);
                disp_PlaneBindedCM.Value = Camera.Position;
                mainGrid_PlaneBindedCM.KeyDown += CameraDisplacement_Handler;
            }
        }

        public void RefSystemReset_Handler(object sender, RoutedEventArgs args) {
            leftAxis_PlaneBindedCM.Value = new Vector3D(0, 0, 0);
            upAxis_PlaneBindedCM.Value = new Vector3D(0, 0, 0);
            leftAxis_PlaneBindedCM.IsEnabled = true;
            upAxis_PlaneBindedCM.IsEnabled = true;
            confirmSys_PlaneBindedCM.IsEnabled = true;
            //rtsMode_PlaneBindedCM.RaiseEvent(new RoutedEventArgs(ToggleButton.UncheckedEvent));
            //rtsMode_PlaneBindedCM.RaiseEvent(new RoutedEventArgs(ToggleButton.CheckedEvent));
            mainGrid_PlaneBindedCM.KeyDown -= CameraDisplacement_Handler;
        }

        public void Displacement_Handler(object sender, RoutedEventArgs args) {
            Vector3D displacement = LeftAxis * disp_PlaneBindedCM.Value.X + UpAxis * disp_PlaneBindedCM.Value.Z + HeightAxis * disp_PlaneBindedCM.Value.Y;
            Camera.Position = (Point3D)displacement;
        }

        public void DispReset_Handler(object sender, RoutedEventArgs args) {
            disp_PlaneBindedCM.Value = new Point3D(0, 0, 0);
        }

        public void GetFocus_Handler(object sender, EventArgs args) {
            (sender as Panel).Focus();
        }

        public void CameraDisplacement_Handler(object sender, KeyEventArgs args) {
            //Up-left
            if (Keyboard.IsKeyDown(Key.Left) && Keyboard.IsKeyDown(Key.Up)) { GoLeft(); GoUp(); }
            //Up-right
            else if (Keyboard.IsKeyDown(Key.Up) && Keyboard.IsKeyDown(Key.Right)) { GoUp(); GoRight(); }
            //Down-right
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.Right)) { GoDown(); GoRight(); }
            //Down-left
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.Left)) { GoDown(); GoLeft(); }
            //Up
            else if (Keyboard.IsKeyDown(Key.Up)) GoUp();
            //Right
            else if (Keyboard.IsKeyDown(Key.Right)) GoRight();
            //Down
            else if (Keyboard.IsKeyDown(Key.Down)) GoDown();
            //Left
            else if (Keyboard.IsKeyDown(Key.Left)) GoLeft();
            InstantUpdate();
        }
    }
}
