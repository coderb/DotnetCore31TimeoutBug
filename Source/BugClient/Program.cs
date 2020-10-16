using System;
using System.IO;
using System.Net;
using System.Text;

namespace ConsoleApp1 {
    class Program {

        private static readonly WebProxy WebProxy = new WebProxy(new Uri("http://127.0.0.1:13121/"));
        private const int TimeoutMillis = 5000;

        static void Main(string[] args) {
            new Program().Run();
        }

        private void Run() {
            // ignore server cert errors
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            MemoryStream ms;
            using (var stream = TransmitRequestReturnStreamOfTypeEncoding()) {
                ms = ReadToMemoryStream(stream);
            }

            var s = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            System.Console.WriteLine("got response: " + s);
        }

        public static MemoryStream ReadToMemoryStream(Stream stream) {
            var buffer = new byte[4096];
            var result = new MemoryStream();
            while (true) {
                int n = stream.Read(buffer);
                if (n == 0) {
                    break;
                }
                result.Write(buffer, 0, n);
            }
            result.Position = 0;
            return result;
        }

        public Stream TransmitRequestReturnStreamOfTypeEncoding() {
            var url = "https://www.google.com/";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = TimeoutMillis;
            request.UserAgent = "dummy-user-agent";
            request.Proxy = WebProxy;
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }
    }
}