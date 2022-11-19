using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ToDo_App
{
    /// <summary>
    /// Interaction logic for PhonePromptDialog.xaml
    /// </summary>
    public partial class PhonePromptDialog : Window
    {
        public PhonePromptDialog()
        {
            InitializeComponent();
        }

        public static string? PhoneNumberPrompt()
        {
            PhonePromptDialog inst = new PhonePromptDialog();
            inst.ShowDialog();

            if (inst.DialogResult != true ||
                inst.tboxPhoneNumber.Text.Length != 10 ||
                inst.cboxCarrier.SelectedItem == null)
            {
                MessageBox.Show("SMS operation canceled. Phone number must be 10 digits (##########) " +
                    "and phone carrier option must be selected.",
                    "SMS Operation Canceled",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return null;
            }

            StringBuilder sb = new StringBuilder(inst.tboxPhoneNumber.Text);
            sb.Append("@");

            switch (inst.cboxCarrier.SelectionBoxItem.ToString())
            {
                case "Rogers":
                    sb.Append("pcs.rogers.com");
                    break;

                case "Bell":
                    sb.Append("txt.bell.ca");
                    break;

                case "Telus":
                    sb.Append("msg.telus.com");
                    break;

                default:
                    sb.Clear();
                    break;
            }

            return sb.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void tboxPhoneNumber_KeyDown(object sender, KeyEventArgs e) // Prevent spaces
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void tboxPhoneNumber_Validation(object sender, TextCompositionEventArgs e) // Prevent any non-numeric characters
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}