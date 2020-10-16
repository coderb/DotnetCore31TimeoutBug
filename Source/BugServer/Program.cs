using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2 {
    class Program {
        static async Task Main(string[] args) {
            await new Program().Run();
        }

        private async Task Run() {
            var certificate = new X509Certificate2(@"test.pfx", "test");

            var listener = new TcpListener(IPAddress.Loopback, 13121);
            listener.Start();
            while (true) {
                try {
                    //await OkResponseHandleClientAsync(listener, certificate);
                    //await OkClientTimeoutHandleClientAsync(listener, certificate);
                    await BuggedClientTimeoutHandleClientAsync(listener, certificate);
                } catch (Exception ex) {
                    Log("exception in handler " + ex);
                }
                await Task.Delay(100);
            }
        }

        private async Task OkResponseHandleClientAsync(TcpListener listener, X509Certificate2 certificate) {
            Log("waiting for conenction");
            using (var client = await listener.AcceptTcpClientAsync()) {
                Log("connected");
                await using (var clientStream = client.GetStream()) {
                    var line = await ReadLineAsync(clientStream);
                    Debug.Assert(line.StartsWith("CONNECT"));
                    line = await ReadLineAsync(clientStream);
                    line = await ReadLineAsync(clientStream);
                    line = await ReadLineAsync(clientStream);
                    Debug.Assert(line.Length == 0);
                    Log("proxy request headers ok");

                    await WriteAsciiAsync(clientStream, "HTTP/1.1 200 OK\r\n\r\n");

                    Log("sslstream ctor");
                    var sslStream = new SslStream(clientStream, leaveInnerStreamOpen:true);

                    Log("ssl auth start");
                    await sslStream.AuthenticateAsServerAsync(certificate);
                    Log("ssl auth done");

                    Log("(ssl) read headers start");
                    while (true) {
                        line = await ReadLineAsync(sslStream);
                        if (line.Trim().Length == 0) {
                            break;
                        }
                    }
                    Log("(ssl) read headers done");

                    var html = "<html><body>here comes the pain</body></html>";

                    await WriteAsciiAsync(sslStream, "HTTP/1.1 200 OK\r\n");
                    await WriteAsciiAsync(sslStream, "Content-Type: text/html; charset=utf-8\r\n");
                    // client timeouts seem to work when content-length header is returned
                    // await WriteLineAsync(sslStream, "Content-Length: 0\r\n");
                    await WriteAsciiAsync(sslStream, "Content-Length: " + html.Length + "\r\n");
                    await WriteAsciiAsync(sslStream, "\r\n");
                    await WriteAsciiAsync(sslStream, html);

                    var millisecondsDelay = 5000;
                    Log("sleep " + millisecondsDelay);
                    await Task.Delay(millisecondsDelay);
                    Log("sleep done");

                    millisecondsDelay = 200000;
                    Log("sleep " + millisecondsDelay);
                    await Task.Delay(millisecondsDelay);
                    Log("sleep done");


                }
                Log("client done");
            }
        }

        private async Task OkClientTimeoutHandleClientAsync(TcpListener listener, X509Certificate2 certificate) {
            Log("waiting for conenction");
            using (var client = await listener.AcceptTcpClientAsync()) {
                Log("connected");
                await using (var clientStream = client.GetStream()) {
                    var line = await ReadLineAsync(clientStream);
                    Debug.Assert(line.StartsWith("CONNECT"));
                    line = await ReadLineAsync(clientStream);
                    line = await ReadLineAsync(clientStream);
                    line = await ReadLineAsync(clientStream);
                    Debug.Assert(line.Length == 0);
                    Log("proxy request headers ok");

                    await WriteAsciiAsync(clientStream, "HTTP/1.1 200 OK\r\n\r\n");

                    Log("sslstream ctor");
                    var sslStream = new SslStream(clientStream, leaveInnerStreamOpen:true);

                    Log("ssl auth start");
                    await sslStream.AuthenticateAsServerAsync(certificate);
                    Log("ssl auth done");

                    Log("(ssl) read headers start");
                    while (true) {
                        line = await ReadLineAsync(sslStream);
                        if (line.Trim().Length == 0) {
                            break;
                        }
                    }
                    Log("(ssl) read headers done");

                    await WriteAsciiAsync(sslStream, "HTTP/1.1 200 OK\r\n");

                    var millisecondsDelay = 5000;
                    Log("sleep " + millisecondsDelay);
                    await Task.Delay(millisecondsDelay);
                    Log("sleep done");

                    millisecondsDelay = 200000;
                    Log("sleep " + millisecondsDelay);
                    await Task.Delay(millisecondsDelay);
                    Log("sleep done");


                }
                Log("client done");
            }
        }

        private async Task BuggedClientTimeoutHandleClientAsync(TcpListener listener, X509Certificate2 certificate) {
            Log("waiting for conenction");
            using (var client = await listener.AcceptTcpClientAsync()) {
                Log("connected");
                await using (var clientStream = client.GetStream()) {
                    var line = await ReadLineAsync(clientStream);
                    Debug.Assert(line.StartsWith("CONNECT"));
                    line = await ReadLineAsync(clientStream);
                    line = await ReadLineAsync(clientStream);
                    line = await ReadLineAsync(clientStream);
                    Debug.Assert(line.Length == 0);
                    Log("proxy request headers ok");

                    await WriteAsciiAsync(clientStream, "HTTP/1.1 200 OK\r\n\r\n");

                    Log("sslstream ctor");
                    var sslStream = new SslStream(clientStream, leaveInnerStreamOpen:true);

                    Log("ssl auth start");
                    await sslStream.AuthenticateAsServerAsync(certificate);
                    Log("ssl auth done");

                    Log("(ssl) read headers start");
                    while (true) {
                        line = await ReadLineAsync(sslStream);
                        if (line.Trim().Length == 0) {
                            break;
                        }
                    }
                    Log("(ssl) read headers done");


                    await WriteAsciiAsync(sslStream, "HTTP/1.1 200 OK\r\n");
                    await WriteAsciiAsync(sslStream, "Content-Type: text/html; charset=utf-8\r\n");
                    // client timeouts seem to work when these headers are returned
                    // await WriteLineAsync(sslStream, "Content-Length: 0\r\n");
                    // await WriteLineAsync(sslStream, "Content-Length: 1000\r\n");
                    await WriteAsciiAsync(sslStream, "\r\n");

                    var millisecondsDelay = 5000;
                    Log("sleep " + millisecondsDelay);
                    await Task.Delay(millisecondsDelay);
                    Log("sleep done");

                    millisecondsDelay = 200000;
                    Log("sleep " + millisecondsDelay);
                    await Task.Delay(millisecondsDelay);
                    Log("sleep done");


                }
                Log("client done");
            }
        }

        private async Task WriteAsciiAsync(Stream stream, string line) {
            Log("server: " + line);
            await stream.WriteAsync(Encoding.ASCII.GetBytes(line));
            await stream.FlushAsync();
        }

        private async Task<string> ReadLineAsync(Stream stream) {
            var buffer = new byte[1];
            var sb = new StringBuilder();
            while (true) {
                var n = await stream.ReadAsync(buffer);
                if (n == 0) break;
                if (n != 1) throw new InvalidOperationException();
                var b = buffer[0];
                if (b == (byte)'\n') break;
                sb.Append((char)b);
            }

            var s = sb.ToString();
            s = s.EndsWith('\r') ? s.Substring(0, s.Length - 1) : s;
            Log("client: " + s);
            return s;
        }

        private static void Log(string s) {
            System.Console.WriteLine(s);
        }
    }
}
