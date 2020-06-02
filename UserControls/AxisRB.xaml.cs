using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace CV.UserControls {
    
    public partial class AxisRB : UserControl {

        public AxisRB() {
            InitializeComponent();
        }

        public Vector3D Value {
            get {
                if (X == true) return new Vector3D(1, 0, 0);
                else if (Y == true) return new Vector3D(0, 1, 0);
                else if (Z == true) return new Vector3D(0, 0, 1);
                else return new Vector3D(0, 0, 0);
            }
            set {
                if (value == new Vector3D(1, 0, 0)) X = true;
                else if (value == new Vector3D(0, 1, 0)) Y = true;
                else if (value == new Vector3D(0, 0, 1)) Z = true;
            }
        }

        public bool X {
            get { return x_AxisRB.IsChecked.Value; }
            set { x_AxisRB.IsChecked = value; }
        }

        public bool Y {
            get { return y_AxisRB.IsChecked.Value; }
            set { y_AxisRB.IsChecked = value; }
        }

        public bool Z {
            get { return z_AxisRB.IsChecked.Value; }
            set { z_AxisRB.IsChecked = value; }
        }

        public override string ToString() {
            return "(" + Value.ToString() + ")";
        }
    }
}
