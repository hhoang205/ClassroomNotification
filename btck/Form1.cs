using SharedNet;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace btck
{
    public partial class Form1 : Form
    {
        private TcpListener _listener;
        private volatile bool _running = false;
        private readonly ConcurrentDictionary<string, ClientSession> _clients =
            new ConcurrentDictionary<string, ClientSession>();
        private string selectedPath = "";
        private bool isFolder = false;

        public Form1()
        {
            InitializeComponent();
            txtPort.Text = "5000";
            AppendLog("Ready.");
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(txtPort.Text.Trim());
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                _running = true;
                ToggleUi(true);
                AppendLog("Server started at 0.0.0.0:" + port);

                // Accept loop (không dùng CancellationToken cho .NET Framework)
                while (_running)
                {
                    TcpClient tcp = null;
                    try
                    {
                        tcp = await _listener.AcceptTcpClientAsync();
                    }
                    catch (ObjectDisposedException)
                    {
                        // listener.Stop() sẽ ném lỗi này khi dừng -> thoát vòng lặp
                        break;
                    }
                    catch (Exception ex)
                    {
                        AppendLog("Accept error: " + ex.Message);
                        continue;
                    }

                    // xử lý client song song
                    _ = HandleClientAsync(tcp);
                }
            }
            catch (Exception ex)
            {
                AppendLog("Start error: " + ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e) => StopServer();

        private void StopServer()
        {
            try
            {
                _running = false;
                try { _listener?.Stop(); } catch { }

                foreach (var kv in _clients) kv.Value.Dispose();
                _clients.Clear();
                RefreshClientList();

                ToggleUi(false);
                AppendLog("Server stopped.");
            }
            catch (Exception ex)
            {
                AppendLog("Stop error: " + ex.Message);
            }
        }

        private async Task HandleClientAsync(TcpClient tcp)
        {
            string id = Guid.NewGuid().ToString("N");
            using (var session = new ClientSession(id, tcp))
            {
                _clients[id] = session;

                try
                {
                    AppendLog("Incoming from " + session.RemoteEndPoint);
                    var stream = session.Stream;

                    // 1) Nhận "hello"
                    var raw = await Wire.ReadJsonRawAsync(stream);
                    if (raw == null) throw new IOException("Client closed before hello");
                    var hello = Wire.Deserialize<HelloMsg>(raw);
                    if (hello == null ||
                        !string.Equals(hello.Type, "hello", StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrWhiteSpace(hello.Name))
                        throw new InvalidOperationException("First packet must be {type:'hello', name:'...'}");

                    session.DisplayName = hello.Name.Trim();
                    AppendLog("[" + id.Substring(0, 6) + "] Hello '" + session.DisplayName + "' (" + session.RemoteEndPoint + ")");
                    RefreshClientList();


                    await Wire.WriteJsonAsync(stream, new ServerNotice("server_notice", "Welcome " + session.DisplayName + "!"));


                    while (_running)
                    {
                        var msg = await Wire.ReadJsonRawAsync(stream);
                        if (msg == null) break;
                        // có thể parse thêm nếu cần
                    }
                }
                catch (Exception ex)
                {
                    AppendLog("[" + id.Substring(0, 6) + "] error: " + ex.Message);
                }
                finally
                {
                    _clients.TryRemove(id, out _);
                    AppendLog("[" + id.Substring(0, 6) + "] disconnected.");
                    RefreshClientList();
                }
            }
        }

        private async void btnSendAll_Click(object sender, EventArgs e)
        {
            var input = !string.IsNullOrEmpty(selectedPath) ? selectedPath : txtAnnouncement.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                AppendLog("Empty message.");
                return;
            }

            // Kiểm tra file/folder path có hợp lệ không
            bool isFile = File.Exists(input);
            bool isFolder = Directory.Exists(input);

            int ok = 0, fail = 0;
            if (isFile)
            {
                string fileName = Path.GetFileName(input);
                byte[] data = File.ReadAllBytes(input);
                AppendLog($"Broadcast file: {fileName} ({data.Length / 1024.0:F2} KB)");

                foreach (var s in _clients.Values.ToArray())
                {
                    try
                    {
                        await Wire.WriteJsonAsync(s.Stream, new BroadcastMsg("file", "Teacher", fileName, data.Length));
                        await s.Stream.WriteAsync(data, 0, data.Length);
                        await s.Stream.FlushAsync();
                        ok++;
                    }
                    catch
                    {
                        fail++;
                    }
                }
                AppendLog($"File broadcast done. OK={ok}, Fail={fail}");
            }
            else if (isFolder)
            {
                string tempZip = Path.Combine(Path.GetTempPath(), Path.GetFileName(input) + ".zip");
                if (File.Exists(tempZip)) File.Delete(tempZip);

                ZipFile.CreateFromDirectory(input, tempZip);
                byte[] zipData = File.ReadAllBytes(tempZip);
                string zipName = Path.GetFileName(tempZip);
                AppendLog($"Broadcast folder: {zipName} ({zipData.Length / 1024.0:F2} KB)");

                foreach (var s in _clients.Values.ToArray())
                {
                    try
                    {
                        await Wire.WriteJsonAsync(s.Stream, new BroadcastMsg("folder", "Teacher", zipName, zipData.Length));
                        await s.Stream.WriteAsync(zipData, 0, zipData.Length);
                        await s.Stream.FlushAsync();
                        ok++;
                    }
                    catch
                    {
                        fail++;
                    }
                }
                AppendLog($"Folder broadcast done. OK={ok}, Fail={fail}");
                try { File.Delete(tempZip); } catch { }
            }
            // Nếu là text or link
            else
            {
                bool isLink = input.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
               || input.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
               || input.StartsWith("www.", StringComparison.OrdinalIgnoreCase);

                string msgType = isLink ? "link" : "text";
                AppendLog(isLink ? $"Broadcast link: {input}" : $"Broadcast text: {input}");

                foreach (var s in _clients.Values.ToArray())
                {
                    try
                    {
                        await Wire.WriteJsonAsync(s.Stream, new BroadcastMsg(msgType, "Teacher", input));
                        ok++;
                    }
                    catch
                    {
                        fail++;
                    }
                }

                AppendLog($"{(isLink ? "Link" : "Text")} broadcast done. OK={ok}, Fail={fail}");
            }
            selectedPath = null;
            isFolder = false;
            pnlAttachment.Visible = false;
            txtAnnouncement.Clear();
        }

        private async void btnSendSelected_Click(object sender, EventArgs e)
        {
            string input = !string.IsNullOrEmpty(selectedPath) ? selectedPath : txtAnnouncement.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                AppendLog("Empty message.");
                return;
            }

            var targets = listClients.SelectedItems.Cast<ClientListItem>().ToArray();
            if (targets.Length == 0)
            {
                AppendLog("Select client(s).");
                return;
            }

            bool isFile = File.Exists(input);
            bool isFolder = Directory.Exists(input);

            int ok = 0, fail = 0;

            if (isFile)
            {
                string fileName = Path.GetFileName(input);
                byte[] data = File.ReadAllBytes(input);
                AppendLog($"Send file: {fileName} ({data.Length / 1024.0:F2} KB)");

                foreach (var item in targets)
                {
                    if (!_clients.TryGetValue(item.Id, out ClientSession s)) continue;

                    try
                    {
                        await Wire.WriteJsonAsync(s.Stream, new BroadcastMsg("file", "Teacher", fileName, data.Length));
                        await s.Stream.WriteAsync(data, 0, data.Length);
                        await s.Stream.FlushAsync();
                        ok++;
                    }
                    catch { fail++; }
                }

                AppendLog($"File send done. OK={ok}, Fail={fail}");
            }
            else if (isFolder)
            {
                string tempZip = Path.Combine(Path.GetTempPath(), Path.GetFileName(input) + ".zip");
                if (File.Exists(tempZip)) File.Delete(tempZip);

                ZipFile.CreateFromDirectory(input, tempZip);
                byte[] zipData = File.ReadAllBytes(tempZip);
                string zipName = Path.GetFileName(tempZip);
                AppendLog($"Send folder: {zipName} ({zipData.Length / 1024.0:F2} KB)");

                foreach (var item in targets)
                {
                    if (!_clients.TryGetValue(item.Id, out ClientSession s)) continue;

                    try
                    {
                        await Wire.WriteJsonAsync(s.Stream, new BroadcastMsg("folder", "Teacher", zipName, zipData.Length));
                        await s.Stream.WriteAsync(zipData, 0, zipData.Length);
                        await s.Stream.FlushAsync();
                        ok++;
                    }
                    catch { fail++; }
                }

                AppendLog($"Folder send done. OK={ok}, Fail={fail}");
                try { File.Delete(tempZip); } catch { }
            }
            // Nếu là text or link
            else
            {
                bool isLink = input.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    || input.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                    || input.StartsWith("www.", StringComparison.OrdinalIgnoreCase);

                string msgType = isLink ? "link" : "text";

                AppendLog(isLink ? $"Send link: {input}" : $"Send text: {input}");

                foreach (var item in targets)
                {
                    if (!_clients.TryGetValue(item.Id, out ClientSession s)) continue;

                    try
                    {
                        await Wire.WriteJsonAsync(s.Stream, new BroadcastMsg(msgType, "Teacher", input));
                        ok++;
                    }
                    catch
                    {
                        fail++;
                    }
                }

                AppendLog($"{(isLink ? "Link" : "Text")} send done. OK={ok}, Fail={fail}");
            }

            selectedPath = null;
            isFolder = false;
            pnlAttachment.Visible = false;
            txtAnnouncement.Clear();
        }


        private void RefreshClientList()
        {
            if (InvokeRequired) { BeginInvoke(new Action(RefreshClientList)); return; }
            listClients.Items.Clear();
            foreach (var c in _clients.Values.OrderBy(x => x.DisplayName ?? x.Id))
            {
                listClients.Items.Add(new ClientListItem(
                    c.Id,
                    c.Id.Substring(0, 6) + " | " + (c.DisplayName ?? "(unknown)") + " | " + c.RemoteEndPoint));
            }
            lblClientCount.Text = "Clients: " + _clients.Count;
        }

        private void ToggleUi(bool running)
        {
            if (InvokeRequired) { BeginInvoke(new Action<bool>(ToggleUi), running); return; }
            btnStart.Enabled = !running;
            txtPort.Enabled = !running;
            btnStop.Enabled = running;
        }

        private void AppendLog(string line)
        {
            if (InvokeRequired) { BeginInvoke(new Action<string>(AppendLog), line); return; }
            txtLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + line + Environment.NewLine);
        }



        private void btnUpload_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("📄 Gửi File / Ảnh", null, (s, ev) => SelectFile());
            menu.Items.Add("📁 Gửi Folder", null, (s, ev) => SelectFolder());
            menu.Show(btnUpload, new Point(0, btnUpload.Height));
        }

        private void SelectFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedPath = ofd.FileName;
                    isFolder = false;
                    AppendLog($"[UPLOAD] Đã chọn file: {Path.GetFileName(selectedPath)} ({new FileInfo(selectedPath).Length / 1024.0:F2} KB)");
                    lblAttachmentName.Text = $"📄 {Path.GetFileName(selectedPath)}";
                    pnlAttachment.Visible = true;
                }
            }
        }

        private void SelectFolder()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    selectedPath = fbd.SelectedPath;
                    isFolder = true;
                    AppendLog($"[UPLOAD] Đã chọn thư mục: {Path.GetFileName(selectedPath)}");
                    lblAttachmentName.Text = $"📁 {Path.GetFileName(selectedPath)}";
                    pnlAttachment.Visible = true;


                }
            }
        }
        private void btnRemoveAttachment_Click(object sender, EventArgs e)
        {
            selectedPath = null;
            isFolder = false;
            pnlAttachment.Visible = false;
            AppendLog("[UPLOAD] Đã hủy chọn tệp / thư mục.");
        }

    }




    // ===== Helpers =====
    public sealed class ClientSession : IDisposable
    {
        public string Id { get; private set; }
        public string DisplayName { get; set; }
        public TcpClient Tcp { get; private set; }
        public NetworkStream Stream { get { return Tcp.GetStream(); } }
        public string RemoteEndPoint { get { return Tcp.Client.RemoteEndPoint != null ? Tcp.Client.RemoteEndPoint.ToString() : "unknown"; } }

        public ClientSession(string id, TcpClient tcp)
        {
            Id = id;
            Tcp = tcp;
            Tcp.NoDelay = true;
        }

        public void Dispose()
        {
            try { Tcp.Close(); } catch { }
            try { Tcp.Dispose(); } catch { }
        }
    }

    public class ClientListItem
    {
        public string Id { get; private set; }
        public string DisplayText { get; private set; }
        public ClientListItem(string id, string display) { Id = id; DisplayText = display; }
        public override string ToString() { return DisplayText; }
    }

    public class FileInfoMsg : BaseMsg
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }

        public FileInfoMsg(string id, string name, long size)
        {
            Type = "file_info";
            Id = id;
            Name = name;
            Size = size;
        }
    }

    public class FileChunkMsg : BaseMsg
    {
        public string Id { get; set; }
        public string Data { get; set; } // base64 encoded

        public FileChunkMsg(string id, string data)
        {
            Type = "file_chunk";
            Id = id;
            Data = data;
        }
    }

    public class FileEndMsg : BaseMsg
    {
        public string Id { get; set; }

        public FileEndMsg(string id)
        {
            Type = "file_end";
            Id = id;
        }
    }


}
