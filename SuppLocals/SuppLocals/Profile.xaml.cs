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
        public Profile(string username)
        {
            InitializeComponent();
            UsernameBox.Text = username;
            string[] lines = File.ReadAllLines(@"..\LoginInfo.txt");
            for(int i = 0; i< lines.Length; i++)
            {
                string[] line = lines[i].Split('`');
                if (line[0] == username && line.Length >= 3)
                {
                    var selectedFileName = line[2];
                    ImageBrush myBrush = new ImageBrush();
                    myBrush.ImageSource = new BitmapImage(new Uri(selectedFileName));
                    Elipse.Fill = myBrush;
                }
            }
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
                        writer.WriteLine(line[0] + "`" + line[1] + "`" + fileName);
                    }
                    else
                    {
                        if (line.Length == 2)
                        {
                            writer.WriteLine(line[0] + "`" + line[1]);
                        }
                        else
                        {
                            writer.WriteLine(line[0] + "`" + line[1] + "`" + line[2]);
                        }
                    }
                }
            }

        }
    }
}
