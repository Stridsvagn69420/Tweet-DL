using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Tweet_DL
{
    public class Downloader
    {
        private readonly HttpClient _Client;
        private readonly string _DownloadPath;
        private readonly int _Delay;
        public Downloader(HttpClient client, string downPath, int delay)
        {
            _Client = client;
            _DownloadPath = downPath;
            _Delay = delay;
        }
        public void DownloadURLs(List<string> URLs)
        {
            Console.Write("Downloading images... ");
            Printer.PrintLine(ConsoleColor.Blue, $"[{URLs.Count}]");
            Console.Write("ETA: ");
            double ETA = _Delay * URLs.Count / 60000;
            Printer.PrintLine(ConsoleColor.Red, $"{Math.Round(ETA)} min");
            for (int i = 0; i < URLs.Count; i++)
            {
                string filename = Path.GetFileName(URLs[i]);
                string absolutePath = Path.Combine(_DownloadPath, filename);
                Printer.Print(ConsoleColor.Blue, $"[{i + 1}] ");
                if (File.Exists(absolutePath))
                {
                    Printer.Print(ConsoleColor.Cyan, filename);
                    Console.WriteLine(" already exists!");
                }
                else
                {
                    HttpRequestMessage httpRequest = new(HttpMethod.Get, URLs[i]);
                    Console.Write("Downloading ");
                    Printer.Print(ConsoleColor.Red, filename);
                    byte[] image;
                    try
                    {
                        image = _Client.SendAsync(httpRequest).Result.Content.ReadAsByteArrayAsync().Result;
                        Printer.Print(ConsoleColor.Yellow, " ↓ ");
                        File.WriteAllBytes(absolutePath, image);
                        Printer.PrintLine(ConsoleColor.Green, "Done!");
                    }
                    catch (HttpRequestException e)
                    {
                        Printer.PrintLine(ConsoleColor.Red, $"\nAn error occured trying to download {filename}:");
                        Console.WriteLine(e.Message);
                    }
                    catch (IOException e)
                    {
                        Printer.PrintLine(ConsoleColor.Red, $"\nAn error occured trying to save {filename}:");
                        Console.WriteLine(e.Message);
                    }
                    Thread.Sleep(_Delay);
                }
            }
        }
    }
}
