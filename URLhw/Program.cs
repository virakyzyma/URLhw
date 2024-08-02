using System.Net;

namespace URLhw
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> urls = new List<string>()
            {
                "https://www.microsoft.com/uk-ua",
                "https://github.com/virakyzyma?tab=repositories"
            };
            CancellationTokenSource cts = new CancellationTokenSource();
            Task[] tasks = new Task[urls.Count];
            for (int i = 0; i < urls.Count; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    while (urls.Count > 0 && !cts.Token.IsCancellationRequested)
                    {
                        string url = null;
                        lock (urls)
                        {
                            if (urls.Count > 0)
                            {
                                url = urls[0];
                                urls.RemoveAt(0);
                            }
                        }

                        if (url != null)
                        {
                            try
                            {
                                Console.WriteLine($"Downloading - {url}");
                                WebClient client = new WebClient();
                                client.DownloadFile(url, $"{DomainCutter.CleanUrl(url)}.txt");
                                Console.WriteLine($"Success - {url}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error - {ex.Message}");
                            }
                        }
                    }
                }, cts.Token);
            }
            Console.WriteLine("Press any key");
            Console.ReadKey();
            cts.Cancel();
            Task.WaitAll(tasks);
        }
    }
    class DomainCutter
    {
        public static string CleanUrl(string url)
        {
            string _url = "", cleanUrl = "";
            if (url.Contains("https://"))
            {
                cleanUrl = url.Substring(8);
            }
            else if (url.Contains("http://"))
            {
                cleanUrl = url.Substring(7);
            }

            if (cleanUrl.Contains("/"))
            {
                cleanUrl = cleanUrl.Trim();
                cleanUrl = cleanUrl.Substring(0, cleanUrl.IndexOf('/'));
            }
            return cleanUrl;
        }
    }
}
