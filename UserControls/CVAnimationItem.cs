using System.ComponentModel;
using System.Windows.Media.Animation;

namespace CV.UserControls {

    public class CVAnimationItem : INotifyPropertyChanged {
        protected string name;
        protected Storyboard animation;

        public CVAnimationItem(string name, Storyboard animation) {
            Name = name;
            Animation = animation;
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

        public Storyboard Animation {
            get { return animation; }
            set {
                if (value != animation) {
                    animation = value;
                    NotifyPropertyChanged("Animation");
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
