using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TalkToApi.V1.Models;

namespace TalkToApi.V1.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Slogan { get; set; }
     
    }
}
