using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media.Media3D;
using CV.CameraModes;

namespace CV.UserControls {

    public partial class DynamicTabs : UserControl {

        protected PerspectiveCamera camera;
        protected ObservableCollection<CV3DElement> modelList;
        protected List<TabItem> tabItems;
        protected TabItem tabAdd;

        public DynamicTabs(PerspectiveCamera cam, ObservableCollection<CV3DElement> modelList) {
            InitializeComponent();
            camera = cam;
            this.modelList = modelList;
            tabItems = new List<TabItem>();
            tabAdd = new TabItem();
            Init();
        }

        public void Init() {
            tabAdd.Header = "+";
            tabItems.Add(tabAdd);
            tabControl_DynamicTabs.DataContext = tabItems;
            tabControl_DynamicTabs.SelectedIndex = 0; //This is what triggers SelectionChanged_Handler and creates the first tab
        }

        public TabControl TabControl {
            get { return tabControl_DynamicTabs; }
        }

        public void InsertTab(TabItem tab) {
            TabControl.DataContext = null;
            tabItems.Insert(tabItems.Count-1, tab);
            TabControl.DataContext = tabItems;
            TabControl.SelectedItem = tab;
        }

        public string NameTab(int count, string tabName = null) {
            if (tabName == null) {
                do {
                    tabName = string.Format("NewTab{0}", count);
                    count++;
                } while (tabItems.Contains(tabItems.Find(i => i.Header.ToString() == tabName))); //Contains an element with the same name
            }
            else {
                string baseName = tabName;
                while (tabItems.Contains(tabItems.Find(i => i.Header.ToString() == tabName))) { //Contains an element with the same name
                    tabName = string.Format(baseName + "{0}", count);
                    count++;
                }
            }
            return tabName;
        }

        public TabItem AddTabItem(string tabName = null) {
            TabItem tab = new TabItem();
            //Decide the tab name
            if (tabName is null) tabName = NameTab(tabItems.Count);
            tab.Header = tabName;
            tab.Name = tabName;
            tab.HeaderTemplate = tabControl_DynamicTabs.FindResource("TabHeader") as DataTemplate;
            Frame frame = new Frame();
            tab.Content = frame;
            return tab;
        }

        public void Navigate(TabItem tab, BlankTab blank) {
            Frame frame = tab.Content as Frame;
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            frame.JournalOwnership = JournalOwnership.OwnsJournal;
            frame.Navigate(blank);
        }

        public void TabLoader(string tabName, string camMode, List<string[]> map) {
            TabItem tab = AddTabItem(tabName);
            BlankTab blank = new BlankTab((Frame)tab.Content, camera, modelList);
            //System.Console.WriteLine(camMode);
            try {
                
                switch (camMode) {
                    case "FixedCM":
                        blank.SelectCamMode("Fixed");
                        FixedCM fixCM = (FixedCM)blank.CurrentCamMode;
                        fixCM.fixedPos_FixedCM.Value = Utilities.Str2Point3D(map[0][1]);
                        fixCM.fixedRot_FixedCM.Value = Utilities.Str2Point3D(map[1][1]);
                        fixCM.autoTracking_FixedCM.IsChecked = bool.Parse(map[2][1]);
                        fixCM.closestCamera_FixedCM.IsChecked = bool.Parse(map[3][1]);
                        fixCM.AssignList(Utilities.Str2PointList(map[4][1]));
                        break;
                    case "ChaseCM":
                        blank.SelectCamMode("Chase");
                        ChaseCM chaCM = blank.CurrentCamMode as ChaseCM;
                        chaCM.rPos_ChaseCM.Value = Utilities.Str2Point3D(map[0][1]);
                        chaCM.fixedAxis_ChaseCM.Value = Utilities.Str2Point3D(map[1][1]);
                        chaCM.tightness_ChaseCM.Value = double.Parse(map[2][1]);
                        chaCM.freeAxis_ChaseCM.IsChecked = bool.Parse(map[3][1]);
                        chaCM.rOffset_ChaseCM.Value = Utilities.Str2Point3D(map[4][1]);
                        break;
                    case "AttachedCM":
                        blank.SelectCamMode("Attached");
                        AttachedCM attCM = blank.CurrentCamMode as AttachedCM;
                        attCM.rPos_AttachedCM.Value = Utilities.Str2Point3D(map[0][1]);
                        attCM.rRot_AttachedCM.Value = Utilities.Str2Point3D(map[1][1]);
                        attCM.charVision_AttachedCM.IsChecked = bool.Parse(map[3][1]);
                        break;
                    case "PlaneBindedCM":
                        blank.SelectCamMode("Plane Binded");
                        PlaneBindedCM plbCM = blank.CurrentCamMode as PlaneBindedCM;
                        List<CVPlaneItem> planes = Utilities.Str2PlaneList(map[0][1]);
                        plbCM.AssignList(planes);
                        plbCM.tightness_PlaneBindedCM.Value = double.Parse(map[1][1]);
                        plbCM.pView_PlaneBindedCM.IsChecked = bool.Parse(map[2][1]);
                        plbCM.zoom_PlaneBindedCM.Value = double.Parse(map[3][1]);
                        plbCM.rtsMode_PlaneBindedCM.IsChecked = bool.Parse(map[4][1]);
                        if (plbCM.rtsMode_PlaneBindedCM.IsChecked == true) {
                            plbCM.leftAxis_PlaneBindedCM.Value = (Vector3D)Utilities.Str2Point3D(map[5][1]);
                            plbCM.upAxis_PlaneBindedCM.Value = (Vector3D)Utilities.Str2Point3D(map[6][1]);
                            plbCM.confirmSys_PlaneBindedCM.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                        plbCM.pitch_PlaneBindedCM.Value = double.Parse(map[7][1]);
                        plbCM.disp_PlaneBindedCM.Value = Utilities.Str2Point3D(map[8][1]);
                        plbCM.moveF_PlaneBindedCM.Value = double.Parse(map[9][1]);
                        break;
                    case "ThroughTargetCM":
                        blank.SelectCamMode("Through Target");
                        ThroughTargetCM thrCM = blank.CurrentCamMode as ThroughTargetCM;
                        //thrCM.fAxis_ThroughTargetCM.Value = Utilities.Str2Point3D(map[0][1]);
                        thrCM.fPos_ThroughTargetCM.Value = Utilities.Str2Point3D(map[0][1]);
                        thrCM.margin_ThroughTargetCM.Value = double.Parse(map[1][1]);
                        thrCM.iControl_ThroughTargetCM.IsChecked = bool.Parse(map[2][1]);
                        break;
                    case "FixedDirectionCM":
                        blank.SelectCamMode("Fixed Direction");
                        FixedDirectionCM fxdCM = blank.CurrentCamMode as FixedDirectionCM;
                        fxdCM.fDir_FixedDirectionCM.Value = Utilities.Str2Point3D(map[0][1]);
                        //fxdCM.fAxis_FixedDirectionCM.Value = Utilities.Str2Point3D(map[1][1]);
                        fxdCM.dist_FixedDirectionCM.Value = double.Parse(map[1][1]);
                        fxdCM.tightness_FixedDirectionCM.Value = double.Parse(map[2][1]);
                        break;
                }
                
            } catch(System.Exception e) {
                System.Console.WriteLine(e.Message +"\n"+e.Source+"\n"+e.StackTrace);
                MessageBox.Show("Error at reading the file.\nCause: " + e.Message);
                return;
            }
            InsertTab(tab);
            Navigate(tab, blank);
            blank.Navigate();
            
        }
        
        //HANDLERS
        public void SelectionChanged_Handler(object sender, SelectionChangedEventArgs args) {
            TabItem tab = tabControl_DynamicTabs.SelectedItem as TabItem;

            if (tab != null && tab.Header != null) {
                if (tab.Header.Equals(tabAdd.Header)) {
                    //Unhook, add tab to list and hook back, then assign the new tab as the current tab
                    tabControl_DynamicTabs.DataContext = null;
                    TabItem newTab = AddTabItem();
                    InsertTab(newTab);
                    BlankTab blank = new BlankTab((Frame)newTab.Content, camera, modelList);
                    Navigate(newTab, blank);
                }
                else if (!((tab.Content as Frame).Content is BlankTab) && !((tab.Content as Frame).Content is null)) {
                   ((tab.Content as Frame).Content as CameraMode).InstantUpdate();
                }
            }
        }

        public void CloseButton_Handler(object sender, RoutedEventArgs args) {
            string tabName = (sender as Button).CommandParameter.ToString();
            var item = tabControl_DynamicTabs.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            if (item is TabItem) {
                if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", item.Header.ToString()), "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    // Get selected tab
                    TabItem selectedTab = tabControl_DynamicTabs.SelectedItem as TabItem;

                    // Unhook DataContext vector, remove the tab from said vector and hook back the vector to the DataContext property of the control
                    tabControl_DynamicTabs.DataContext = null;
                    tabItems.Remove(item);
                    tabControl_DynamicTabs.DataContext = tabItems;

                    // If the tab closed was the current tab, then select first tab
                    if (selectedTab == null || selectedTab.Equals(item)) {
                        selectedTab = tabItems[0];
                    }
                    tabControl_DynamicTabs.SelectedItem = selectedTab;
                }
            }
        }

        public void RenameTab_Handler(object sender, RoutedEventArgs args) {
            TabItem tab = (((sender as MenuItem).Parent as ContextMenu).PlacementTarget as TabItem);
            try {
                Popup menu = tabControl_DynamicTabs.Resources["ContextMenu"] as Popup;
                menu.PlacementTarget = tab;
                menu.Width = tab.ActualWidth;
                (menu.Child as TextBox).Text = tab.Header.ToString();
                menu.IsOpen = true;
            } catch (System.Exception e) { System.Console.WriteLine("Stack: "+e.StackTrace+"\nCause: "+e.Message); }
        }

        public void HelperTBLostFocus_Handler(object sender, KeyboardFocusChangedEventArgs args) {
            //Change name and close popup
            try {
                TextBox helperTB = sender as TextBox;
                helperTB.Text = NameTab(1, helperTB.Text);
                TabItem tab = (helperTB.Parent as Popup).PlacementTarget as TabItem;
                tab.Header = helperTB.Text;
                tab.Name = "";
                foreach(char c in helperTB.Text) {
                    if (c != ' ') tab.Name += c;
                }
            } catch (System.Exception e) { System.Console.WriteLine("Stack: "+e.StackTrace+"\nCause: "+e.Message); }
        }

        public void Drag_Handler(object sender, MouseEventArgs args) {
            if (sender is TabItem && args.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(sender as TabItem, sender, DragDropEffects.Move);
            }
        }

        public void GiveFeedback_Handler(object sender, GiveFeedbackEventArgs args) {
            /*Cursor = Cursors.Hand;
            args.Handled = true;*/
        }

        public void Drop_Handler(object sender, DragEventArgs args) {
            TabItem sourceTab = args.Data.GetData(typeof(TabItem)) as TabItem;
            if (sender is TabItem) {
                tabControl_DynamicTabs.DataContext = null;
                int index = tabItems.IndexOf(sender as TabItem);
                tabItems.RemoveAt(tabItems.IndexOf(sourceTab));
                tabItems.Insert(index, sourceTab);
                tabControl_DynamicTabs.DataContext = tabItems;
                tabControl_DynamicTabs.SelectedItem = sourceTab;
            }
        }
    }
}
