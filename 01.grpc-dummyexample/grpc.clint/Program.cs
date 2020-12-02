using grpc.server.Service;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace grpc.clint
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Basic Grpc connection\n\n");

            //Define Where...
            var uri = "https://localhost:9501";
            var channel = GrpcChannel.ForAddress(uri);

            //Define What...
            var client = new CharacterInfoService.CharacterInfoServiceClient(channel);
            var request = new CharacterRequest() { Id = new Random().Next(1, 5) };

            //Get Response
            var response = await client.GetCharacterInfoAsync(request);
            Console.WriteLine(FormatResponse(response));

            Console.ReadLine();
        }

        private static string FormatResponse(CharacterReply response)
            => $"The character selected was:" +
            $"\nName:    {response.Name}" +
            $"\nHeight:  {response.Height}" +
            $"\nMass:    {response.Mass}" +
            $"\nGender:  {response.Gender}" +
            $"\n______________________________\n\n";
    }
}
