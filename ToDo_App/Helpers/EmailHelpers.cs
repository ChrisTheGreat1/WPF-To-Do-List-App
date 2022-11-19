using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ToDo_App.Models;

namespace ToDo_App.Helpers
{
    public static class EmailHelpers
    {
        private const int _IMAPPORT = 993;
        private const int _POPPORT = 995;
        private const int _SMTPPORT = 587;
        private static List<Email>? _emailMessages = new();
        private static ImapClient? _imapClient;
        private static string _loggedInEmail = "";
        private static SmtpClient? _smtpClient;

        public static async Task CreateAndSendEmailAsync(string emailBody, string? emailSubject, DateTime? itemDate)
        {
            if (!IsLoggedIn()) return;

            try
            {
                var mailkitEmail = new MimeMessage();

                mailkitEmail.From.Add(MailboxAddress.Parse(EmailHelpers.LoggedInAs()));
                mailkitEmail.To.Add(MailboxAddress.Parse(EmailHelpers.LoggedInAs()));

                if (emailSubject != null && emailSubject != "")
                {
                    mailkitEmail.Subject = emailSubject;
                }
                else
                {
                    mailkitEmail.Subject = "To Do List Item, Created " + DateTime.Now.ToString();
                }

                mailkitEmail.Body = new TextPart(TextFormat.Text)
                {
                    Text = "Item Date/Deadline: " + itemDate.ToString() + Environment.NewLine + Environment.NewLine + emailBody
                };

                await _smtpClient.SendAsync(mailkitEmail);

                MessageBox.Show("Email successfully sent!", "Email successful");
            }
            catch
            {
                Console.WriteLine("Error sending email.");
                MessageBox.Show("Error sending email.", "Email unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static async Task CreateAndSendSmsAsync(string emailBody, string? emailTitle, DateTime? itemDate, string smsDetails)
        {
            if (!IsLoggedIn()) return;

            try
            {
                var mailkitEmail = new MimeMessage();

                mailkitEmail.From.Add(MailboxAddress.Parse(EmailHelpers.LoggedInAs()));

                mailkitEmail.To.Add(MailboxAddress.Parse(smsDetails));

                if (emailTitle != null && emailTitle != "")
                {
                    mailkitEmail.Subject = emailTitle;
                }
                else
                {
                    mailkitEmail.Subject = "To Do List Item, Created " + DateTime.Now.ToString();
                }

                mailkitEmail.Body = new TextPart(TextFormat.Text)
                {
                    Text = "Item Date/Deadline: " + itemDate.ToString() + Environment.NewLine + Environment.NewLine + emailBody
                };

                await _smtpClient.SendAsync(mailkitEmail);

                MessageBox.Show("SMS successfully sent!", "SMS successful");
            }
            catch
            {
                Console.WriteLine("Error sending SMS.");
                MessageBox.Show("Error sending SMS.", "SMS unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static bool IsLoggedIn()
        {
            if (
                    (_imapClient != null && _imapClient.IsAuthenticated && _imapClient.IsConnected)
                    &&
                    (_smtpClient != null && _smtpClient.IsAuthenticated && _smtpClient.IsConnected)
               ) return true;
            else return false;
        }

        public static string? LoggedInAs()
        {
            return _loggedInEmail;
        }

        public static async Task LoginToEmailAsync(string login, string password)
        {
            try
            {
                _imapClient = new ImapClient();
                _smtpClient = new SmtpClient();

                {
                    Task connectImapClient = _imapClient.ConnectAsync("outlook.office365.com", _IMAPPORT, true);
                    Task connectSmtpClient = _smtpClient.ConnectAsync("smtp-mail.outlook.com", _SMTPPORT, SecureSocketOptions.StartTls);
                    await connectImapClient;
                    await connectSmtpClient;

                    Task authenticateImapClient = _imapClient.AuthenticateAsync(login, password);
                    Task authenticateSmtpClient = _smtpClient.AuthenticateAsync(login, password);
                    await authenticateImapClient;
                    await authenticateSmtpClient;

                    _loggedInEmail = login;
                }
            }
            catch
            {
                _imapClient.Dispose();
                _smtpClient.Dispose();
                Console.WriteLine("Error logging into email.");
            }
        }

        public static async Task LogoutAsync()
        {
            if ((_imapClient == null) && (_smtpClient == null)) return;

            try
            {
                if (                                                                // If
                        (!(_imapClient.IsConnected || _imapClient.IsAuthenticated)) // IMAP client is not connected or authenticated
                        &&                                                          // and
                        (!(_smtpClient.IsConnected || _smtpClient.IsAuthenticated)) // SMTP client is not connected or authenticated
                   ) return;                                                        // Do nothing

                Task disconnectImapClient = _imapClient.DisconnectAsync(true);
                Task disconnectSmtpClient = _smtpClient.DisconnectAsync(true);
                await disconnectImapClient;
                await disconnectSmtpClient;

                _imapClient.Dispose();
                _smtpClient.Dispose();
            }
            catch
            {
                _imapClient.Dispose();
                _smtpClient.Dispose();
                Console.WriteLine("Error disconnecting and logging out of email.");
            }
        }

        public static async Task ReplyToEmailAsync(Email email, string emailResponseBody)
        {
            if (!IsLoggedIn()) return;

            try
            {
                var mailkitEmail = new MimeMessage();

                mailkitEmail.From.Add(MailboxAddress.Parse(EmailHelpers.LoggedInAs()));

                if (email.FromAddressList != null)
                {
                    mailkitEmail.To.Add(email.FromAddressList.First());
                }

                mailkitEmail.Subject = email.Subject;

                if (mailkitEmail.Subject.Substring(0, 4) != "Re: ")
                {
                    mailkitEmail.Subject = mailkitEmail.Subject.Insert(0, "Re: ");
                }

                mailkitEmail.Body = new TextPart(TextFormat.Text)
                {
                    Text = emailResponseBody
                };

                await _smtpClient.SendAsync(mailkitEmail);

                MessageBox.Show("Email successfully sent!", "Reply successful");
            }
            catch
            {
                Console.WriteLine("Error replying to email.");
                MessageBox.Show("Error sending email.", "Reply unsuccessful", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static async Task<List<Email>?> RetrieveEmailsAsync()
        {
            try
            {
                var emailInbox = _imapClient.Inbox;
                await emailInbox.OpenAsync(FolderAccess.ReadOnly);

                MimeMessage emailMessage;

                for (int i = 0; i < emailInbox.Count; i++)
                {
                    emailMessage = await emailInbox.GetMessageAsync(i);

                    _emailMessages.Add(new Email()
                    {
                        Date = emailMessage.Date,
                        FromAddressList = emailMessage.From,
                        Subject = emailMessage.Subject,
                        Body = emailMessage.TextBody
                    });
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error when retrieving emails.");
            }

            return _emailMessages;
        }

        public static List<Email>? ReturnEmailInbox()
        {
            return _emailMessages;
        }
    }
}