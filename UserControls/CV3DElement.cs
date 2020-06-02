using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace CV.UserControls {

    public class CV3DElement : INotifyPropertyChanged {

        protected string name;
        protected ModelVisual3D _object;

        public CV3DElement(string name, ModelVisual3D obj) {
            Name = name;
            Object = obj;
        }

        public string Name {
            get { return name; }
            set {
                if (value != name) {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public ModelVisual3D Object {
            get { return _object; }
            set {
                if (value != _object) {
                    _object = value;
                    NotifyPropertyChanged("Object");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return Name;
        }
    }
}
