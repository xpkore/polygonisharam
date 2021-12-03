using DEFI.haram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace haram
{
    public class Haram1
    {
        public void Run(string url)
        {
            Haram2 client = new Haram2(url);
            client.Start();
            client.SubscribePendingTransactions(PendingTransactionHandler);
        }

        private void PendingTransactionHandler(string obj)
        {
            long s = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine(s + " : " +obj);
        }
    }
}
