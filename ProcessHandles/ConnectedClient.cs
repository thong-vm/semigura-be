using System.Net.WebSockets;

namespace semigura.ProcessHandles
{
    public class ConnectedClient
    {
        public static readonly int FrameDicItemMax = 20;

        public ConnectedClient(string socketId, WebSocket socket, TaskCompletionSource<object> taskCompletion)
        {
            SocketId = socketId;
            Socket = socket;
            TaskCompletion = taskCompletion;
            StreamViewerClientIds = new List<string>();
        }

        public string SocketId { get; private set; }
        public WebSocket Socket { get; private set; }
        public TaskCompletionSource<object> TaskCompletion { get; private set; }
        public List<string> StreamViewerClientIds { get; set; }
        public bool IsStreaming { get; set; }
    }
}