using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ToDo_App.Helpers;
using ToDo_App.Models;

namespace ToDo_App
{
    /// <summary>
    /// Interaction logic for EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        private Email? _email;

        public EmailWindow()
        {
            InitializeComponent();

            Set_TbEmailLoggedInStatus();
            SetEnable_btnEmailLogin();
            SetEnable_btnEmailLogout();
            SetEnable_btnEmailUpdateInbox();

            if (EmailHelpers.IsLoggedIn())
            {
                lvEmailOverview.ItemsSource = EmailHelpers.ReturnEmailInbox();
            }
        }

        private void btnEmailLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tboxLogin.Text;
            string password = pboxPassword.Password;
            LoginToEmailAsync(login, password);
        }

        private void btnEmailLogout_Click(object sender, RoutedEventArgs e)
        {
            tbEmailLoggedInStatus.Text = "Logged out";
            btnEmailLogin.IsEnabled = true;
            btnEmailLogout.IsEnabled = false;
            btnEmailUpdateInbox.IsEnabled = false;

            ClearLoginDetailInputBoxes();
            ClearAllDataBoxes();

            EmailHelpers.LogoutAsync();
        }

        private void btnEmailUpdateInbox_Click(object sender, RoutedEventArgs e)
        {
            if (!EmailHelpers.IsLoggedIn()) return;

            ClearAllDataBoxes();
            UpdateEmailListViewAsync();
        }

        private void btnReply_Click(object sender, RoutedEventArgs e)
        {
            if (_email is null) return;

            if (!EmailHelpers.IsLoggedIn())
            {
                MessageBox.Show("Must be logged in to send emails.", "Email Sign-In Required", MessageBoxButton.OK, MessageBoxImage.Error);

                tabEmailLogin.Focus();
                return;
            }

            string emailResponseBody = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd).Text.TrimEnd();

            EmailHelpers.ReplyToEmailAsync(_email, emailResponseBody);
            ClearReplyDataBoxes();
        }

        private void btnViewReply_Click(object sender, RoutedEventArgs e)
        {
            if (lvEmailOverview.SelectedItem == null) return;

            _email = lvEmailOverview.SelectedItem as Email;

            lblLoggedInAs.Content = EmailHelpers.LoggedInAs();
            lblReplyingTo.Content = _email.FromAddressList.ToString();
            lblEmailSubject.Content = _email.Subject;

            rtbEditor.Document.Blocks.Clear();
            rtbEditor.Document.Blocks.Add(new Paragraph(new Run("")));
            rtbEditor.Document.Blocks.Add(new Paragraph(new Run("------------ ORIGINAL MESSAGE ------------")));
            rtbEditor.Document.Blocks.Add(new Paragraph(new Run(_email.Body)));

            tabEmailReply.Focus();
            rtbEditor.Focus();
        }

        private void ClearAllDataBoxes()
        {
            lvEmailOverview.ItemsSource = null;

            tbSelectedEmailSubject.Text = null;
            tbSelectedEmailFrom.Text = null;
            tbSelectedEmailDate.Text = null;
            tbSelectedEmailBody.Text = null;

            ClearReplyDataBoxes();
        }

        private void ClearLoginDetailInputBoxes()
        {
            tboxLogin.Text = "";
            pboxPassword.Password = "";
        }

        private void ClearReplyDataBoxes()
        {
            _email = null;

            lblLoggedInAs.Content = null;
            lblReplyingTo.Content = null;
            lblEmailSubject.Content = null;

            rtbEditor.Document.Blocks.Clear();
        }

        private async Task LoginToEmailAsync(string login, string password)
        {
            if (!(new EmailAddressAttribute().IsValid(login)))
            {
                tbEmailLoggedInStatus.Text = "Please enter a valid email address.";
                return;
            }

            if (password == null || password == "")
            {
                tbEmailLoggedInStatus.Text = "Password is required.";
                return;
            }

            await EmailHelpers.LoginToEmailAsync(login, password);

            if (!EmailHelpers.IsLoggedIn()) // Do not continue if login unsuccessful
            {
                tbEmailLoggedInStatus.Text = "Email login failed.";
                return;
            }

            lvEmailOverview.ItemsSource = await EmailHelpers.RetrieveEmailsAsync();

            Set_TbEmailLoggedInStatus();
            SetEnable_btnEmailLogin();
            SetEnable_btnEmailLogout();
            SetEnable_btnEmailUpdateInbox();
            ClearLoginDetailInputBoxes();
        }
        private void lvEmailOverview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbSelectedEmailBody.FontStyle = FontStyles.Normal; // Revert text block contents if previously set
            tbSelectedEmailSubject.Text = "Subject: ";
            tbSelectedEmailFrom.Text = "Sender: ";
            tbSelectedEmailDate.Text = "Sent: ";

            if (lvEmailOverview.ItemsSource != null && lvEmailOverview.SelectedItem != null)
            {
                var email = lvEmailOverview.SelectedItem as Email;
                tbSelectedEmailBody.Text = email.Body;

                tbSelectedEmailSubject.Text += email.Subject;
                tbSelectedEmailFrom.Text += email.FromAddressList.ToString();
                tbSelectedEmailDate.Text += email.Date.DateTime.ToString();

                if (tbSelectedEmailBody.Text == "")
                {
                    tbSelectedEmailBody.FontStyle = FontStyles.Italic;
                    tbSelectedEmailBody.Text = "*** Message body is blank. ***";
                }
            }
        }

        private void Set_TbEmailLoggedInStatus()
        {
            if (EmailHelpers.IsLoggedIn())
            {
                tbEmailLoggedInStatus.Text = "Logged in as: " + EmailHelpers.LoggedInAs();
            }
            else tbEmailLoggedInStatus.Text = "Logged out";
        }

        private void SetEnable_btnEmailLogin()
        {
            if (EmailHelpers.IsLoggedIn())
            {
                btnEmailLogin.IsEnabled = false;
            }
            else btnEmailLogin.IsEnabled = true;
        }

        private void SetEnable_btnEmailLogout()
        {
            if (EmailHelpers.IsLoggedIn())
            {
                btnEmailLogout.IsEnabled = true;
            }
            else btnEmailLogout.IsEnabled = false;
        }

        private void SetEnable_btnEmailUpdateInbox()
        {
            if (EmailHelpers.IsLoggedIn())
            {
                btnEmailUpdateInbox.IsEnabled = true;
            }
            else btnEmailUpdateInbox.IsEnabled = false;
        }

        private async Task UpdateEmailListViewAsync()
        {
            lvEmailOverview.ItemsSource = await EmailHelpers.RetrieveEmailsAsync();
        }
    }
}