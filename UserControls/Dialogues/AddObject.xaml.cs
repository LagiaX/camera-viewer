using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Reflection;

namespace CV.UserControls.Dialogues {
    
    public partial class AddObject : Window {

        public string figure;

        public AddObject() {
            InitializeComponent();
        }

        public AddObject(string figure) {
            InitializeComponent();

            Init(figure);
        }

        public void Init(string figure) {
            Object = new ModelVisual3D();
            this.figure = figure;
            switch (figure) {
                case "Cube":
                    material_Cube.ItemsSource = typeof(Colors).GetProperties();
                    material_Cube.SelectedIndex = 0;
                    backMaterial_Cube.ItemsSource = typeof(Colors).GetProperties();
                    backMaterial_Cube.SelectedIndex = 1;
                    ((Content as Grid).Children[0] as StackPanel).Visibility = Visibility.Visible;
                    break;
                case "Ellipsoid":
                    material_Ellipsoid.ItemsSource = typeof(Colors).GetProperties();
                    material_Ellipsoid.SelectedIndex = 0;
                    backMaterial_Ellipsoid.ItemsSource = typeof(Colors).GetProperties();
                    backMaterial_Ellipsoid.SelectedIndex = 1;
                    ((Content as Grid).Children[1] as StackPanel).Visibility = Visibility.Visible;
                    break;
            }
        }

        public new string Name { get; set; } = "MyFigure";

        public ModelVisual3D Object { get; set; }

        protected void CreateFigure() {
            try {
                Point3D center;
                Color color;
                Color backColor;
                switch (figure) {
                    case "Cube":
                        Object = new HelixToolkit.Wpf.CubeVisual3D();
                        center = center_Cube.Value;
                        double size = size_Cube.Value;
                        color = (Color)(material_Cube.SelectedItem as PropertyInfo).GetValue(null, null);
                        backColor = (Color)(backMaterial_Cube.SelectedItem as PropertyInfo).GetValue(null, null);
                        Object = new HelixToolkit.Wpf.CubeVisual3D();
                        HelixToolkit.Wpf.CubeVisual3D cube = Object as HelixToolkit.Wpf.CubeVisual3D;
                        cube.Center = center;
                        cube.SideLength = size;
                        cube.Material = new DiffuseMaterial(new SolidColorBrush(color));
                        cube.BackMaterial = new DiffuseMaterial(new SolidColorBrush(color));
                        Name = name_Cube.Text;
                        break;
                    case "Ellipsoid":
                        Object = new HelixToolkit.Wpf.EllipsoidVisual3D();
                        center = center_Ellipsoid.Value;
                        double xRad = xRadius_Ellipsoid.Value;
                        double yRad = yRadius_Ellipsoid.Value;
                        double zRad = zRadius_Ellipsoid.Value;
                        color = (Color)(material_Ellipsoid.SelectedItem as PropertyInfo).GetValue(null, null);
                        backColor = (Color)(backMaterial_Ellipsoid.SelectedItem as PropertyInfo).GetValue(null, null);
                        HelixToolkit.Wpf.EllipsoidVisual3D ellips = Object as HelixToolkit.Wpf.EllipsoidVisual3D;
                        ellips.Center = center;
                        ellips.RadiusX = xRad;
                        ellips.RadiusY = yRad;
                        ellips.Material = new DiffuseMaterial(new SolidColorBrush(color));
                        ellips.BackMaterial = new DiffuseMaterial(new SolidColorBrush(color));
                        Name = name_Ellipsoid.Text;
                        break;
                }
            } catch (Exception e) { Console.WriteLine(e.Message); Console.WriteLine(e.StackTrace); }
            
        }

        //HANDLERS
        public void ConfirmObject_Handler(object sender, RoutedEventArgs args) {
            CreateFigure();
            DialogResult = true;
        }
    }
}
