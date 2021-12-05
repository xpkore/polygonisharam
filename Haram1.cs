using DEFI.haram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DEFI.haram.Haram2;

namespace haram
{
    public class Haram1
    {
        public void Run(string url)
        {
            Haram2 client = new Haram2(url);
            client.Start();
            client.SubscribeBlockHeaders(BlockHeadersHandler);
            //client.SubscribePendingTransactions(PendingTransactionHandler);
        }

        private void BlockHeadersHandler(BlockHeaderResult obj)
        {
            long s = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine(s + " : " + obj.number);
        }

        private void PendingTransactionHandler(string obj)
        {
            long s = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine(s + " : " +obj);
        }
    }
}
