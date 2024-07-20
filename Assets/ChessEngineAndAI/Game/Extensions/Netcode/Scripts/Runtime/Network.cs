using System.Net;

namespace ChessEngine.Networking
{
    /// <summary>
    /// A static class that holds information and global functions about the network.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public static class Network
    {
        /// <summary>
        /// Attempts to get and return the external IP address for the client.
        /// </summary>
        public static string GetExternalIPAddress()
        {
            string result = string.Empty;
            string[] checkIPUrl =
            {
                    "https://ipinfo.io/ip",
                    "https://checkip.amazonaws.com/",
                    "https://api.ipify.org/"
                };

            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " + "(compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                foreach (string url in checkIPUrl)
                {
                    try
                    {
                        result = client.DownloadString(url);
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(result))
                        break;
                }
            }

            return result.Replace("\n", "").Trim();
        }
    }
}
