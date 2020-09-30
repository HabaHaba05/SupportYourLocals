using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for PasswordChange.xaml
    /// </summary>
    public partial class PasswordChange : Window
    {
        string user;
        string pass;
        public PasswordChange(string username, string password)
        {
            InitializeComponent();
            user = username;
            pass = password;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var oldPassword = oldPasswordBox.Password.ToString();
            var newPassword = newPasswordBox.Password.ToString();
            var confPassword = confNewPassBox.Password.ToString();
            if (oldPassword != "" && newPassword != "" && confPassword != "" && pass.Equals(oldPassword) && newPassword.Equals(confPassword))
            {

                string[] lines = File.ReadAllLines(@"..\LoginInfo.txt");
                using (StreamWriter writer = new StreamWriter(@"..\LoginInfo.txt"))
                {
                    for (int currentLine = 0; currentLine < lines.Length; currentLine++)
                    {
                        string[] line = lines[currentLine].Split('`');
                        if (user == line[0])
                        {
                            writer.Write(line[0] + "`" + newPassword + "`" + line[2] + "`");
                            if (line.Length > 3)
                            {
                                for (int i = 3; i < line.Length; i++)
                                {
                                    if (i != line.Length - 1)
                                    {
                                        writer.Write(line[i] + "`");
                                    }
                                    else
                                    {
                                        writer.Write(line[i] + Environment.NewLine);
                                    }
                                }
                            }
                            writer.Write(Environment.NewLine);
                            MessageBox.Show("Password was changed");

                        }
                        else
                        {
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (i != line.Length - 1)
                                {
                                    writer.Write(line[i] + "`");
                                }
                                else
                                {
                                    writer.Write(line[i] + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Old password is incorrect or new password does not match confirm password");
                oldPasswordBox.Password = "";
                newPasswordBox.Password = "";
                confNewPassBox.Password = "";
            }
        }
    }
}
