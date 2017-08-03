using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace InternetStatusConsoleApp
{
    internal class Program
    {
        private const int InternetConnectionModem = 1;
        private const int InternetConnectionLan = 2;

        [DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        private static void Main(string[] args)
        {
            Console.ForegroundColor=ConsoleColor.Cyan;
            
            Task.Factory.StartNew(() =>
            {
                var url = "www.baidu.com";
                var count = 0;
                var failCount =0;
                while (true)
                {
                    Thread.Sleep(1000);
                    var localConnectionStatus = GetLocalConnectionStatus();
                    Console.WriteLine($"[本地连接状态] {localConnectionStatus}");
                    var result = GetPingResult(url);
                    ++count;
                    if (!result)
                        ++failCount;
                    Console.WriteLine($"[ping {url}] {result}");
                    Console.WriteLine($"[Count]:{count} [FailCount]:{failCount}");
                    Console.WriteLine($"---------------------------------------");
                }
            });
            
            Console.ReadLine();
        }

        public enum LocalConnectionStatus
        {
            采用调制解调器上网,
            采用网卡上网,
            未知,
            未连网
        }

        /// <summary>
        /// 判断本地的连接状态
        /// </summary>
        /// <returns></returns>
        private static LocalConnectionStatus GetLocalConnectionStatus()
        {
            var dwFlag = 0;
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                return LocalConnectionStatus.未连网;
            }

            if ((dwFlag & InternetConnectionModem) != 0)
            {
                return LocalConnectionStatus.采用调制解调器上网;
            }
            if ((dwFlag & InternetConnectionLan) != 0)
            {
                return LocalConnectionStatus.采用网卡上网;
            }
            return LocalConnectionStatus.未知;
        }

        /// <summary>
        /// ping IP
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool GetPingResult(string url)
        {
            var ping = new Ping();
            try
            {
                var pingReply = ping.Send(url);
                return pingReply?.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

     
    }
}