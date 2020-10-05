using System.ComponentModel;


namespace SuppLocals
{
    public class ValidateUsername : IDataErrorInfo
    {
        public ValidateUsername()
        {
            /* Set default Username and email */
            this.Username = "";
            //this.Email = "";
        }

        public string Username { get; set; }
        //public string Email { get; set; }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Username":
                        if (this.Username == "")
                            return "Username can not be empty!";
                        if (this.Username.Length < 5)
                            return "Username has to contain from 5 to 12 symbols!";
                        break;

                    /*case "Email":
                        if (this.Email == "")
                            return "Email can not be empty!";

                        break;*/
                }

                return string.Empty;
            }
        }
    }
}