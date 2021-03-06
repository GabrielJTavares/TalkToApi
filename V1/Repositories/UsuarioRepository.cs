﻿using Microsoft.AspNetCore.Identity;
using TalkToApi.V1.Models;
using TalkToApi.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkToApi.V1.Repositories
{
   
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
       
        public ApplicationUser Find(string email, string senha)
        {
            var usuario = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(usuario, senha).Result)
            {
                return usuario;
            }
            else
            {
                throw new Exception("Usuário não localizado");
            }

        }

      

        public void Register(ApplicationUser usuario,string senha)
        {
            var result=_userManager.CreateAsync(usuario, senha).Result;
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var erro in result.Errors)
                {
                    sb.Append(erro.Description);

                }
                throw new Exception($"Usuário não cadastrado! {sb.ToString()}");
            }
        }

        public ApplicationUser Find(string id)
        {
            return _userManager.FindByIdAsync(id).Result;         

        }

    }
}