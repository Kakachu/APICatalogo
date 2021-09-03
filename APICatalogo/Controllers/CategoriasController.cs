using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Models.Context;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork contexto, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _context = contexto;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("Produtos")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            _logger.LogInformation("=====================GET api/categorias/produtos =========================");

            var categorias = _context.CategoriaRepository.GetCategoriasProdutos().ToList();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            _logger.LogInformation("=====================GET api/categorias =========================");

            var categorias = _context.CategoriaRepository.Get().ToList();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }


        [HttpGet("{id}", Name = "ObterCategoriaId")]
        public ActionResult<Categoria> Get(int id)
        {
            _logger.LogInformation($"=====================GET api/categorias/id = {id} =========================");
            
            try
            {
                var categoria = _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    _logger.LogInformation($"=====================GET api/categorias/id = {id} NOT FOUND =========================");
                    return NotFound($"a categoria com id ={id} não foi encontrada");
                }
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       "Erro ao tentar obter categorias do banco de dados");
            }
        }

        [HttpPost]
        public ActionResult<CategoriaDTO> Post([FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _context.CategoriaRepository.Add(categoria);
                _context.Commit();

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

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            { 
                if (id != categoriaDto.CategoriaId)
                {
                    return BadRequest($"A categoria com id = { id} não foi encontrada");
                }

                var categoria = _mapper.Map<Categoria>(categoriaDto);
                
                _context.CategoriaRepository.Update(categoria);
                _context.Commit();

                return Ok($"A categoria com id = { id} foi atualizada com sucesso ");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar a categoria com o id = {id}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            try
            {
                var categoria = _context.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound();
                }
                _context.CategoriaRepository.Delete(categoria);
                _context.Commit();
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
