using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Messages
{
    public class MessagesOptions
    {
        public string TemplateName { get; set; }
        public string FrontURL { get; set; }
        public string FrontURLAdmin { get; set; }
        public string EmailFrom { get; set; }
        public string EmailFromPassword { get; set; }
    }
}
