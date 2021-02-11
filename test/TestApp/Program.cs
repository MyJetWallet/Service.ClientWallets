using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.ClientWallets.Client;
using Service.ClientWallets.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new ClientWalletsClientFactory("http://localhost:5001");
            

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
