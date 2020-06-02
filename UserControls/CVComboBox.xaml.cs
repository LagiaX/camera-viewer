using System.Windows.Controls;

namespace CV.UserControls {

    public class CVComboBox : ComboBox {

        public CVComboBox() { }

        public override string ToString() {
            string str = "";
            foreach (object item in Items) {
                if (item.GetType() == typeof(System.Windows.Media.Media3D.Point3D) || item.GetType() == typeof(System.Windows.Media.Media3D.Vector3D))
                    str += "(" + item.ToString() + "),";
                else str += item.ToString() + ",";
            }
            str = str.Substring(0, str.Length-1); //Remove last comma
            return str;
        }
    }
}
