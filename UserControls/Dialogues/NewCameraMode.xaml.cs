using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CV.UserControls.Dialogues {

    public partial class NewCameraMode : Window {

        public NewCameraMode() {
            InitializeComponent();

            Init();
        }

        public void Init() {
            CameraFamilyList.Add(new CVCameraFamilyItem(new BitmapImage(new Uri(@"/Resources/Img/fixedCameraIcon.png", UriKind.RelativeOrAbsolute)), CV.Resources.CameraModeNames.CameraModes.Fixed));
            CameraFamilyList.Add(new CVCameraFamilyItem(new BitmapImage(new Uri(@"/Resources/Img/chaseCameraIconv2.png", UriKind.RelativeOrAbsolute)), CV.Resources.CameraModeNames.CameraModes.Chase));
            CameraFamilyList.Add(new CVCameraFamilyItem(new BitmapImage(new Uri(@"/Resources/Img/attachedCameraIcon.png", UriKind.RelativeOrAbsolute)), CV.Resources.CameraModeNames.CameraModes.Attached));
            CameraFamilyList.Add(new CVCameraFamilyItem(new BitmapImage(new Uri(@"/Resources/Img/planeBindedCameraIcon.png", UriKind.RelativeOrAbsolute)), CV.Resources.CameraModeNames.CameraModes.PlaneBinded));
            CameraFamilyList.Add(new CVCameraFamilyItem(new BitmapImage(new Uri(@"/Resources/Img/fixedDirectionCameraIcon.png", UriKind.RelativeOrAbsolute)), CV.Resources.CameraModeNames.CameraModes.FixedDirection));
            CameraFamilyList.Add(new CVCameraFamilyItem(new BitmapImage(new Uri(@"/Resources/Img/throughCameraIcon.png", UriKind.RelativeOrAbsolute)), CV.Resources.CameraModeNames.CameraModes.ThroughTarget));
            CameraFamilyList[0].Description = "In this mode the camera position and orientation never change.\nIt has a tracking functionality and an automatic selector for the closest point to the target.";
            CameraFamilyList[1].Description = "In this mode the camera follows the target. The orientation of the camera can be fixed by a yaw axis if desired.";
            CameraFamilyList[2].Description = "In this mode the camera is attached to the target as a child. It has also a switch to show or hide the scene as a first person camera.";
            CameraFamilyList[3].Description = "In this mode the camera is constrained to the limits of a plane. The camera always points to the target, mostly perpendicular to the plane. There's a RTS type game functionality.";
            CameraFamilyList[4].Description = "In this mode the target is always seen from the same direction. It is also posible to set the distance from which the camera will track the target.";
            CameraFamilyList[5].Description = "In this mode the camera points to a given position, the focus, through the target. The camera orientation is fixed by a yaw axis.";
            behaviors_NewCameraMode.ItemsSource = CameraFamilyList;
        }

        public List<CVCameraFamilyItem> CameraFamilyList { get; set; } = new List<CVCameraFamilyItem>();

        public Resources.CameraModeNames.CameraModes SelectedFamily { get; set; }

        //HANDLERS
        public void Confirm_Handler(object sender, RoutedEventArgs args) {
            if (!(name_NewCameraMode.Text == "") && !(behaviors_NewCameraMode.SelectedItem is null)) {
                Name = name_NewCameraMode.Text;
                SelectedFamily = (behaviors_NewCameraMode.SelectedItem as CVCameraFamilyItem).Family;
                DialogResult = true;
            }
        }

        public void Cancel_Handler(object sender, RoutedEventArgs args) {
            Close();
        }
    }
}
