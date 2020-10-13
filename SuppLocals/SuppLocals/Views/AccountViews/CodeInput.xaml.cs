using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuppLocals.Views
{
    public partial class CodeInput : Window
    {
        public User ActiveUser;
        public string Email;
        public string Code;
 

        public CodeInput(User user, string email , string code)
        {
            
            ActiveUser = user;
            Email = email;
            Code = code;
            InitializeComponent();
        }

        public void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            EmailSender emailSender = new EmailSender();


            if (Code == CodeTextBox.Text)
            {

                MainWindow map = new MainWindow(ActiveUser);
                map.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(Code);
            }

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }


    }
}
