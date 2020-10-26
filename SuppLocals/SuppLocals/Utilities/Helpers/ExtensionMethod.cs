using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SuppLocals.Classes
{
    public static class ExtensionMethod
    {
        public static bool IsEmail(this string email)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}