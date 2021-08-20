using APICatalogo.Models;
using APICatalogo.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;


        public CategoriasController(AppDbContext contexto)
        {
            _context = contexto;
        }

        [HttpGet("Produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            return _context.Categorias.Include(x => x.Produtos).ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                return _context.Categorias.AsNoTracking().ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       "Erro ao tentar obter categorias do banco de dados");
            }
        }


        [HttpGet("{id}", Name = "ObterCategoriaId")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound($"a categoria com id ={id} não foi encontrada");
                }
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                       "Erro ao tentar obter categorias do banco de dados");
            }
        }

        [HttpPost]
        public ActionResult<Categoria> Post([FromBody] Categoria categoria)
        {
            try
            {
                _context.Categorias.Add(categoria);
                _context.SaveChanges();
                return new CreatedAtRouteResult("ObterCategoriaId",
                    new { id = categoria.CategoriaId }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                      "Erro ao tentar criar uma nova categoria");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.CategoriaId)
                {
                    return BadRequest($"A categoria com id = { id} não foi encontrada");
                }

                _context.Entry(categoria).State = EntityState.Modified;
                _context.SaveChanges();
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
                var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound();
                }
                _context.Categorias.Remove(categoria);
                _context.SaveChanges();
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
