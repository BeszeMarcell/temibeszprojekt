using car.ViewModels;
using System;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using System.IO;
using car.Views.Windows;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace car.Views.Pages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataPage : INavigableView<ViewModels.DataViewModel>
    {


        private INavigationControl navigationControl;

        public ViewModels.DataViewModel ViewModel
        {
            get;
        }

        public event EventHandler ButtonClicked;
        public int key = 3;
        public DataPage(ViewModels.DataViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        public interface INavigationControl
        {
            void NavigateTo(string pageTag);
        }

        private string Decrypt(string encryptedPassword, int key)
        {
            string decryptedMessage = "";
            foreach (char c in encryptedPassword)
            {
                if (Char.IsLetter(c))
                {
                    char baseChar = Char.IsUpper(c) ? 'A' : 'a';
                    char decryptedChar = (char)(((int)c - baseChar - key + 26) % 26 + baseChar);
                    decryptedMessage += decryptedChar;
                }
                else
                {
                    decryptedMessage += c;
                }
            }
            return decryptedMessage;
        }
        private StringBuilder actualPassword = new StringBuilder();

        private void PasswordTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int caretIndex = log2.CaretIndex;
            string maskedText = new string('●', e.Text.Length);
            log2.Text = log2.Text.Insert(caretIndex, maskedText);
            actualPassword.Insert(caretIndex, e.Text);
            log2.CaretIndex = caretIndex + e.Text.Length;
            e.Handled = true;

        }
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                TextBox textBox = (TextBox)sender;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }

        private void CommandManager_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                TextBox textBox = (TextBox)e.Source;
                if (textBox.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                }
            }
        }
        private void reg2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && actualPassword.Length > 0)
            {
                actualPassword.Remove(actualPassword.Length - 1, 1);
            }
        }
        private void logbutton_Click(object sender, RoutedEventArgs e)
        {
            string username = log1.Text;
            string encryptedPassword = actualPassword.ToString();
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "password");
            string filePath = Path.Combine(folderPath, "registers.txt");
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        string storedUsername = parts[0].Trim();
                        string storedEncryptedPassword = parts[1].Trim();
                        string decryptedPassword = Decrypt(storedEncryptedPassword, key);

                        if (username == storedUsername && encryptedPassword == decryptedPassword)
                        {
                            log1.Clear();
                            log2.Clear();
                            MessageBox.Show("Sikeres bejelentkezés!", "Bejelentkezés", MessageBoxButton.OK, MessageBoxImage.Information);
                            (Application.Current.MainWindow as MainWindow)?.ProcessButtonClicked();
                            return;
                        }
                    }
                }


                log1.Clear();
                log2.Clear();
                actualPassword.Clear();

                MessageBox.Show("Hibás felhasználónév vagy jelszó!", "Bejelentkezés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                log1.Clear();
                log2.Clear();
                actualPassword.Clear();

                string nem = ("A regiszter fájl nem található!");
                MessageBox.Show(nem, "Bejelentkezés", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
