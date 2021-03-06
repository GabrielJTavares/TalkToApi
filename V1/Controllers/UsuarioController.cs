using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TalkToApi.V1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using TalkToApi.V1.Repositories.Contracts;
using TalkToApi.V1.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using TalkToApi.Helpers.Constants;

namespace TalkToApi.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsuarioController(IUsuarioRepository usuarioRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("")]
        public ActionResult FindAllUsers([FromHeader(Name = "Accept")] string mediaType)
        {
            var usuarioAppUser = _userManager.Users.ToList();
            

            if (mediaType == ContantMEdiaType.Hateoas)
            {
                var listaUsuarioDTO = _mapper.Map<List<ApplicationUser>, List<UsuarioDTO>>(usuarioAppUser);

                foreach (var usuarioDTO in listaUsuarioDTO)
                {
                      usuarioDTO.Links.Add(new LinkDTO("_self", Url.Link("ObterUsuario", new { id = usuarioDTO.Id }), "GET"));
                }
                return Ok(listaUsuarioDTO);
            }
            else
            {
                var usuarioPadrao = _mapper.Map<List<ApplicationUser>, List<Usuario>>(usuarioAppUser);

                return Ok(usuarioPadrao);
            }
        }

        [Authorize]
        [HttpGet("{id}", Name = "ObterUsuario")]
        public ActionResult Find(string id, [FromHeader(Name = "Accept")] string mediaType)
        {
            var usuario = _userManager.FindByIdAsync(id).Result;
            if (usuario == null)
                return NotFound();

            
            if (mediaType == ContantMEdiaType.Hateoas)
            {
                var usuarioDTO = _mapper.Map<ApplicationUser, UsuarioDTO>(usuario);
                usuarioDTO.Links.Add(new LinkDTO("_self", Url.Link("ObterUsuario", new { id = usuarioDTO.Id }), "GET"));
                usuarioDTO.Links.Add(new LinkDTO("_Atualizar", Url.Link("AtualizarUsuario", new { id = usuarioDTO.Id }), "PUT"));
                return Ok(usuarioDTO);
            }
            else
            {
                var usuarioPadrao = _mapper.Map<ApplicationUser, Usuario>(usuario);

                return Ok(usuarioPadrao);
              
            }
            

            
        }
        /// <summary>
        /// Faz o Login
        /// </summary>
        /// <param name="usuarioDTO">Email</param>
        /// <returns>retorna um token</returns>
        [HttpPost("login")]
        public ActionResult Login([FromBody]UsuarioDTO usuarioDTO)
        {
            ModelState.Remove("ConfirmarSenha");
            ModelState.Remove("Nome");
            if (ModelState.IsValid)
            {
                ApplicationUser usuario = _usuarioRepository.Find(usuarioDTO.Email, usuarioDTO.Senha);
                if (usuario != null)
                {
                    return GerarNovoToken(usuario);
                }
                else
                {
                    return NotFound("Usuário não localizado!");
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }


        /// <summary>
        /// faz a renovação do token para não ser necessario um novo login
        /// </summary>
        /// <param name="tokenDTO">token</param>
        /// <returns>um novo token</returns>
        [HttpPost("renovar")]
        public ActionResult Renovar([FromBody]TokenDTO tokenDTO)
        {
            var refreshTokenDB = _tokenRepository.FindToken(tokenDTO.RefreshToken);

            if (refreshTokenDB == null)
                return NotFound();

            //atualizado token
            refreshTokenDB.Atualizado = DateTime.Now;
            refreshTokenDB.Utilizado = true;
            _tokenRepository.UpdateToken(refreshTokenDB);

            //gera um novo token
            var usuario = _usuarioRepository.Find(refreshTokenDB.UsuarioId);

            return GerarNovoToken(usuario);


        }

        /// <summary>
        /// Cadastra um novo usuario
        /// </summary>
        /// <param name="usuarioDTO">email</param>
        /// <returns>o novo usuario</returns>
        [HttpPost("",Name ="CadastrarUsuario")]
        public ActionResult Cadastrar([FromBody] UsuarioDTO usuarioDTO, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser usuario = new ApplicationUser();
                usuario.FullName = usuarioDTO.Nome;
                usuario.UserName = usuarioDTO.Email;
                usuario.Email = usuarioDTO.Email;
                var resultado = _userManager.CreateAsync(usuario, usuarioDTO.Senha).Result;

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);
                    }
                    return UnprocessableEntity(erros);
                }
                else
                {

                    if (mediaType == ContantMEdiaType.Hateoas)
                    {
                        var usuarioDTOdb = _mapper.Map<ApplicationUser, UsuarioDTO>(usuario);
                        usuarioDTOdb.Links.Add(new LinkDTO("_self", Url.Link("CadastrarUsuario", new { id = usuarioDTOdb.Id }), "POST"));
                        usuarioDTOdb.Links.Add(new LinkDTO("_Atualizar", Url.Link("AtualizarUsuario", new { id = usuarioDTOdb.Id }), "PUT"));
                        usuarioDTO.Links.Add(new LinkDTO("_obter", Url.Link("ObterUsuario", new { id = usuarioDTO.Id }), "GET"));

                        return Ok(usuarioDTOdb);
                    }
                    else
                    {
                        var usuarioPadrao = _mapper.Map<ApplicationUser, Usuario>(usuario);

                        return Ok(usuarioPadrao);
                    }
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [Authorize]
        [HttpPut("{id}", Name = "AtualizarUsuario")]
        public ActionResult Atualizar(string id,[FromBody] UsuarioDTO usuarioDTO, [FromHeader(Name = "Accept")] string mediaType)
        {
            ApplicationUser usuario = _userManager.GetUserAsync(HttpContext.User).Result;
            if (usuario.Id != id)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
             
                usuario.FullName = usuarioDTO.Nome;
                usuario.UserName = usuarioDTO.Email;
                usuario.Email = usuarioDTO.Email;
                usuario.Slogan = usuarioDTO.Slogan;
                var resultado = _userManager.UpdateAsync(usuario).Result;
                _userManager.RemovePasswordAsync(usuario);
                _userManager.AddPasswordAsync(usuario,usuarioDTO.Senha);

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);
                    }
                    return UnprocessableEntity(erros);
                }
                else
                {
                    if (mediaType == ContantMEdiaType.Hateoas)
                    {
                        var usuarioDTOdb = _mapper.Map<ApplicationUser, UsuarioDTO>(usuario);
                        usuarioDTOdb.Links.Add(new LinkDTO("_self", Url.Link("AtualizarUsuario", new { id = usuarioDTOdb.Id }), "PUT"));
                        usuarioDTO.Links.Add(new LinkDTO("_obter", Url.Link("ObterUsuario", new { id = usuarioDTO.Id }), "GET"));
                        return Ok(usuarioDTO);
                    }
                    else
                    {
                        var usuarioPadrao = _mapper.Map<ApplicationUser, Usuario>(usuario);

                        return Ok(usuarioPadrao);
                    }
                    
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        private TokenDTO BuildToken(ApplicationUser usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email,usuario.Email),
                 new Claim(JwtRegisteredClaimNames.Sub,usuario.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CH4V35-jwt!@e-t4lkT04p1"));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: exp,
                signingCredentials: sign

                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();
            var expritationRefreshToken = DateTime.UtcNow.AddHours(2);

            var TokenDTO = new TokenDTO { Token = tokenString, Expiration = exp, ExpirationRefreshToken = expritationRefreshToken, RefreshToken = refreshToken };
            return TokenDTO;
        }

        private ActionResult GerarNovoToken(ApplicationUser usuario)
        {
            var token = BuildToken(usuario);
            var tokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirtationRefreshToken = token.ExpirationRefreshToken,
                ExpirtationToken = token.Expiration,
                Usuario = usuario,
                Criado = DateTime.Now,
                Utilizado = false
            };

            _tokenRepository.RegisterToken(tokenModel);
            return Ok(token);
        }
    }
}