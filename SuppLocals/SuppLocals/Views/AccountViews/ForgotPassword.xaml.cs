using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuppLocals.Views
{

    public partial class ForgotPassword : Window
    {
        
        public ForgotPassword()
        {
            InitializeComponent();
            this.DataContext = new ValidateUsername();
        }

        public void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            var email = emailForForgotPassword.Text;
            EmailSender emailSender = new EmailSender();
            emailSender.SendEmail(email);

            using UsersDbTable db = new UsersDbTable();
            var usersList = db.Users.ToList();
            var user = usersList.SingleOrDefault(x => (x.Email == email));

            CodeInput codeInput = new CodeInput(user);
            codeInput.Show();
            Close();

        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void EmailTextBoxChanged(object sender, TextChangedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(emailForForgotPassword.Text))
            {
                sendBtn.IsEnabled = false;
            }
            else
            {
                sendBtn.IsEnabled = true;
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

    }
}
