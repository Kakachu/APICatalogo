using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace APICatalogo.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    //[Authorize(AuthenticationSchemes = "Bearer")]

    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public CategoriasController(IUnitOfWork contexto, IMapper mapper) //ILogger<CategoriasController> logger
        {
            _context = contexto;
            _mapper = mapper;
            //_logger = logger;
        }

        /// <summary>
        /// Obtém os produtos relacionados para cada categoria
        /// </summary>
        /// <returns>Objetos Categoria e respectivo Objetos Produtos</returns>
        [HttpGet("Produtos")]
        [EnableCors("AllowRequest")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            //_logger.LogInformation("=====================GET api/categorias/produtos =========================");

            var categorias = await _context.CategoriaRepository.GetCategoriasProdutos();

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
        }

        /// <summary>
        /// Retorna uma coleção de objetos Categoria
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        [HttpGet]
        [EnableCors("AllowRequest")]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            try
            {
                var categorias = _context.CategoriaRepository.Get().ToList();
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
                return categoriasDto;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<CategoriaDTO>> Get()
        //{
        //    try
        //    {
        //        var categorias = _context.CategoriaRepository.Get().ToList();
        //        var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
        //        return categoriasDto;
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }           
        //}

        [HttpGet("paginacao")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetPaginacao(int pag = 1, int reg = 5)
        {
            if (reg > 99)
                reg = 5;

            var categorias = _context.CategoriaRepository
                .LocalizaPagina<Categoria>(pag, reg)
                .ToList();

            var totalDeRegistros = _context.CategoriaRepository.GetTotalRegistros();
            var numeroPaginas = ((int)Math.Ceiling((double)totalDeRegistros / reg));

            Response.Headers["X-Total-Registros"] = totalDeRegistros.ToString();
            Response.Headers["X-Numero-Paginas"] = numeroPaginas.ToString();

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }

        /// <summary>
        /// Obtem uma Categoria pelo seu Id
        /// </summary>
        /// <param name="id">codigo do categoria</param>
        /// <returns>Objetos Categoria</returns>
        [HttpGet("{id}", Name = "ObterCategoriaId")]
        [EnableCors("AllowRequest")]
        public async Task<ActionResult<CategoriaDTO>> Get(int? id)
        {
           // _logger.LogInformation($"=====================GET api/categorias/id = {id} =========================");

            try
            {
                var categoria = await _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    //_logger.LogInformation($"=====================GET api/categorias/id = {id} NOT FOUND =========================");
                    return NotFound($"a categoria com id ={id} não foi encontrada");
                }
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       "Erro ao tentar obter categorias do banco de dados");
            }
        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST api/categorias
        ///     {
        ///        "categoriaId": 1,
        ///        "nome": "categoria1",
        ///        "imagemUrl": "http://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDto">objeto Categoria</param>
        /// <returns>O objeto Categoria incluida</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>
        [HttpPost]
        
        public async Task<ActionResult<CategoriaDTO>> Post([FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _context.CategoriaRepository.Add(categoria);
                await _context.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoriaId",
                    new { id = categoria.CategoriaId }, categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                      "Erro ao tentar criar uma nova categoria");
            }
        }

        /// <summary>
        /// Atualiza um produto pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                if (id != categoriaDto.CategoriaId)
                {
                    return BadRequest($"A categoria com id = { id} não foi encontrada");
                }

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _context.CategoriaRepository.Update(categoria);
                await _context.Commit();

                return Ok($"A categoria com id = { id} foi atualizada com sucesso ");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar a categoria com o id = {id}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Categoria>> Delete(int? id)
        {
            try
            {
                if (!id.HasValue)
                    return BadRequest();

                var categoria = await _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound();
                }
                _context.CategoriaRepository.Delete(categoria);
                await _context.Commit();
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     $"Erro ao tentar excluir categoria de id = {id} ");
            }


        }
    }
}
