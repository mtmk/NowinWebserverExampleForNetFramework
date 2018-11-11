using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nowin;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        {
            ServerBuilder
                .New()
                .SetPort(8888)
                .SetOwinApp(App)
                .Build()
                .Start();
            
            Console.WriteLine($"Started on http://localhost:8888");
            Console.ReadLine();
        }

        static async Task App(IDictionary<string, object> o)
        {
            var path = (string) o["owin.RequestPath"];
            var body = (Stream) o["owin.ResponseBody"];
            using (var r = new StreamWriter(body))
            {
                string html;
                try
                {
                    html = Html(path);

                    if (html == null)
                    {
                        html = "Not found";
                        o["owin.ResponseStatusCode"] = 404;
                    }
                }
                catch (Exception e)
                {
                    html = $"Error: {e.Message}";
                    o["owin.ResponseStatusCode"] = 500;
                }
                
                await r.WriteLineAsync(html);
            }
        }

        static string Html(string path)
        {
            if (path == "/")
            {
                 return "<pre>\n"
                        + "<a href=\"/i/asd\">asd</a>\n"
                        + "<a href=\"/i/qwe\">qwe</a>\n"
                        + "<a href=\"/no\">not found</a>\n"
                        + "<a href=\"/err\">error</a>\n"
                    ;
            }

            if (path.StartsWith("/i/"))
            {
                return $"<pre>name: {path}";
            }
            
            if (path == "/err")
            {
                throw new Exception("oops!");
            }

            return null;
        }
    }
}
