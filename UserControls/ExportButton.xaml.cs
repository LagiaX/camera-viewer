using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace CV.UserControls {

    public partial class ExportButton : UserControl {

        public ExportButton() {
            InitializeComponent();
        }

        public void ExportUnity(string path, Dialogues.ExportUnity.CameraModes_Items treeRoot) {
            int index = 0;
            string text = "";
            string[] lines;
            string uniScript = "";

            //CameraModes
            Uri uri = new Uri(@"/Resources/BasicCameraModes.txt", UriKind.RelativeOrAbsolute);
            var resourceStream = Application.GetResourceStream(uri);
            using (StreamReader sr = new StreamReader(resourceStream.Stream)) {
                text = sr.ReadToEnd();
                lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
            treeRoot.Mode = "";
            uniScript += ReadTree(lines, treeRoot);

            using (StreamWriter sw = File.CreateText(path + "\\CameraModes.cs")) {
                index = 0;
                string packages = "";
                while (!lines[index].Contains("Axis System Chosen")) index++;
                for (int start = 0; start <= index; start++) packages += lines[start] + "\n"; //packages from the top of the document
                sw.Write(packages + uniScript);
            }

            //CameraControlSystem
            uri = new Uri(@"/Resources/CameraControlSystem.txt", UriKind.RelativeOrAbsolute);
            resourceStream = Application.GetResourceStream(uri);
            using (StreamReader sr = new StreamReader(resourceStream.Stream)) {
                text = sr.ReadToEnd();
            }

            using (StreamWriter sw = File.CreateText(path + "\\CameraControlSystem.cs")) {
                sw.Write(text);
            }

            //iTightable
            uri = new Uri(@"/Resources/ITightable.txt", UriKind.RelativeOrAbsolute);
            resourceStream = Application.GetResourceStream(uri);
            using (StreamReader sr = new StreamReader(resourceStream.Stream)) {
                text = sr.ReadToEnd();
            }

            using (StreamWriter sw = File.CreateText(path + "\\ITightable.cs")) {
                sw.Write(text);
            }
        }

        public string ReadTree(string[] lines, Dialogues.ExportUnity.CameraModes_Items node) {
            int index = 0;
            int start = 0;
            int end = 0;
            string chunk = "";
            if (node.Included) {
                chunk = "\n";
                while (!lines[index].Contains("class " + node.Mode + "CameraMode")) index++;
                start = index;
                while (!lines[index].Contains("end class " + node.Mode + "CameraMode")) index++;
                end = index;
                //Extract every line into a string
                for (index = start; index <= end; index++) chunk += lines[index] + "\n";
            }
            foreach (Dialogues.ExportUnity.CameraModes_Items child in node.ChildCameras) {
                chunk = chunk + ReadTree(lines, child);
            }
            return chunk;
        }

        //HANDLERS
        public void HoverIn_Handler(object sender, RoutedEventArgs args) {
            //SolidColorBrush color = new SolidColorBrush(Colors.DarkTurquoise);
            //(sender as Button).Background = Brushes.Black;
        }

        public void HoverOut_Handler(object sender, RoutedEventArgs args) {
            //SolidColorBrush color = new SolidColorBrush(Colors.Blue);
            //(sender as Button).Background = Brushes.Red;
        }

        public void Hover_Handler(object sender, RoutedEventArgs args) { }

        public void ExportButton_Handler(object sender, RoutedEventArgs args) {
            try {
                Dialogues.ExportUnity inputDialog = new Dialogues.ExportUnity();
                if (inputDialog.ShowDialog() == true) {
                    ExportUnity(inputDialog.Path, inputDialog.CameraModes[0]);
                }
            } catch (Exception e) { Console.WriteLine(e.Message + "\n" + e.StackTrace); }
        }
    }
}
