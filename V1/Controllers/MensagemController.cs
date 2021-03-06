using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TalkToApi.Helpers.Constants;
using TalkToApi.V1.Models;
using TalkToApi.V1.Models.DTO;
using TalkToApi.V1.Repositories.Contracts;

namespace TalkToApi.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MensagemController : ControllerBase
    {
        private readonly IMensagemRepository _mensagemRepository;
        private readonly IMapper _mapper;
        public MensagemController(IMensagemRepository mensagemRepository,IMapper mapper)
        {
            _mensagemRepository = mensagemRepository;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("{UsuarioUmId}/{UsuarioDoisId}",Name ="encontrarMensages")]
        public ActionResult FindAllMessages(string usuarioUmId, string usuarioDoisId,[FromHeader(Name ="Accept")] string mediaType)
        {
            if (usuarioUmId == usuarioDoisId)
                return UnprocessableEntity();

            var mensagens = _mensagemRepository.FindAllMessages(usuarioUmId, usuarioDoisId);
            if (mediaType== ContantMEdiaType.Hateoas)
            {
               
                var listaMSG = _mapper.Map<List<Mensagem>, List<MensagemDTO>>(mensagens);

                var lista = new ListaDTO<MensagemDTO>() { Lista = listaMSG };
                lista.Links.Add(new LinkDTO("_self", Url.Link("encontrarMensages", new { usuarioUmId = usuarioUmId, usuarioDoisId = usuarioDoisId }), "GET"));

                return Ok(lista);
            }
            else
            {
                return Ok(mensagens);
            }

            
        }
        [Authorize]
        [HttpPost("",Name ="CadastroMensagem")]
        public ActionResult RegisterMessage([FromBody]Mensagem mensagem, [FromHeader(Name = "Accept")] string mediaType)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _mensagemRepository.Register(mensagem);

                    if (mediaType == ContantMEdiaType.Hateoas)
                    {
                        var MensagemDTO = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
                        MensagemDTO.Links.Add(new LinkDTO("_self", Url.Link("CadastroMensagem", null), "POST"));
                        MensagemDTO.Links.Add(new LinkDTO("_Atualizar", Url.Link("AtualizarParcial", new { id = mensagem.Id }), "PACTH"));

                        return Ok(MensagemDTO);
                    }
                    else
                    {
                        return Ok(mensagem);
                    }

                     
                }
                catch (Exception e)
                {
                    return UnprocessableEntity(e);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }


        }


        [Authorize]
        [HttpPatch("{id}",Name = "AtualizarParcial")]
        public ActionResult UpdatePartial(int id, [FromBody] string request, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (request == null)
                return BadRequest();

            var mensagem = _mensagemRepository.FindByIdMessages(id);

            mensagem.Texto = request;
            mensagem.Atualizado = DateTime.UtcNow;

            _mensagemRepository.Atualizar(mensagem);

            if (mediaType == ContantMEdiaType.Hateoas)
            {

                var MensagemDTO = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
                MensagemDTO.Links.Add(new LinkDTO("_Self", Url.Link("AtualizarParcial", new { id = mensagem.Id }), "PACTH"));

                return Ok(MensagemDTO);

            }
            else
            {
                return Ok(mensagem);
            }

            
        }

    }
}