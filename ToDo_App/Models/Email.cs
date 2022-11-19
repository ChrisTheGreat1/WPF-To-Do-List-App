using System;

namespace ToDo_App.Models
{
    public class Email
    {
        public String? Body { get; set; }
        public DateTimeOffset Date { get; set; }
        public MimeKit.InternetAddressList? FromAddressList { get; set; }

        public String? Subject { get; set; }
    }
}