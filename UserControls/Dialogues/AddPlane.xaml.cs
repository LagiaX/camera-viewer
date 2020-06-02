using System.Windows;
using System.Numerics;

namespace CV.UserControls.Dialogues {
    
    public partial class AddPlane : Window {

        protected Plane plane;

        public AddPlane() {
            InitializeComponent();
        }

        public string PlaneName {
            get { return name_AddPlane.Text; }
        }

        public Plane Plane {
            get { return plane; }
        } 

        //HANDLERS
        public void AddPlane_Handler(object sender, RoutedEventArgs args) {
            if (creation_AddPlane.SelectedIndex == 0) { //Creation by triangle vertices
                plane = Plane.CreateFromVertices(Utilities.MediaVec2NumVec(p1_AddPlane.Value), Utilities.MediaVec2NumVec(p2_AddPlane.Value), Utilities.MediaVec2NumVec(p3_AddPlane.Value));
                DialogResult = true;
            }
            else if (creation_AddPlane.SelectedIndex == 1) { //Creation by normal and distance
                plane = new Plane(Utilities.MediaVec2NumVec(norm_AddPlane.Value), (float)dist_AddPlane.Value);
                DialogResult = true;
            }
        }
    }
}
