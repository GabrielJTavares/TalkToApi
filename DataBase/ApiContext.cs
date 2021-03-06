using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalkToApi.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TalkToApi.DataBase
{
    public class ApiContext:IdentityDbContext<ApplicationUser>
    {
        public ApiContext(DbContextOptions<ApiContext> options):base(options)
        {

        }
        public DbSet<Mensagem> Mensagem { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
