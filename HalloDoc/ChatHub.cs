using Data_Layer.CustomModels;
using Data_Layer.DataModels;
using Data_Layer.DataContext;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _db;

    public ChatHub(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task SendMessage(string user, string message, string RequestID, string adminId, string ProviderId, string sentBy, string flag)
    {
            if (Convert.ToInt32(flag) == 1)
            {
                Chat chat = new Chat()
                {
                    RequestId = Convert.ToInt32(RequestID),
                    AdminId = Convert.ToInt32(adminId),
                    PhyscainId = Convert.ToInt32(ProviderId),
                    Message = message,
                    SentDate = DateTime.Now,
                    SentBy = Convert.ToInt32(sentBy)
                };

                _db.Add(chat);
                _db.SaveChanges();
            }
            if (Convert.ToInt32(flag) == 2)
            {
                Chat chat = new Chat()
                {
                    RequestId = Convert.ToInt32(RequestID),
                    AdminId = Convert.ToInt32(adminId),
                    PhyscainId = null,
                    Message = message,
                    SentDate = DateTime.Now,
                    SentBy = Convert.ToInt32(sentBy)
                };

                _db.Add(chat);
                _db.SaveChanges();
            }
            if (Convert.ToInt32(flag) == 3)
            {
                Chat chat = new Chat()
                {
                    RequestId = Convert.ToInt32(RequestID),
                    AdminId = null,
                    PhyscainId = Convert.ToInt32(ProviderId),
                    Message = message,
                    SentDate = DateTime.Now,
                    SentBy = Convert.ToInt32(sentBy)
                };

                _db.Add(chat);
                _db.SaveChanges();
        }

        await Clients.All.SendAsync("ReceiveMessage",user, message);
    }
}