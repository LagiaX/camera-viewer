using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CV.UserControls {

    public partial class AxisTB : UserControl {

        public static readonly RoutedEvent ChangedEvent = EventManager.RegisterRoutedEvent(
                                                              "Changed",
                                                              RoutingStrategy.Bubble,
                                                              typeof(RoutedEventHandler),
                                                              typeof(AxisTB)
                                                          );
        protected double maxValue;
        protected double minValue;
        protected bool isAngleRotation;
        protected double currentValue;

        public AxisTB() {
            InitializeComponent();

            Init();
        }

        public void Init() {
            Value = 0.0;
            IsAngleRotation = false;
            tb_AxisTB.Text = Value.ToString();
            Changed += OnChanged_Handler;
        }

        public double Value {
            get { return currentValue; }
            set {
                if (value != currentValue) {
                    if (value > MaxValue) currentValue = MaxValue;
                    else if (value < MinValue) currentValue = MinValue;
                    else currentValue = value;
                    RaiseChangedEvent();
                }
            }
        }

        public double MaxValue { get; set; } = double.PositiveInfinity;

        public double MinValue { get; set; } = double.NegativeInfinity;

        public bool IsAngleRotation {
            get { return isAngleRotation; }
            set {
                if (value == true) {
                    MaxValue = 359;
                    MinValue = 0;
                }
                isAngleRotation = value;
            }
        }

        public event RoutedEventHandler Changed {
            add { AddHandler(ChangedEvent, value); }
            remove { RemoveHandler(ChangedEvent, value); }
        }

        public void PlusValue() {
            double number = Value + 1.0;
            if (IsAngleRotation) Value = number % 360;
            else {
                if (number < MaxValue) Value = number;
                else Value = MaxValue;
            }
        }

        public void MinusValue() {
            double number = Value - 1.0;
            if (IsAngleRotation) Value = (360 + number) % 360;
            else {
                if (number > MinValue) Value = number;
                else Value = MinValue;
            }
        }

        public void RaiseChangedEvent() {
            RoutedEventArgs eventArgs = new RoutedEventArgs(ChangedEvent);
            RaiseEvent(eventArgs);
        }

        public override string ToString() {
            return currentValue.ToString();
        }

        //HANDLERS
        public void KeyConfirm_Handler(object sender, KeyEventArgs args) {
            if (args.Key == Key.Enter) {
                Keyboard.ClearFocus();
            }
            else if (args.Key == Key.Space) {
                args.Handled = true;
            }
        }

        public void KeyFilter_Handler(object sender, TextCompositionEventArgs args) {
            if (args.Text == "." && tb_AxisTB.Text.Contains(".")) args.Handled = true;
            else if (args.Text == "-" && tb_AxisTB.Text.Contains("-")) args.Handled = true;
            else args.Handled = !Utilities.IsNumericKey(args.Text);
        }

        public void TextValidation_Handler(object sender, TextChangedEventArgs args) {
            TextBox tb = sender as TextBox;
            if (tb.Text != "" && tb.Text != "-" && tb.Text != ".") {
                if (Utilities.IsNumeric(tb.Text) && tb.Text[tb.Text.Length - 1] != '.') { //Valid value, more than a single character
                    Value = double.Parse(tb.Text);
                }
            }
        }

        public void PasteValidation_Handler(object sender, DataObjectPastingEventArgs args) {
            if (!Utilities.IsNumeric(args.DataObject.GetData(typeof(string)) as string)) {
                args.CancelCommand();
            }
        }

        public void TypeCancel_Handler(object sender, KeyboardFocusChangedEventArgs args) {
            if (tb_AxisTB.Text == "" || tb_AxisTB.Text == "-" || tb_AxisTB.Text == ".") {
                tb_AxisTB.Text = currentValue.ToString();
            }
            else {
                Value = double.Parse(tb_AxisTB.Text);
            }
        }

        public void OnChanged_Handler(object sender, RoutedEventArgs args) {
            tb_AxisTB.Text = currentValue.ToString();
        }

        public void HoverWheel_Handler(object sender, MouseWheelEventArgs args) {
            if (args.Delta > 0) PlusValue();
            else if (args.Delta < 0) MinusValue();
            tb_AxisTB.Text = currentValue.ToString();
        }

        public void ClickInside_Handler(object sender, MouseButtonEventArgs args) {
            CaptureMouse();
        }

        public void ClickOutside_Handler(object sender, MouseButtonEventArgs args) {
            ReleaseMouseCapture();
            Keyboard.ClearFocus();
        }

        public void Plus_Handler(object sender, RoutedEventArgs args) {
            PlusValue();
            tb_AxisTB.Text = currentValue.ToString();
        }

        public void Minus_Handler(object sender, RoutedEventArgs args) {
            MinusValue();
            tb_AxisTB.Text = currentValue.ToString();
        }
    }
}
