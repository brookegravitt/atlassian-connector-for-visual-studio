using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TestBambooLoginOnStac {
    class Program {

        private const string LOGIN_ACTION = "https://studio.atlassian.com/builds/api/rest/login.action";

        static void Main(string[] args) {
            if (args.Count() < 2) {
                printUsage();
            }

            const string url = LOGIN_ACTION + "?os_authType=basic";

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 10000;
            req.ReadWriteTimeout = 20000;
            req.ContentType = "application/xml";
            req.Method = "GET";
            req.Accept = "application/xml";

            req.Credentials = new NetworkCredential(args[0], args[1]);

            var resp = (HttpWebResponse)req.GetResponse();
            using (Stream stream = resp.GetResponseStream()) {
                if (!resp.StatusCode.Equals(HttpStatusCode.OK)) {
                    Console.WriteLine(resp.StatusDescription);
                    Console.WriteLine();
                    Console.WriteLine("*** press key to exit ***");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                int count;

                do {
                    // fill the buffer with data
                    count = stream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count == 0) continue;
                    // translate from bytes to ASCII text
                    string tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
                while (count > 0); // any more data to read?

                Console.Write(sb.ToString());
                Console.WriteLine();
                Console.WriteLine("**** Press any key to exit ****");
                Console.ReadKey();
            }
        }

        private static void printUsage() {
            Console.WriteLine("usage: prog <user name> <password>");
            Environment.Exit(0);
        }
    }
}
