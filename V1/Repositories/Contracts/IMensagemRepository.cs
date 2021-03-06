using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.V1.Models;

namespace TalkToApi.V1.Repositories.Contracts
{
    public interface IMensagemRepository
    {
        List<Mensagem> FindAllMessages(string usuarioIdUm, string usuarioIdDois);
        Mensagem FindByIdMessages(int id);
        void Register(Mensagem mensagem);

        void Atualizar(Mensagem mensagem);
    }
}
