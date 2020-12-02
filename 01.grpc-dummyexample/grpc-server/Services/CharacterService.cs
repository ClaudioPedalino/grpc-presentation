using grpc.server.Data;
using grpc.server.Service;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace grpc.server
{
    public class CharacterService : CharacterInfoService.CharacterInfoServiceBase
    {
        private readonly ILogger<CharacterService> _logger;

        public CharacterService(ILogger<CharacterService> logger)
        {
            _logger = logger;
        }

        public override Task<CharacterReply> GetCharacterInfo(CharacterRequest request, ServerCallContext context)
        {
            var storage = DummyStorageData.GetData();

            var character = storage.Where(x => x.Id == request.Id).FirstOrDefault(); ;

            var reply = new CharacterReply()
            {
                Id = character.Id,
                Name = character.Name,
                Height = character.Height,
                Mass = character.Mass,
                Gender = character.Gender
            };

            return Task.FromResult(reply);
        }

    }
}
