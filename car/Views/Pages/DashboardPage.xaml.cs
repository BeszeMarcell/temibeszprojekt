using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text;

namespace car.Views.Pages
{
    class Register
    {
        public string Username { get; set; }
        public string Pass { get; set; }
    }

    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        public int key = 3;
        private List<Register> registers = new List<Register>();
        private StringBuilder actualPassword = new StringBuilder();

        public ViewModels.DashboardViewModel ViewModel { get; }

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private string Encrypt(string password, int key)
        {
            string encryptedMessage = "";
            foreach (char c in password)
            {
                if (Char.IsLetter(c))
                {
                    char baseChar = Char.IsUpper(c) ? 'A' : 'a';
                    char encryptedChar = (char)(((int)c - baseChar + key) % 26 + baseChar);
                    encryptedMessage += encryptedChar;
                }
                else
                {
                    encryptedMessage += c;
                }
            }
            return encryptedMessage;
        }

        private void PasswordTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int caretIndex = reg2.CaretIndex;
            string maskedText = new string('●', e.Text.Length);
            reg2.Text = reg2.Text.Insert(caretIndex, maskedText);
            actualPassword.Insert(caretIndex, e.Text);
            reg2.CaretIndex = caretIndex + e.Text.Length;
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

        private void regbutton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string actualPasswordValue = actualPassword.ToString();

            Register register1 = new Register();
            register1.Username = reg1.Text;
            register1.Pass = actualPasswordValue;

            if (string.IsNullOrEmpty(register1.Username) || string.IsNullOrEmpty(register1.Pass))
            {
                string regjo = "Kérlek töltsd ki az összes mezőt!";
                System.Windows.MessageBox.Show(regjo, "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                actualPassword.Clear();
                return;
            }

            if (register1.Pass.Length < 8)
            {
                System.Windows.MessageBox.Show("A jelszónak legalább 8 karakter hosszúnak kell lennie!", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                actualPassword.Clear();
                return;
            }

            if (!register1.Pass.Any(char.IsUpper))
            {
                System.Windows.MessageBox.Show("A jelszónak tartalmaznia kell legalább egy nagybetűt!", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                actualPassword.Clear();
                return;
            }

            if (!Regex.IsMatch(register1.Pass, @"^[a-zA-Z0-9]+$"))
            {
                System.Windows.MessageBox.Show("A jelszó nem tartalmazhat ékezetes karaktereket vagy speciális karaktereket.", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                actualPassword.Clear();
                return;
            }

            registers.Add(register1);
            reg1.Clear();
            reg2.Clear();
            System.Windows.MessageBox.Show("Sikeres Regisztráció!", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "password");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "registers.txt");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Register register in registers)
                {
                    string encryptedPass = Encrypt(register.Pass, key);
                    writer.WriteLine($"{register.Username},{encryptedPass}");
                }
            }

            actualPassword.Clear();
        }
    }
}
