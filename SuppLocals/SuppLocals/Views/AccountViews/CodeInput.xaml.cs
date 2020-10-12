using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SuppLocals.Views
{
public partial class CodeInput : Window
    {
        public User ActiveUser;


        public CodeInput(User user)
        {
            ActiveUser = user;
            InitializeComponent();
        }

    public void Apply_Button_Click(object sender, RoutedEventArgs e)
    {
            EmailSender emailSender = new EmailSender();
            

            if (emailSender.HoldCode() == CodeTextBox.Text)
            {

                MainWindow map = new MainWindow(ActiveUser);
                map.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(emailSender.HoldCode());
            }

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }


}
