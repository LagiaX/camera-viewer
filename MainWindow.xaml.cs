using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using CV.UserControls;
using HelixToolkit.Wpf;
using Microsoft.Win32;

namespace CV {

    public partial class MainWindow : Window {

        public DynamicTabs dTabs;
        public Storyboard storyboard;
        public ObservableCollection<CV3DElement> modelList; //Name property is 'model<n>'
        public ObservableCollection<CVAnimationItem> animationList;
        public static TimeSpan lastRendTime;

        public MainWindow() {
            InitializeComponent();
            //Initialize object list
            modelList = new ObservableCollection<CV3DElement>();
            modelList.Add(new CV3DElement("MainModel", mainTarget_MainWindow));
            objects_MainWindow.ItemsSource = modelList;
            //Insert DynamicTabs
            dTabs = new DynamicTabs(mainCamera_MainWindow, modelList);
            dTabs.Name = "dTabs_MainWindow";
            controlDock_MainWindow.Children.Add(dTabs);
            //Initialize animation list
            animationList = new ObservableCollection<CVAnimationItem>();
            string[] keys = new string[Resources.Keys.Count];
            Resources.Keys.CopyTo(keys, 0);
            for (int index = 0; index < Resources.Keys.Count; index++) {
                animationList.Add(new CVAnimationItem(keys[index], Resources[keys[index]] as Storyboard));
            }
            animList_MainWindow.ItemsSource = animationList;
            animList_MainWindow.SelectedIndex = 0;
            //Select targets for animations
            Storyboard.SetTarget(animationList[0].Animation.Children[0], modelList[0].Object);
            //Storyboard.SetTarget(animationList[0].Animation.Children[1], modelList[0].Object.Transform);
            storyboard = (animList_MainWindow.SelectedItem as CVAnimationItem).Animation;
            storyboard.Begin(this, true);
            storyboard.Stop(this);
            //Load skybox
            try { skybox_MainWindow.Source = "Resources\\Skybox\\Desert\\"; skybox_MainWindow.Size = 500; }
            catch (Exception e) { Console.WriteLine(e.Message); Console.WriteLine(e.StackTrace); }
            //Activate tooltips
            menuTooltips_MainWindow.IsChecked = true;
            //Rendering
            lastRendTime = TimeSpan.Zero;
            CompositionTarget.Rendering += RenderUpdate_Handler;
        }

        public void LoadModel(string path) {
            ModelImporter importer = new ModelImporter();
            Model3DGroup group = importer.Load(path);
            group.Transform = new ScaleTransform3D(0.1, 0.1, 0.1);
            ModelVisual3D temp = new ModelVisual3D();
            temp.Content = group;
            
            models_MainWindow.Children.Add(temp);
            modelList.Add(new CV3DElement("model" + modelList.Count, temp));
        }

        public void RemoveModel(string name) {
            int index = 0;
            while (name != modelList[index].Name) index++;
            //if (index == 0) storyboard.Stop(this); //If removing first model, stop the animation
            models_MainWindow.Children.Remove(modelList[index].Object);
            modelList.RemoveAt(index);
            for (int index2 = 0; index2 < animationList.Count; index2++) //Loop to go through animations
                Storyboard.SetTarget(animationList[index2].Animation.Children[0], modelList[0].Object);
            storyboard.Begin(this, true);
            storyboard.Stop(this);
            Page tab;
            for (int index3 = 0; index3 < dTabs.TabControl.Items.Count - 1; index3++) {
                tab = (((dTabs.TabControl.SelectedItem as TabItem).Content as Frame).Content as Page);
                if (!(tab is CameraModes.BlankTab)) { //tab is not BlankTab
                    CameraModes.CameraMode.Target = (modelList[0].Object as ModelVisual3D).Content as GeometryModel3D;
                    (tab as CameraModes.CameraMode).AssignTargetHandler();
                }
            }
        }

        public string SaveSettings(FrameworkElement controls, string settings = "") {
            if (controls is Panel) {
                for (int i = 0; i < ((Panel)controls).Children.Count; i++) {
                    if (((Panel)controls).Children[i] is Panel || ((Panel)controls).Children[i] is ScrollViewer) { //If it's a panel, call(n+1)
                        settings += SaveSettings(((Panel)controls).Children[i] as Panel);
                    }
                    else if (((Panel)controls).Children[i] is Decorator) {
                        settings += SaveSettings(((Panel)controls).Children[i] as Decorator);
                    }
                    else if (((Panel)controls).Children[i].GetType() == typeof(Label)) { //If it's a label, add it as string followed by ';'
                        if ((((Panel)controls).Children[i] as Label).Content is TextBlock) {
                            settings += ((((Panel)controls).Children[i] as Label).Content as TextBlock).Text + ";";
                        }
                        else settings += (((Panel)controls).Children[i] as Label).Content.ToString() + ";";
                    }
                    else if (((Panel)controls).Children[i].GetType() != typeof(Button)) { //If it's NOT a buttton (nor panel, nor label), add it as string and line jump
                        settings += ((Panel)controls).Children[i].ToString() + "\n";
                    }
                }
            }
            else if (controls is Decorator) {
                settings += SaveSettings(((Decorator)controls).Child as FrameworkElement);
            }
            else if (controls.GetType() == typeof(Label)) { //If it's a label, add it as string followed by ';'
                if ((controls as Label).Content is TextBlock) {
                    settings += ((controls as Label).Content as TextBlock).Text + ";";
                }
                else settings += (controls as Label).Content.ToString() + ";";    
            }
            else if (controls.GetType() != typeof(Button)) { //If it's NOT a buttton (nor panel, nor label), add it as string and line jump
                    settings += controls.ToString() + "\n";
            }
            return settings;
        }

        public bool LoadSettings(string fileName, string rawSettings) {
            StringReader sr = new StringReader(rawSettings);
            fileName = fileName.Remove(fileName.Length-4);
            string camMode = sr.ReadLine();
            string line;
            List<string[]> mappedSettings = new List<string[]>();
            while ((line = sr.ReadLine()) != null) {
                mappedSettings.Add(line.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            }
            dTabs.TabLoader(fileName, camMode, mappedSettings);
            return true;
        }

        /*public Storyboard RotateModel(Point3D a, Point3D b) {
            Vector3D direction = b - a;
            modelList[0].Object.Transform.Value.Transform(direction);
            //double Vector3D.AngleBetween(direction, modelList[0].Object.Transform.Value.Transform(new Vector3D(0, 0, -1)));
            return dafq;
        }*/

        public static T Cast<T>(object o) {
            return (T)o;
        }

        //HANDLERS
        public void WindowLoaded_Handler(object sender, RoutedEventArgs e) {
            this.Left = SystemParameters.WorkArea.Left;
            this.Top = SystemParameters.WorkArea.Top;
            this.Height = SystemParameters.WorkArea.Height;
            this.Width = SystemParameters.WorkArea.Width;
        }

        public void NewCamera_Handler(object sender, RoutedEventArgs args) {
            string camMode = (string)(sender as MenuItem).Header;
            camMode = camMode.Substring(1).Remove(camMode.Length-8); //This trims out '_' and ' Camera'
            camMode = camMode.Replace(" ",""); //This removes empty spaces
            TabItem temp = dTabs.AddTabItem(camMode);
            CameraModes.BlankTab blank = new CameraModes.BlankTab((Frame)temp.Content, mainCamera_MainWindow, modelList);
            dTabs.InsertTab(temp);
            blank.SelectCamMode(camMode);
            dTabs.Navigate(temp, blank);
            blank.Navigate();
        }

        public void RenderUpdate_Handler(object sender, EventArgs args) {
            if ((args as RenderingEventArgs).RenderingTime != lastRendTime) { //Render once per frame
                if (!((dTabs.TabControl.SelectedContent as Frame).Content is CameraModes.BlankTab)) {
                    if(storyboard.GetCurrentState(this) != ClockState.Stopped && storyboard.GetIsPaused(this) == false) {
                        ((dTabs.TabControl.SelectedContent as Frame).Content as CameraModes.CameraMode).UpdateScene();
                    }
                }
            }
            lastRendTime = (args as RenderingEventArgs).RenderingTime;
        }

        public void StartStoryboard_Handler(object sender, RoutedEventArgs args) {
            if (!(animList_MainWindow.SelectedItem is null) && modelList.Count > 0) {
                storyboard = (animList_MainWindow.SelectedItem as CVAnimationItem).Animation;
                storyboard.Begin(this, true);
            }
        }

        public void ResumeStoryboard_Handler(object sender, RoutedEventArgs args) {
            if (!(animList_MainWindow.SelectedItem is null) && modelList.Count > 0) {
                storyboard.Resume(this);
            }
        }

        public void PauseStoryboard_Handler(object sender, RoutedEventArgs args) {
            if (!(animList_MainWindow.SelectedItem is null) && modelList.Count > 0) {
                storyboard.Pause(this);
            }
        }

        public void StopStoryboard_Handler(object sender, RoutedEventArgs args) {
            if (!(animList_MainWindow.SelectedItem is null) && modelList.Count > 0) {
                storyboard.Stop(this);
            }
        }

        public void SpeedupStoryboard_Handler(object sender, RoutedEventArgs args) {
            if (!(animList_MainWindow.SelectedItem is null) && modelList.Count > 0) {
                storyboard.SetSpeedRatio(this, storyboard.SpeedRatio * 2.0);
            }
        }

        public void SpeeddownStoryboard_Handler(object sender, RoutedEventArgs args) {
            if (!(animList_MainWindow.SelectedItem is null) && modelList.Count > 0) {
                storyboard.SetSpeedRatio(this, storyboard.SpeedRatio * 0.5);
            }
        }

        public void UpdateView_Handler(object sender, RoutedEventArgs args) {
            try {
                if (dTabs.TabControl.HasItems && !((dTabs.TabControl.SelectedContent as Frame).Content is CameraModes.BlankTab)) {
                    ((dTabs.TabControl.SelectedContent as Frame).Content as CameraModes.CameraMode).InstantUpdate();
                }
            } catch(Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace);}
        }

        public void ToolTips_Handler(object sender, RoutedEventArgs args) {
            try {
                bool enabled = (sender as MenuItem).IsChecked;
                Style style;
                this.Resources.Remove(typeof(ToolTip));
                if (enabled) style = Application.Current.FindResource("ToolTipsOn") as Style;
                else style = Application.Current.FindResource("ToolTipsOff") as Style;
                this.Resources.Add(typeof(ToolTip), style);
                //Console.WriteLine("Style changed for main window");
                Page tab;
                for (int index = 0; index < dTabs.TabControl.Items.Count-1; index++) {
                    tab = (((dTabs.TabControl.Items[index] as TabItem).Content as Frame).Content as Page);
                    if (!(tab is CameraModes.BlankTab)) { //tab is not BlankTab
                        tab.Resources.Remove(typeof(ToolTip));
                        tab.Resources.Add(typeof(ToolTip), style);
                        
                    }
                }
                //Console.WriteLine("Style changed also for all tabs");
            } catch(Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void LoadModel_Handler(object sender, RoutedEventArgs args) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "3D models (*.3ds, *.obj, etc)|*.3ds;*.obj;*.stl";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == true) {
                LoadModel(dialog.FileName);
            }
        }

        public void ShowGrid_Handler(object sender, RoutedEventArgs args) {
            if (gridLines_MainWindow.Visible) gridLines_MainWindow.Visible = false;
            else gridLines_MainWindow.Visible = true; 
        }

        public void NewCameraMode_Handler(object sender, RoutedEventArgs args) {
            try {
                UserControls.Dialogues.NewCameraMode dialog = new UserControls.Dialogues.NewCameraMode();
                if (dialog.ShowDialog() == true) {
                    TabItem temp = dTabs.AddTabItem(dialog.Name);
                    CameraModes.BlankTab blank = new CameraModes.BlankTab((Frame)temp.Content, mainCamera_MainWindow, modelList);
                    blank.SelectCamMode(dialog.SelectedFamily.ToString());
                    dTabs.InsertTab(temp);
                    dTabs.Navigate(temp, blank);
                    blank.Navigate();
                }
            } catch (Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void SaveWork_Handler(object sender, RoutedEventArgs args) {
            try {
                Page currentCamMode = (Page)(dTabs.TabControl.SelectedContent as Frame).Content;
                string camMode = currentCamMode.GetType().Name;
                if (camMode != "BlankTab") {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Filter = "Camera Viewer Work files (*.cmw) | *.cmw";
                    if (dialog.ShowDialog() == true) {         
                        string settings = camMode + "\n" + SaveSettings((Grid)((currentCamMode.Content as ScrollViewer).Content as Border).Child);
                        using (StreamWriter sw = File.CreateText(dialog.FileName)) {
                            sw.Write(settings);
                        }
                        args.Handled = true;
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public void LoadWork_Handler(object sender, RoutedEventArgs args) {
            try {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Camera Viewer Work files (*.cmw) | *.cmw";
                if (dialog.ShowDialog() == true) {
                    using(StreamReader sr = File.OpenText(dialog.FileName)) {
                        string buffer = sr.ReadToEnd();
                        LoadSettings(dialog.SafeFileName, buffer);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void About_Handler(object sender, RoutedEventArgs args) {
            UserControls.Dialogues.About dialogue = new UserControls.Dialogues.About();
            if (dialogue.ShowDialog() == true) {
                //Dialogue closed
            }
        }

        public void ObjectSelection_Handler(object sender, RoutedEventArgs args) {
            if (!(objects_MainWindow.SelectedItem is null)) {
                if ((objects_MainWindow.SelectedItem as CV3DElement).Object is CubeVisual3D) {
                    objectPosition_MainWindow.Value = ((objects_MainWindow.SelectedItem as CV3DElement).Object as CubeVisual3D).Center;
                }
                else if ((objects_MainWindow.SelectedItem as CV3DElement).Object is EllipsoidVisual3D) {
                    objectPosition_MainWindow.Value = ((objects_MainWindow.SelectedItem as CV3DElement).Object as EllipsoidVisual3D).Center;
                }
                //Utilities.FindCenter((objects_MainWindow.SelectedItem as CV3DElement).Object.Content.Bounds);
            }
        }

        public void ObjectCurrentPosition_Handler(object sender, RoutedEventArgs args) {
            try {
                if (!(objects_MainWindow.SelectedItem is null)) {
                    CV3DElement element = objects_MainWindow.SelectedItem as CV3DElement;
                    /*ModelVisual3D model = element.Object;
                    Matrix3D temp = (model.Content as Model3D).Transform.Value;
                    temp.OffsetX = objectPosition_MainWindow.Value.X;
                    temp.OffsetY = objectPosition_MainWindow.Value.Y;
                    temp.OffsetZ = objectPosition_MainWindow.Value.Z;*/
                    if (element.Object is CubeVisual3D) {
                       (element.Object as CubeVisual3D).Center = objectPosition_MainWindow.Value;
                    }
                    else if (element.Object is EllipsoidVisual3D) {
                        (element.Object as EllipsoidVisual3D).Center = objectPosition_MainWindow.Value;
                    }
                    //(model.Content as Model3D).Transform = new MatrixTransform3D(temp);
                }
            } catch (Exception e) { Console.WriteLine(e.Message); Console.WriteLine(e.Source); }
        }

        public void AddObject_Handler(object sender, RoutedEventArgs args) {
            Button button = sender as Button;
            Popup menu = button.Resources["options"] as Popup;
            menu.Width = button.ActualWidth;
            menu.PlacementTarget = button;
            menu.IsOpen = true;
        }

        public void AddFigure_Handler(object sender, RoutedEventArgs args) {
            try {
                Button button = sender as Button;
                UserControls.Dialogues.AddObject dialog = new UserControls.Dialogues.AddObject(button.Content as string);
                if (dialog.ShowDialog() == true) {
                    models_MainWindow.Children.Add(dialog.Object);
                    //Find if modelList contains element with the same name
                    int index = 0;
                    bool exists = false;
                    while (!exists && index < modelList.Count) {
                        if (modelList[index].Name != dialog.Name) index++;
                        else exists = true;
                    }
                    //If there's an object of the same name, add a number to the name
                    if (exists) modelList.Add(new CV3DElement(dialog.Name + (modelList.Count + 1), dialog.Object));
                    else modelList.Add(new CV3DElement(dialog.Name, dialog.Object));
                    for (int index2 = 0; index2 < animationList.Count; index2++) {
                        Storyboard.SetTarget(animationList[index2].Animation.Children[0], modelList[0].Object);
                        //Storyboard.SetTarget(animationList[index2].Animation.Children[1], modelList[0].Object.Transform);
                    }
                }
            } catch (Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void RemoveObject_Handler(object sender, RoutedEventArgs args) {
            try {
                if (!(objects_MainWindow.SelectedItem is null) && objects_MainWindow.Items.Count > 1) RemoveModel((objects_MainWindow.SelectedItem as CV3DElement).Name);
            } catch (Exception e) { Console.WriteLine("Cause: " + e.Message + "\nMethod: " + e.TargetSite + "\nStack: " + e.StackTrace); }
        }

        public void ListItemDrag_Handler(object sender, MouseEventArgs args) {
            if (sender is ListBoxItem && args.LeftButton == MouseButtonState.Pressed) {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        public void ListItemDrop_Handler(object sender, DragEventArgs args) {
            CV3DElement droppedData = args.Data.GetData(typeof(CV3DElement)) as CV3DElement;
            CV3DElement target = ((ListBoxItem)(sender)).DataContext as CV3DElement;

            int removedIndex = objects_MainWindow.Items.IndexOf(droppedData);
            int targetIndex = objects_MainWindow.Items.IndexOf(target);

            if (removedIndex < targetIndex) {
                modelList.Insert(targetIndex + 1, droppedData);
                modelList.RemoveAt(removedIndex);
            }
            else {
                int remIndex = removedIndex + 1;
                if (modelList.Count + 1 > remIndex)
                {
                    modelList.Insert(targetIndex, droppedData);
                    modelList.RemoveAt(remIndex);
                }
            }
            objects_MainWindow.Items.Refresh();
        }

        protected void ExitCommand_CanExe(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = true;
        }

        protected void ExitCommand_Exe(object sender, ExecutedRoutedEventArgs args) {
            Application.Current.Shutdown();
        }
    }

    //COMMANDS
    public static class Commands {

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(Commands),
            new InputGestureCollection() {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );
    }
}
