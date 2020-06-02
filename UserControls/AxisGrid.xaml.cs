using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace CV.UserControls {

    public partial class AxisGrid : UserControl, INotifyPropertyChanged {
        
        public event PropertyChangedEventHandler PropertyChanged;

        public AxisGrid() {
            InitializeComponent();
        }
        
        public Point3D Value {
            get { return new Point3D(X, Y, Z); }
            set {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public double X {
            get { return x_AxisGrid.Value; }
            set { x_AxisGrid.Value = value; }
        }

        public double Y {
            get { return y_AxisGrid.Value; }
            set { y_AxisGrid.Value = value; }
        }

        public double Z {
            get { return z_AxisGrid.Value; }
            set { z_AxisGrid.Value = value; }
        }

        public bool IsAngleRotation {
            get { return x_AxisGrid.IsAngleRotation && y_AxisGrid.IsAngleRotation && z_AxisGrid.IsAngleRotation; }
            set {
                x_AxisGrid.IsAngleRotation = y_AxisGrid.IsAngleRotation = z_AxisGrid.IsAngleRotation = value;
            }
        }

        private void NotifyPropertyChanged(string propertyName = "") {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return "(" + Value.ToString() + ")";
        }
    }
}
