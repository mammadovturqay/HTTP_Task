
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

Console.WriteLine("Server Side");

var listener = new HttpListener();
listener.Prefixes.Add("http://127.0.0.1:27001/");
listener.Prefixes.Add("http://localhost:27004/");

listener.Start();
while (true)
{
    var context = await listener.GetContextAsync();
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;

    var url = request.Url.AbsolutePath.ToLower();

    using (var writer = new StreamWriter(response.OutputStream))
    {
        string content;

        if (url == "/" || url == "/index")
        {
            content = File.ReadAllText("Pages/index.html");
        }
        else
        {
            // Eğer URL "/Pages/Gallery/cars" gibi bir formatta gelirse
            if (url.StartsWith("/pages/gallery/") || url.StartsWith("/pages/step/"))
            {
                var parts = url.Split('/');
                var folder = parts[2];
                var filename = parts[3];
                filename = filename.EndsWith(".html") ? filename : filename + ".html";

                if (File.Exists($"Pages/{folder}/{filename}"))
                {
                    content = File.ReadAllText($"Pages/{folder}/{filename}");
                }
                else
                {
                    content = File.ReadAllText("Pages/404.html");
                    response.StatusCode = 404;
                }
            }
            else
            {
                // Diğer durumlarda, URL'ye göre dosya okunur
                var filename = url.EndsWith(".html") ? url : url + ".html";

                if (File.Exists($"Pages/{filename}"))
                {
                    content = File.ReadAllText($"Pages/{filename}");
                }
                else
                {
                    content = File.ReadAllText("Pages/404.html");
                    response.StatusCode = 404;
                }
            }
        }

        writer.Write(content);
    }
}


