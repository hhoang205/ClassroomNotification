using SharedNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentClientWinForms
{
    public partial class Form1 : Form
    {
        private TcpClient _tcp;

        // Lưu map ID -> file path thật
        private readonly Dictionary<string, string> _fileLinkMap = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
            txtServer.Text = "127.0.0.1";
            txtPort.Text = "5000";
            txtName.Text = "Student-" + Environment.UserName;

            txtReceived.DetectUrls = true;
            txtReceived.LinkClicked += txtReceived_LinkClicked;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_tcp != null)
                {
                    Append("Already connected.");
                    return;
                }

                string host = txtServer.Text.Trim();
                int port = int.Parse(txtPort.Text.Trim());
                string name = txtName.Text.Trim();

                _tcp = new TcpClient();
                await _tcp.ConnectAsync(host, port);
                _tcp.NoDelay = true;

                Append($"Connected to {host}:{port}");
                ToggleUi(true);

                await Wire.WriteJsonAsync(_tcp.GetStream(), new HelloMsg("hello", name));
                _ = ReadLoopAsync();
            }
            catch (Exception ex)
            {
                Append("Connect error: " + ex.Message);
                DisconnectInternal();
            }
        }

        private async Task ReadLoopAsync()
        {
            try
            {
                var ns = _tcp.GetStream();

                while (true)
                {
                    var rawJson = await Wire.ReadJsonRawAsync(ns);
                    if (rawJson == null) break;

                    var baseMsg = Wire.Deserialize<BaseMsg>(rawJson);
                    if (baseMsg == null)
                    {
                        Append("Nhận dữ liệu lỗi, không parse được.");
                        continue;
                    }

                    switch (baseMsg.Type?.ToLower())
                    {
                        case "text":
                            {
                                var msg = Wire.Deserialize<BroadcastMsg>(rawJson);
                                Append($"[{msg.From}] {msg.Text}");
                                continue;
                            }
                        case "link":
                            {
                                var msg = Wire.Deserialize<BroadcastMsg>(rawJson);
                                if (msg == null) break;

                                string originalText = msg.Text;
                                string displayText = $"[{msg.From}] ";

                                // Pattern phát hiện URL
                                string pattern = @"(https?://[^\s]+)";
                                var matches = System.Text.RegularExpressions.Regex.Matches(originalText, pattern);

                                if (matches.Count > 0)
                                {
                                    int lastIndex = 0;
                                    foreach (System.Text.RegularExpressions.Match match in matches)
                                    {
                                        string url = match.Value;

                                        // Thêm phần text trước link
                                        if (match.Index > lastIndex)
                                        {
                                            string before = originalText.Substring(lastIndex, match.Index - lastIndex);
                                            displayText += before;
                                        }

                                        // Thêm link clickable (dùng ký hiệu đặc biệt để UI nhận diện)
                                        displayText += $"[LINK:{url}]";

                                        lastIndex = match.Index + match.Length;
                                    }

                                    // Thêm phần còn lại sau link cuối
                                    if (lastIndex < originalText.Length)
                                    {
                                        displayText += originalText.Substring(lastIndex);
                                    }
                                }
                                else
                                {
                                    displayText += originalText;
                                }

                                Append(displayText);
                                break;
                            }

                        case "file":
                            {
                                var msg = Wire.Deserialize<BroadcastMsg>(rawJson);
                                if (msg == null || msg.FileSize <= 0)
                                {
                                    Append("File message rỗng hoặc lỗi.");
                                    continue;
                                }

                                Append($"[{msg.From}] gửi file: {msg.FileName} ({msg.FileSize / 1024.0:F2} KB)");

                                byte[] fileData = new byte[msg.FileSize];
                                int received = await Wire.ReadExactlyAsync(ns, fileData, 0, fileData.Length);
                                if (received != msg.FileSize)
                                {
                                    Append($"Lỗi nhận thiếu file {msg.FileName}");
                                    continue;
                                }

                                string downloadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
                                Directory.CreateDirectory(downloadDir);
                                string filePath = Path.Combine(downloadDir, msg.FileName);

                                // Dùng bản sync cho .NET Framework
                                File.WriteAllBytes(filePath, fileData);

                                Append("[DEBUG] Lưu file tại: " + filePath);

                                string id = Guid.NewGuid().ToString("N");
                                _fileLinkMap[id] = filePath;

                                Append($"✅ Đã tải file: {msg.FileName} -> openfile://{id}");
                                AddDownloadItemToList(id, msg.FileName);
                                continue;
                            }
                        case "folder":
                            {
                                var msg = Wire.Deserialize<BroadcastMsg>(rawJson);
                                if (msg == null || msg.FileSize <= 0)
                                {
                                    Append("Folder message rỗng hoặc lỗi.");
                                    continue;
                                }

                                Append($"[{msg.From}] gửi thư mục (nén): {msg.FileName} ({msg.FileSize / 1024.0:F2} KB)");

                                byte[] zipData = new byte[msg.FileSize];
                                int received = await Wire.ReadExactlyAsync(ns, zipData, 0, zipData.Length);
                                if (received != msg.FileSize)
                                {
                                    Append($"Lỗi nhận thiếu thư mục nén {msg.FileName}");
                                    continue;
                                }

                                string downloadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
                                Directory.CreateDirectory(downloadDir);
                                string zipPath = Path.Combine(downloadDir, msg.FileName);

                                // Lưu file nén .zip
                                File.WriteAllBytes(zipPath, zipData);

                                Append("[DEBUG] Lưu thư mục nén tại: " + zipPath);

                                // Tạo ID map để click mở file
                                string id = Guid.NewGuid().ToString("N");
                                _fileLinkMap[id] = zipPath;

                                // Append dạng link giống file
                                Append($"✅ Đã tải thư mục nén: {msg.FileName} -> openfile://{id}");

                                // Add vào danh sách tải xuống
                                AddDownloadItemToList(id, msg.FileName);

                                continue;
                            }

                        case "server_notice":
                            {
                                var notice = Wire.Deserialize<ServerNotice>(rawJson);
                                if (notice != null) Append($"[Server] {notice.Text}");
                                continue;
                            }

                        default:
                            Append("Unknown message type: " + baseMsg.Type);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Append("Read error: " + ex.Message);
            }
            finally
            {
                Append("Disconnected.");
                DisconnectInternal();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Append("Disconnecting...");
            DisconnectInternal();
        }

        private void DisconnectInternal()
        {
            try { _tcp?.Close(); _tcp?.Dispose(); } catch { }
            _tcp = null;
            ToggleUi(false);
        }

        private void ToggleUi(bool connected)
        {
            if (InvokeRequired) { BeginInvoke(new Action<bool>(ToggleUi), connected); return; }

            btnConnect.Enabled = !connected;
            txtServer.Enabled = !connected;
            txtPort.Enabled = !connected;
            txtName.Enabled = !connected;
            btnDisconnect.Enabled = connected;
        }

        private void Append(string line)
        {
            if (InvokeRequired) { BeginInvoke(new Action<string>(Append), line); return; }

            string processed = System.Text.RegularExpressions.Regex.Replace(
               line,
               @"\[LINK:(https?://[^\]]+)\]",
               "$1" // chỉ giữ lại URL thật
           );

            txtReceived.SelectionStart = txtReceived.TextLength;
            txtReceived.SelectionLength = 0;
            txtReceived.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}\n");
            txtReceived.AppendText(processed + Environment.NewLine);
            txtReceived.ScrollToCaret();
        }


        // Map file ID -> mở file thật
        private void txtReceived_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string link = e.LinkText;

            // 1. Trường hợp mở file local dạng openfile://xxxxx
            if (link.StartsWith("openfile://"))
            {
                string id = link.Substring("openfile://".Length);

                if (_fileLinkMap.TryGetValue(id, out var filePath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể mở file: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file tương ứng.");
                }

                return;
            }

            // 2. Mặc định: mở URL bình thường
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở liên kết: " + ex.Message);
            }
        }




        // ------------ DOWNLOAD LIST ----------------

        private void AddDownloadItemToList(string id, string name)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, string>(AddDownloadItemToList), id, name);
                return;
            }

            lstDownloads.Items.Add(new DownloadItem { Id = id, Name = name });
        }

        private class DownloadItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private void btnOpenSelected_Click(object sender, EventArgs e)
        {
            var item = lstDownloads.SelectedItem as DownloadItem;
            if (item == null) { MessageBox.Show("Hãy chọn 1 file để mở."); return; }

            if (_fileLinkMap.TryGetValue(item.Id, out var path))
                OpenLocalFile(path);
            else
                MessageBox.Show("Không tìm thấy file trong hệ thống.");
        }

        private void lstDownloads_DoubleClick(object sender, EventArgs e)
        {
            btnOpenSelected_Click(sender, e);
        }

        private void OpenLocalFile(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("File đã bị xóa hoặc di chuyển:\n" + path);
                return;
            }

            try
            {
                MessageBox.Show("Đang mở file:\n" + path, "Opening");
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được file:\n" + ex.Message);
            }
        }
    }
}
