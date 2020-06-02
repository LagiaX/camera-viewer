using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CV.UserControls {
    
    public partial class TBSlider : UserControl {

        protected double currentValue;

        public TBSlider() {
            InitializeComponent();

            Init();
        }

        public void Init() {
            slider_TBSlider.Value = 0.5;
        }

        public double Value {
            get { return currentValue; }
            set {
                if (value > MaxValue) currentValue = MaxValue;
                else if (value < MinValue) currentValue = MinValue;
                else currentValue = value;
            }
        }

        public double MaxValue {
            get { return slider_TBSlider.Maximum; }
            set {
                if (value >= MinValue) slider_TBSlider.Maximum = value;
            }
        }

        public double MinValue {
            get { return slider_TBSlider.Minimum; }
            set {
                if (value <= MaxValue) slider_TBSlider.Minimum = value;
            }
        }

        public override string ToString() {
            return tb_TBSlider.Text;
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
            if (args.Text == "." && tb_TBSlider.Text.Contains(".")) args.Handled = true;
            else if (args.Text == "-" && tb_TBSlider.Text.Contains("-")) args.Handled = true;
            else args.Handled = !Utilities.IsNumericKey(args.Text);
        }

        public void TextValidation_Handler(object sender, TextChangedEventArgs args) {
            TextBox tb = sender as TextBox;
            if (tb.Text != "" && tb.Text != "-" && tb.Text != ".") {
                if (Utilities.IsNumeric(tb.Text)) { //Valid value, more than a single character
                    Value = double.Parse(tb.Text);
                }
                else tb.Text = currentValue.ToString();
            }
        }

        public void PasteValidation_Handler(object sender, DataObjectPastingEventArgs args) {
            if (!Utilities.IsNumeric(args.DataObject.GetData(typeof(string)) as string)) {
                args.CancelCommand();
            }
        }
        
        public void TypeCancel_Handler(object sender, KeyboardFocusChangedEventArgs args) {
            if (tb_TBSlider.Text == "" || tb_TBSlider.Text == "-" || tb_TBSlider.Text == ".") {
                tb_TBSlider.Text = currentValue.ToString();
                args.Handled = true;
            }
            else {
                Value = double.Parse(tb_TBSlider.Text);
                tb_TBSlider.Text = currentValue.ToString();
            }
        }
    }
}
