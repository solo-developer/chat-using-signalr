using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.IO;
using System.Web.Mvc;

namespace signalR
{
    [HubName("chatHub")]
    public class chatHub : Hub
    {
      
        public void send(string name,string message,string msg_from,string msg_to)
        {
            Clients.All.broadcastMessage(name,message,msg_from,msg_to);
        }
        public void saveUserDetails(string user_name)
        {
          
            try
            {
             int count=   globalVariables.user_name.Length;
                int index = Array.IndexOf(globalVariables.user_name, null);
                globalVariables.user_name[index] = user_name;
            }
            catch (Exception ex)
            {
               
                throw ex;
            }

        }
        
       public void shareUsername(string username)
        {
            Clients.All.getUsername(username);
        }

        public void removeUsername(string username)
        {
            int index = Array.IndexOf(globalVariables.user_name, username);
            List<string> users = globalVariables.user_name.ToList();
            users.RemoveAt(index);
            globalVariables.user_name = users.ToArray();

            int result = globalVariables.user_name.Count(s => s != null);
            if (result == 0)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/shared images"));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            Clients.All.removedUser(username);
        }
      
        public void changeUsername(string original_username,string new_username)
        {
        int index= Array.IndexOf(globalVariables.user_name, original_username);
            globalVariables.user_name[index] = new_username;
            Clients.Others.changedUsername(original_username, new_username);
        }
       
        public static void send_files(string message_from,string message_to,string file_path)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<chatHub>();
            context.Clients.All.receive_files(message_from, message_to, file_path);
        }

        public void check_username(string user_name)
        {
            bool is_allowed = false;
            int index = Array.IndexOf(globalVariables.user_name, user_name);
            if (index == -1)
                is_allowed = true;
            Clients.Caller.isUserAllowed(is_allowed);
        }
    }
}