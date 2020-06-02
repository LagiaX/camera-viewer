using System.Numerics;
using System.ComponentModel;

namespace CV.UserControls {

    public class CVPlaneItem : INotifyPropertyChanged {

        protected string name;
        protected Plane plane;

        public CVPlaneItem(string name, Plane plane) {
            Name = name;
            Plane = plane;
        }

        public string Name {
            get { return name; }
            set {
                if (value != name) {
                    value = value.Trim();
                    value = value.Replace(" ","_");
                    name = value.Replace(",","_");
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public Plane Plane {
            get { return plane; }
            set {
                if (value != plane) {
                    plane = value;
                    NotifyPropertyChanged("Plane");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return Name + " | " + Plane.ToString();
        }
    }
}
