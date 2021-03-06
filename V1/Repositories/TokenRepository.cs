using TalkToApi.DataBase;
using TalkToApi.V1.Models;
using TalkToApi.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToApi.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApiContext _banco;
        public TokenRepository(ApiContext banco)
        {
            _banco = banco;
        }
        public Token FindToken(string refreshToken)
        {
           return _banco.Tokens.FirstOrDefault(a => a.RefreshToken == refreshToken && a.Utilizado==false);
        }

        public void RegisterToken(Token token)
        {
            _banco.Tokens.Add(token);
            _banco.SaveChanges();
        }

        public void UpdateToken(Token token)
        {
            _banco.Tokens.Update(token);
            _banco.SaveChanges();
        }
    }
}
