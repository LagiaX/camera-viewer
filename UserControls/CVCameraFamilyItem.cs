using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CV.UserControls {

    public class CVCameraFamilyItem {

        public CVCameraFamilyItem(ImageSource img, Resources.CameraModeNames.CameraModes mode) {
            Image = img;
            Family = mode;
        }

        public ImageSource Image { get; set; }

        public Resources.CameraModeNames.CameraModes Family { get; set; }

        public string Description { get; set; }

        public override string ToString() {
            return Family.ToString();
        }
    }
}
