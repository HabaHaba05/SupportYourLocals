using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SuppLocals
{
    public class Review
    { 
        //Constructor
        public Review(Image image, string sender, string text)
        {
            this.Sender = sender;
            this.Text = text;
            this.Image = image;
        }

        //Sender
        public string Sender { get; set; }

        //Sender blank image
        public Image Image { get; set; }

        //Review
        public string Text { get; set; }
    }
}
