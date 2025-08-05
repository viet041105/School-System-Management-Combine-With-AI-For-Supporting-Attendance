using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backendd.Service
{
    public class VideoProcessingHub : Hub
    {
        public async Task JoinGroup(string videoId)
        {
            Console.WriteLine($"Client {Context.ConnectionId} joining group {videoId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, videoId);
        }
    }
}