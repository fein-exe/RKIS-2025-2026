using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TodoApp.Server
{
    class Program
    {
        private const string Prefix = "http://localhost:5000/";
        private static readonly string DataDir = "server_data";

        static async Task Main(string[] args)
        {
            if (!Directory.Exists(DataDir))
            {
                Directory.CreateDirectory(DataDir);
            }

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(Prefix);
                listener.Start();
                Console.WriteLine($"Сервер запущен на {Prefix}");
                Console.WriteLine("Нажмите Ctrl+C для остановки\n");

                while (true)
                {
                    try
                    {
                        var context = await listener.GetContextAsync();
                        _ = Task.Run(() => ProcessRequestAsync(context));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                }
            }
        }

        private static async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {request.HttpMethod} {request.Url.PathAndQuery}");

            try
            {
                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/profiles")
                {
                    await HandleSaveProfiles(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/profiles")
                {
                    await HandleLoadProfiles(response);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath.StartsWith("/todos/"))
                {
                    await HandleSaveTodos(request, response);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/todos/"))
                {
                    await HandleLoadTodos(request, response);
                }
                else
                {
                    response.StatusCode = 404;
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        await writer.WriteAsync("Not Found");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки: {ex.Message}");
                response.StatusCode = 500;
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync($"Server Error: {ex.Message}");
                }
            }
            finally
            {
                response.Close();
            }
        }

        private static async Task HandleSaveProfiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = Path.Combine(DataDir, "profiles.dat");
            
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var requestStream = request.InputStream)
            {
                await requestStream.CopyToAsync(fileStream);
            }
            
            response.StatusCode = 200;
            using (var writer = new StreamWriter(response.OutputStream))
            {
                await writer.WriteAsync("OK");
            }
            Console.WriteLine("  -> Профили сохранены");
        }

        private static async Task HandleLoadProfiles(HttpListenerResponse response)
        {
            string filePath = Path.Combine(DataDir, "profiles.dat");
            
            if (File.Exists(filePath))
            {
                byte[] data = await File.ReadAllBytesAsync(filePath);
                response.OutputStream.Write(data, 0, data.Length);
            }
            
            response.StatusCode = 200;
            Console.WriteLine("  -> Профили отправлены");
        }

        private static async Task HandleSaveTodos(HttpListenerRequest request, HttpListenerResponse response)
        {
            string path = request.Url.AbsolutePath;
            string userId = path.Replace("/todos/", "");
            string filePath = Path.Combine(DataDir, $"todos_{userId}.dat");
            
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var requestStream = request.InputStream)
            {
                await requestStream.CopyToAsync(fileStream);
            }
            
            response.StatusCode = 200;
            using (var writer = new StreamWriter(response.OutputStream))
            {
                await writer.WriteAsync("OK");
            }
            Console.WriteLine($"  -> Задачи пользователя {userId} сохранены");
        }

        private static async Task HandleLoadTodos(HttpListenerRequest request, HttpListenerResponse response)
        {
            string path = request.Url.AbsolutePath;
            string userId = path.Replace("/todos/", "");
            string filePath = Path.Combine(DataDir, $"todos_{userId}.dat");
            
            if (File.Exists(filePath))
            {
                byte[] data = await File.ReadAllBytesAsync(filePath);
                response.OutputStream.Write(data, 0, data.Length);
            }
            
            response.StatusCode = 200;
            Console.WriteLine($"  -> Задачи пользователя {userId} отправлены");
        }
    }
}