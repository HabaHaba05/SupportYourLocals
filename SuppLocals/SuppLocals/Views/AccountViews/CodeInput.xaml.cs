using System.Windows;
using System.Windows.Input;

namespace SuppLocals.Views
{
    public partial class CodeInput : Window
    {
        public User ActiveUser;
        public string Code;
        public string Email;


        public CodeInput(User user, string email, string code)
        {
            ActiveUser = user;
            Email = email;
            Code = code;
            InitializeComponent();
        }

        public void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Code == CodeTextBox.Text)
            {
                var map = new MainWindow(ActiveUser);
                map.Show();
                Close();
            }
            else
            {
                MessageBox.Show(Code);
            }
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            Close();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}