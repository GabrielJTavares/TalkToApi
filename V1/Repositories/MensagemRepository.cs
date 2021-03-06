using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.DataBase;
using TalkToApi.V1.Models;
using TalkToApi.V1.Repositories.Contracts;

namespace TalkToApi.V1.Repositories
{
    public class MensagemRepository : IMensagemRepository
    {
        private readonly ApiContext _banco;
        public MensagemRepository(ApiContext banco)
        {
            _banco = banco;
        }


        public List<Mensagem> FindAllMessages(string usuarioIdUm, string usuarioIdDois)
        {
            return _banco.Mensagem.Where(a => (a.DeId == usuarioIdUm || a.DeId == usuarioIdDois) && (a.ParaId == usuarioIdUm || a.ParaId == usuarioIdDois)).OrderBy(a=>a.Id).ToList();
        }

        public void Register(Mensagem mensagem)
        {
            _banco.Mensagem.Add(mensagem);
            _banco.SaveChanges();

        }


        public void Atualizar(Mensagem mensagem)
        {
            _banco.Mensagem.Update(mensagem);
            _banco.SaveChanges();
        }

        public Mensagem FindByIdMessages(int id)
        {
            return _banco.Mensagem.Find(id);
        }
    }
}
