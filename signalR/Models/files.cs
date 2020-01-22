using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace signalR.Models
{
    public class files
    {
        public HttpPostedFileWrapper ImageFile { get; set; }
        public string message_from { get; set; }
        public string message_to { get; set; }
    }
}