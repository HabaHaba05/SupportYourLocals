using Microsoft.Win32;
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
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        string user;
        string pass;
        public Profile(string username, string password)
        {
            InitializeComponent();
            GetInfo(username);
            user = username;
            pass = password;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\desktop";
            dlg.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            dlg.RestoreDirectory = true;
            var fileName = "";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string selectedFileName = dlg.FileName;
                fileName = selectedFileName;
                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource = new BitmapImage(new Uri(selectedFileName));
                Elipse.Fill = myBrush;
            }
            
            string[] lines = File.ReadAllLines(@"..\LoginInfo.txt");
            using (StreamWriter writer = new StreamWriter(@"..\LoginInfo.txt"))
            {
                for (int currentLine = 0; currentLine < lines.Length; currentLine++)
                {
                    string[] line = lines[currentLine].Split('`');
                    if (line[0] == UsernameBox.Text)
                    {
                        writer.Write(line[0] + "`" + line[1] + "`" + fileName + "`");
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

        }

        private void EditProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            int j = 0;
            if (EditProfileBtn.Content.ToString() == "Save")
            {
                string[] lines = File.ReadAllLines(@"..\LoginInfo.txt");
                using (StreamWriter writer = new StreamWriter(@"..\LoginInfo.txt"))
                {
                    for (int currentLine = 0; currentLine < lines.Length; currentLine++)
                    {
                        string[] line = lines[currentLine].Split('`');
                        if (line[0] == UsernameBox.Text)
                        {
                            writer.Write(line[0] + "`" + line[1] + "`" + line[2] + "`" + NameBox.Text + "`" + LastNameBox.Text + "`" + AdressBox.Text + Environment.NewLine);
                        }
                        else
                        {
                            var lenght = line.Length;
                            for (int i = 0; i < lenght; i++)
                            {
                                if (i != lenght - 1)
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
                ChangeProperties(true, "Edit profile");
                j = 1;

            }
            if (j != 1)
            {
                ChangeProperties(false, "Save");
                j = 0;
            }
        }

        private void ChangeProperties (bool tf, string buttonContent)
        {
            UsernameBox.IsReadOnly = tf;
            NameBox.IsReadOnly = tf;
            LastNameBox.IsReadOnly = tf;
            AdressBox.IsReadOnly = tf;
            EditProfileBtn.Content = buttonContent;
        }

        private void GetInfo(string username)
        {
            UsernameBox.Text = username;
            string[] lines = File.ReadAllLines(@"..\LoginInfo.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                string[] line = lines[i].Split('`');
                if (line[0] == username && line.Length >= 3)
                {
                    if (line[2] != "noPhoto")
                    {
                        var selectedFileName = line[2];
                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource = new BitmapImage(new Uri(selectedFileName));
                        Elipse.Fill = myBrush;
                    }
                    if (line.Length == 4)
                    {
                        NameBox.Text = line[3];
                    }
                    if (line.Length == 5)
                    {
                        NameBox.Text = line[3];
                        LastNameBox.Text = line[4];
                    }
                    if (line.Length == 6)
                    {
                        NameBox.Text = line[3];
                        LastNameBox.Text = line[4];
                        AdressBox.Text = line[5];
                    }
                }
            }
        }

        private void mapBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            PasswordChange change = new PasswordChange(user, pass);
            change.Show();

        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
