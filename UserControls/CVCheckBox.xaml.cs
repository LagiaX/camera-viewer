using System.Windows.Controls;

namespace CV.UserControls {

    public class CVCheckBox : CheckBox {

        public CVCheckBox() { }

        public override string ToString() {
            return IsChecked.Value.ToString();
        }
    }
}
