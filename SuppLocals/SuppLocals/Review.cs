using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SuppLocals
{
    public class Review
    { 
        //Constructor
        public Review(string sender, string text, string date)
        {
            this.Sender = sender;
            this.Text = text;
            this.Date = date;
        }

        //Sender
        public string Sender { get; set; }

        //Review
        public string Text { get; set; }

        // Publishing date
        public string Date { get; set; }
    }
}
