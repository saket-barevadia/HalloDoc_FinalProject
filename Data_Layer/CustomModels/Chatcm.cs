using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.CustomModels
{
    public class Chatcm
    {
        public int? ChatId { get; set; }

        public int? AdminId { get; set; }

        public int? ProviderId { get; set; }

        public int? RequestId { get; set; }

        public string? Message { get; set; }

        public DateTime ChatDate { get; set; }

        public int? SentBy { get; set; }
        public int? Flag { get; set; }

        public string? ChatBoxClass { get; set; }

        public string? RecieverName { get; set; }

        public List<Chatcm> chats { get; set; }
    }
}
