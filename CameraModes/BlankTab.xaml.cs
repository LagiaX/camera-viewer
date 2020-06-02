using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CV.UserControls;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

/*
* double rotationX = Vector3D.AngleBetween(new Vector3D(1, 0, 0), yourMatrix3D.Transform(new Vector3D(1, 0, 0)));
* double rotationY = Vector3D.AngleBetween(new Vector3D(0, 1, 0), yourMatrix3D.Transform(new Vector3D(0, 1, 0)));
* double rotationZ = Vector3D.AngleBetween(new Vector3D(0, 0, 1), yourMatrix3D.Transform(new Vector3D(0, 0, 1)));
*/

namespace CV.CameraModes {

    public partial class BlankTab : Page {

        protected List<string> cameraModeList;
        protected PerspectiveCamera camera;
        protected ObservableCollection<CV3DElement> scene;

        public BlankTab(Frame frame, PerspectiveCamera cam, ObservableCollection<CV3DElement> scene) {
            InitializeComponent();
            cameraModeList = new List<string>() { "Fixed", "Chase", "Attached", "Plane Binded", "Through Target", "Fixed Direction" };
            camList_BlankTab.ItemsSource = cameraModeList;

            frame_BlankTab = frame;
            camera = cam;
            this.scene = scene;

            frame_BlankTab.ContentRendered += DeleteHistory_Handler;
        }

        public CameraMode CurrentCamMode { get; set; }

        public void TabChange() {
            if (CurrentCamMode != null) CurrentCamMode.InstantUpdate();
        }

        public void SelectCamMode(string cameraMode) {
            cameraMode = cameraMode.Replace(" ","");
            Console.WriteLine(cameraMode);
            try {
                switch (cameraMode) {
                    case "Fixed":
                        CurrentCamMode = new FixedCM(camera, scene);
                        break;
                    case "Chase":
                        CurrentCamMode = new ChaseCM(camera, scene);
                        break;
                    case "Attached":
                        CurrentCamMode = new AttachedCM(camera, scene);
                        break;
                    case "PlaneBinded":
                        CurrentCamMode = new PlaneBindedCM(camera, scene);
                        break;
                    case "ThroughTarget":
                        CurrentCamMode = new ThroughTargetCM(camera, scene);
                        break;
                    case "FixedDirection":
                        CurrentCamMode = new FixedDirectionCM(camera, scene);
                        break;
                }
            } catch(Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void Navigate() {
            frame_BlankTab.Navigate(CurrentCamMode);
        }

        //HANDLERS
        public void ConfirmButton_Handler(object sender, RoutedEventArgs e) {
            if (!(camList_BlankTab.SelectedItem is null)) {
                SelectCamMode((string)camList_BlankTab.SelectedItem);
                Navigate();
            }
        }

        private void DeleteHistory_Handler(object sender, EventArgs args) {
            frame_BlankTab.RemoveBackEntry();
        }
    }
}
