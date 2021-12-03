using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
namespace haram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            (new Haram1()).Run(args[0]);
            while (true) { System.Threading.Thread.Sleep(1000); }
        }
    }
}
