using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

//
//
//SERVER
//
//

namespace SharedNet
{
    public static class Wire
    {
        // Gửi: 4 byte length (Big Endian / network order) + JSON UTF-8
        public static async Task WriteJsonAsync(NetworkStream stream, object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);

            int len = data.Length;
            byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len));

            await stream.WriteAsync(header, 0, header.Length);
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }

        // Đọc 1 frame JSON, trả null nếu peer đóng kết nối trước khi đọc header
        public static async Task<string> ReadJsonRawAsync(NetworkStream stream)
        {
            byte[] header = new byte[4];
            int gotHeader = await ReadExactlyAsync(stream, header, 0, 4);
            if (gotHeader == 0) return null;

            int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
            if (len <= 0 || len > 10_000_000)
                throw new IOException("Invalid frame length");

            byte[] payload = new byte[len];
            int gotPayload = await ReadExactlyAsync(stream, payload, 0, len);
            if (gotPayload < len)
                throw new IOException("Premature EOF payload");

            return Encoding.UTF8.GetString(payload);
        }

        // ReadExactly cho .NET Framework
        public static async Task<int> ReadExactlyAsync(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int total = 0;
            while (total < count)
            {
                int r = await stream.ReadAsync(buffer, offset + total, count - total);
                if (r == 0) return total; // EOF
                total += r;
            }
            return total;
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    // ====== POCO thay cho 'record' ======
    public class HelloMsg
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public HelloMsg() { }
        public HelloMsg(string type, string name) { Type = type; Name = name; }
    }

    public class BroadcastMsg:BaseMsg
    {
        public string Type { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public string FileName { get; set; }  // tên file hoặc folder.zip
        public long FileSize { get; set; }
        public BroadcastMsg() { }
        public BroadcastMsg(string type, string from, string text) { Type = type; From = from; Text = text; }
        public BroadcastMsg(string type, string from, string fileName, long fileSize)
        {
            Type = type;
            From = from;
            FileName = fileName;
            FileSize = fileSize;
        }
    }

    public class ServerNotice
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public ServerNotice() { }
        public ServerNotice(string type, string text) { Type = type; Text = text; }
    }
    public class BaseMsg
    {
        public string Type { get; set; }

        public BaseMsg() { }

        public BaseMsg(string type)
        {
            Type = type;
        }
    }
}
