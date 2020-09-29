using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LogInBtnClick(object sender, RoutedEventArgs e)
        {
            bool discovered = false;
            string username = Username.Text;
            string password = PasswordBox.Password.ToString();

            string path = @"..\LoginInfo.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            else if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(@"..\LoginInfo.txt");

                if (username != "" && password != "")
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] line = lines[i].Split('`');
                        if (line[0] == username && line[1] == password)
                        {
                            discovered = true;
                            Profile profile = new Profile(line[0], line[1]);
                            profile.Show();
                            this.Close();
                            /*MainWindow map = new MainWindow();
                            map.Show();
                            this.Close();*/
                        }
                    }
                    if (discovered == false)
                    {
                        MessageBox.Show("Username or password is incorrect.");
                        Username.Clear();
                        PasswordBox.Clear();
                    }
                }
            }
        }

        private void SignUpBtnClick(object sender, RoutedEventArgs e)
        {
            /*Profile profile = new Profile();
            profile.Show();*/

            SignUp signUpForm = new SignUp();
            signUpForm.Show();
            this.Close();

        }

        private void ContinueBtnClick(object sender, RoutedEventArgs e)
        {
            MainWindow map = new MainWindow();
            map.Show();
            this.Close();
        }
    }
}
