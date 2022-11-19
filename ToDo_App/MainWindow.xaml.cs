using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ToDo_App.Helpers;
using ToDo_App.Models;

namespace ToDo_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static EmailWindow? _emailWindow;
        private static WeatherWindow? _weatherWindow;
        private bool _isEditingExitingItem = false;
        private ToDoListItem? _toDoListItem;
        private List<ToDoListItem>? _toDoListItemsList = new();
        public MainWindow()
        {
            InitializeComponent();

            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontFamily.SelectedItem = FontFamily; // Initialize combo box so it's not blank

            RefreshListViews();
        }

        private static void SuggestUserToSignInToEmail()
        {
            MessageBox.Show("Must be logged in to send emails.", "Email Sign-In Required", MessageBoxButton.OK, MessageBoxImage.Error);

            if (_emailWindow == null || _emailWindow.IsLoaded == false)
            {
                _emailWindow = new();
            }

            _emailWindow.Show();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ClearEditorFields();

            tabEditor.Focus();
            rtbEditor.Focus();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DisableTabButtons();

            gridDeleteItem.Visibility = Visibility.Visible;
            gridDeleteItem.Focus();
        }

        private void btnDeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            gridDeleteItem.Visibility = Visibility.Hidden;
            EnableTabButtons();
            lvSelectDeleteItem.SelectedItem = null;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            DisableTabButtons();

            gridEditItem.Visibility = Visibility.Visible;
            gridEditItem.Focus();
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            gridEditItem.Visibility = Visibility.Hidden;
            EnableTabButtons();
            lvSelectEditItem.SelectedItem = null;
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            if (_emailWindow == null || _emailWindow.IsLoaded == false)
            {
                _emailWindow = new();
            }

            _emailWindow.Show();
        }

        private void btnSaveToList_Click(object sender, RoutedEventArgs e)
        {
            string editorContentsString = Gather_rtbEditor_Contents();

            if (editorContentsString == null || editorContentsString == "") return;

            if (_isEditingExitingItem == true && _toDoListItem != null)
            {
                _toDoListItem.Contents = editorContentsString;
                _toDoListItem.Title = tbTitle.Text;
                _toDoListItem.Date = datePicker.SelectedDate;

                SqliteDataAccess.UpdateToDoListItem(_toDoListItem);

                _toDoListItem = null;
                _isEditingExitingItem = false;
            }
            else
            {
                SqliteDataAccess.SaveToDoListItem(
                    new ToDoListItem()
                    {
                        Contents = editorContentsString,
                        Title = tbTitle.Text,
                        Date = datePicker.SelectedDate
                    });
            }

            ClearEditorFields();
            RefreshListViews();
        }

        private void btnSendAsEmail_Click(object sender, RoutedEventArgs e)
        {
            if (!EmailHelpers.IsLoggedIn())
            {
                SuggestUserToSignInToEmail();
                return;
            }

            string editorContentsString = Gather_rtbEditor_Contents();
            if (editorContentsString == null || editorContentsString == "") return;

            string subject = tbTitle.Text;
            DateTime? itemDate = datePicker.SelectedDate;

            btnSaveToList_Click(this, new RoutedEventArgs());

            EmailHelpers.CreateAndSendEmailAsync(editorContentsString, subject, itemDate);
        }

        private void btnSendAsSMS_Click(object sender, RoutedEventArgs e)
        {
            if (!EmailHelpers.IsLoggedIn())
            {
                SuggestUserToSignInToEmail();
                return;
            }

            string editorContentsString = Gather_rtbEditor_Contents();
            if (editorContentsString == null || editorContentsString == "") return;

            string subject = tbTitle.Text;
            DateTime? itemDate = datePicker.SelectedDate;

            btnSaveToList_Click(this, new RoutedEventArgs());

            string? smsDetails = PhonePromptDialog.PhoneNumberPrompt();

            if (smsDetails != null)
            {
                EmailHelpers.CreateAndSendSmsAsync(editorContentsString, subject, itemDate, smsDetails);
            }
        }

        private void btnToDoList_Click(object sender, RoutedEventArgs e)
        {
            tabCreateEditDelete.Focus();
        }

        private void btnWeather_Click(object sender, RoutedEventArgs e)
        {
            if (_weatherWindow == null || _weatherWindow.IsLoaded == false)
            {
                _weatherWindow = new();
            }

            _weatherWindow.Show();
        }

        private void ClearEditorFields()
        {
            tbTitle.Clear();
            datePicker.SelectedDate = null;
            rtbEditor.Document.Blocks.Clear();
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            }
        }

        private void DisableTabButtons()
        {
            tabStartMenu.IsEnabled = false;
            tabEditor.IsEnabled = false;
            tabViewItem.IsEnabled = false;
        }

        private void EnableTabButtons()
        {
            tabStartMenu.IsEnabled = true;
            tabEditor.IsEnabled = true;
            tabViewItem.IsEnabled = true;
        }

        private string Gather_rtbEditor_Contents()
        {
            var editorContents = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            return editorContents.Text.TrimEnd();
        }

        private void lvDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvDataBinding.SelectedItem != null)
            {
                ToDoListItem toDoListItemPreview = (ToDoListItem)lvDataBinding.SelectedItem;
                tbDisplay.Text = toDoListItemPreview.Contents;
            }
        }

        private void lvSelectDeleteItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridDeleteItem.Visibility = Visibility.Hidden;

            EnableTabButtons();

            var deletedItem = (ToDoListItem)lvSelectDeleteItem.SelectedItem;

            if (deletedItem != null)
            {
                SqliteDataAccess.DeleteToDoListItem(deletedItem);
            }

            RefreshListViews();

            tabCreateEditDelete.Focus();
        }

        private void lvSelectEditItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridEditItem.Visibility = Visibility.Hidden;

            EnableTabButtons();

            if (lvSelectEditItem.SelectedItem != null)
            {
                _isEditingExitingItem = true;

                _toDoListItem = (ToDoListItem)lvSelectEditItem.SelectedItem;

                ClearEditorFields();

                rtbEditor.Document.Blocks.Add(new Paragraph(new Run(_toDoListItem.Contents)));
                tbTitle.Text = _toDoListItem.Title;
                datePicker.SelectedDate = _toDoListItem.Date;
            }

            tabEditor.Focus();
            rtbEditor.Focus();

            return;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true)
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                    ClearEditorFields();
                    range.Load(fileStream, DataFormats.Rtf);
                    fileStream.Close();
                }
            }
            catch
            {
                Console.WriteLine("Error when opening selected file.");
            }
        }

        // Not efficient to reload entire db everytime a create/edit/delete operation occurs but minor objective of this project is
        // to learn Dapper and lower level SQL db interactions. More efficient implementation would be to keep items list in memory
        // and additionally perform the same modifications on the local in-memory list as being performed on the SQLite db
        private void RefreshListViews()
        {
            _toDoListItemsList = SqliteDataAccess.LoadToDoListTable();

            lvDataBinding.ItemsSource = _toDoListItemsList;
            lvSelectEditItem.ItemsSource = _toDoListItemsList;
            lvSelectDeleteItem.ItemsSource = _toDoListItemsList;
            tbDisplay.Text = "";
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true)
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Rtf);
                    fileStream.Close();
                }

                btnSaveToList_Click(this, new RoutedEventArgs());
            }
            catch
            {
                Console.WriteLine("Error when saving to file.");
            }
        }

        // If item is selected for editing, then user navigates away from editor tab, then de-select the edited item.
        // Prevents accidental edits of list items if user navigates away from editor tab and forgets item editing is still in memory.
        private void tabItems_GotFocus_ClearEditedItem(object sender, RoutedEventArgs e)
        {
            if (_isEditingExitingItem)
            {
                ClearEditorFields();
            }

            _toDoListItem = null;
            _isEditingExitingItem = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
            return;
        }
    }
}