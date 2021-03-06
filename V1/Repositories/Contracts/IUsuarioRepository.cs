using TalkToApi.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToApi.V1.Repositories.Contracts
{
    public interface IUsuarioRepository
    {
        void Register(ApplicationUser usuario,string senha);
        ApplicationUser Find(string email, string senha);
        ApplicationUser Find(string id);
    }
}
