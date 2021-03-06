using TalkToApi.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToApi.V1.Repositories.Contracts
{
    public interface ITokenRepository
    {
        void RegisterToken(Token token);
        Token FindToken(string refreshToken);
        void UpdateToken(Token token);

    }
}
