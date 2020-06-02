using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using CV.Resources;

namespace CV.UserControls.Dialogues {
    
    public partial class ExportUnity : Window {

        public ExportUnity() {
            InitializeComponent();
            //this.DataContext = this;
            cameraModes_ExportUnity.ItemsSource = CameraModes;
            Init();
        }

        public void Init() {
            CameraModes_Items root = new CameraModes_Items(CameraModeNames.CameraModes.CameraMode.ToString());
            CameraModes.Add(root);
            /////////////////////////////////////////////////////
            CameraModes_Items fixedCamera = new CameraModes_Items(CameraModeNames.CameraModes.Fixed.ToString());
            root.ChildCameras.Add(fixedCamera);
            CameraModes_Items fixedTrackingCamera = new CameraModes_Items(CameraModeNames.CameraModes.FixedTracking.ToString());
            fixedCamera.ChildCameras.Add(fixedTrackingCamera);
            CameraModes_Items closest2TargetCamera = new CameraModes_Items(CameraModeNames.CameraModes.ClosestToTarget.ToString());
            fixedTrackingCamera.ChildCameras.Add(closest2TargetCamera);
            /////////////////////////////////////////////////////
            CameraModes_Items chaseCamera = new CameraModes_Items(CameraModeNames.CameraModes.Chase.ToString());
            root.ChildCameras.Add(chaseCamera);
            CameraModes_Items chaseFYACamera = new CameraModes_Items(CameraModeNames.CameraModes.ChaseFreeYawAxis.ToString());
            chaseCamera.ChildCameras.Add(chaseFYACamera);
            /////////////////////////////////////////////////////
            CameraModes_Items attachedCamera = new CameraModes_Items(CameraModeNames.CameraModes.Attached.ToString());
            root.ChildCameras.Add(attachedCamera);
            CameraModes_Items firstPersonCamera = new CameraModes_Items(CameraModeNames.CameraModes.FirstPerson.ToString());
            attachedCamera.ChildCameras.Add(firstPersonCamera);
            /////////////////////////////////////////////////////
            CameraModes_Items planeBindedCamera = new CameraModes_Items(CameraModeNames.CameraModes.PlaneBinded.ToString());
            root.ChildCameras.Add(planeBindedCamera);
            CameraModes_Items planViewCamera = new CameraModes_Items(CameraModeNames.CameraModes.PlanView.ToString());
            planeBindedCamera.ChildCameras.Add(planViewCamera);
            CameraModes_Items rtsCamera = new CameraModes_Items(CameraModeNames.CameraModes.RTS.ToString());
            planViewCamera.ChildCameras.Add(rtsCamera);
            /////////////////////////////////////////////////////
            CameraModes_Items throughTargetCamera = new CameraModes_Items(CameraModeNames.CameraModes.ThroughTarget.ToString());
            root.ChildCameras.Add(throughTargetCamera);
            /////////////////////////////////////////////////////
            CameraModes_Items fixedDirectionCamera = new CameraModes_Items(CameraModeNames.CameraModes.FixedDirection.ToString());
            root.ChildCameras.Add(fixedDirectionCamera);
            path_ExportUnity.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public class CameraModes_Items : INotifyPropertyChanged {

            public event PropertyChangedEventHandler PropertyChanged;
            protected ObservableCollection<CameraModes_Items> childCameras;
            protected bool included;

            public CameraModes_Items(string modeName = "") {
                Mode = modeName;
                childCameras = new ObservableCollection<CameraModes_Items>();
                included = false;
            }

            public string Mode { get; set; }

            public bool Included {
                get { return included; }
                set {
                    included = value;
                    this.NotifyPropertyChanged("boolChanged");
                }
            }

            public ObservableCollection<CameraModes_Items> ChildCameras {
                get { return childCameras; }
                set { childCameras = value; }
            }

            private void NotifyPropertyChanged(string propertyName = "") {  
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<CameraModes_Items> CameraModes { get; set; } = new ObservableCollection<CameraModes_Items>();

        public string Path { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //HANDLERS
        public void Browser_Handler(object sender, RoutedEventArgs args) {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if (dialog.ShowDialog() == true) {
                path_ExportUnity.Text = Path = dialog.SelectedPath;
            }
        }
        //???? This won't work, not like is relevant...
        public void SelectedItem_Handler(object sender, RoutedEventArgs args) {
            /*CameraModes_Items item = cameraModes_ExportUnity.SelectedItem as CameraModes_Items;
            if (item.Included) { item.Included = false; Console.WriteLine("INCLUDED TO FALSE"); }
            else item.Included = true;*/
        }

        public void ConfirmExport_Handler(object sender, RoutedEventArgs args) {
            if (path_ExportUnity.Text != "" && !(path_ExportUnity is null)) DialogResult = true;
        }
    }
}
