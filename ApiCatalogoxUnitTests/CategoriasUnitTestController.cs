using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models.Context;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiCatalogoxUnitTests
{
    public class CategoriasUnitTestController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        //String de conexão
        public static string connectionString = "Server=localhost;Database=CatalogoDB;Uid=Kakachu;Pwd=Musicass111.";

        //Construtor estático para inicializar o dbContextOptions
        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                        .Options;
        }
        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);
            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            _unitOfWork = new UnitOfWork(context);
        }

        //testes unitários================================================
        // testar o método GET
        //====================================Get(int id) =====================================
        [Fact]
        public async Task GetCategoriaById_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var catId = 2;

            //Act  
            var data = await controller.Get(catId);
            Console.WriteLine(data);

            //Assert  
            Assert.IsType<CategoriaDTO>(data.Value);
        }

        [Fact]
        public void GetCategoriaById_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var catId = 9999;

            //Act  
            var data = controller.Get(catId);

            //Assert  
            Assert.IsType<NotFoundResult>(data.Result);
        }
        [Fact]
        public void GetCategoriaById_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            int? catId = null;

            //Act  
            var data = controller.Get(catId);

            //Assert  
            Assert.IsType<BadRequestResult>(data.Result);
        }

        [Fact]
        public async Task GetCategoriaById_MatchResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            int? catId = 1;

            //Act  
            var data = await controller.Get(catId);

            //Assert  
            Assert.IsType<CategoriaDTO>(data.Value);
            var cat = data.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            Assert.Equal("Bebidas alterada", cat.Nome);
            Assert.Equal("bebidas21.jpg", cat.ImagemUrl);
        }

        //===============================================Get=====================================
        [Fact]
        public void GetCategorias_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var categoriaParameters = new CategoriasParameters();
            //Act  
            var data = controller.Get();

            //Assert  
            Assert.IsType<List<CategoriaDTO>>(data.Value);
        }

        [Fact]
        public void GetCategorias_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var categoriaParameters = new CategoriasParameters();
            //Act  
            var data = controller.Get();
            //data = null;

            //if (data != null)
            //Assert  
            Assert.IsType<BadRequestResult>(data.Result);
        }

        [Fact]
        public void GetCategorias_MatchResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var categoriaParameters = new CategoriasParameters();
            //Act  
            var data = controller.Get();
            //Assert  
            Assert.IsType<List<CategoriaDTO>>(data.Value);
            var cat = data.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas alterada", cat[0].Nome);
            Assert.Equal("bebidas21.jpg", cat[0].ImagemUrl);

            Assert.Equal("Lanches", cat[1].Nome);
            Assert.Equal("lanches.jpg", cat[1].ImagemUrl);
        }

        //====================================Post=====================================

        [Fact]
        public void Post_Categoria_AddValidData_Return_CreatedResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);

            var cat = new CategoriaDTO() { Nome = "Teste Unitario 1", ImagemUrl = "testecat.jpg" };

            //Act  
            var data = controller.Post(cat);

            //Assert  
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        [Fact]
        public void Post_Categoria_Add_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);

            var cat = new CategoriaDTO()
            {
                Nome = "Teste Unitario dados invalidos de nome muito " +
                "loooooooooooooooooonnnnnnnnnnngggggggggggggggggggoooooooooooooo"
                ,
                ImagemUrl = "testecat.jpg"
            };

            //Act              
            var data = controller.Post(cat);

            //Assert  
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async Task Post_Categoria_Add_ValidData_MatchResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);

            var cat = new CategoriaDTO() { Nome = "Teste Unitario 1", ImagemUrl = "testecat.jpg" };

            //Act  
            var data = await controller.Post(cat);

            //Assert  
            Assert.IsType<CreatedAtRouteResult>(data);

            var okResult = data.Should().BeOfType<CreatedAtRouteResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            Assert.Equal(3, okResult.Value);
        }

        //===========================================Put =====================================

        [Fact]
        public void Put_Categoria_Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var catId = 2;

            //Act  
            var existingPost = controller.Get(catId).Result;
            //var okResult = existingPost.Should().BeOfType<CategoriaDTO>().Subject;
            var result = existingPost.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;
            //var result = okResult.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = catId;
            catDto.Nome = "Categoria Atualizada - Testes 1";
            catDto.ImagemUrl = result.ImagemUrl;

            var updatedData = controller.Put(catId, catDto).Result;

            //Assert  
            Assert.IsType<OkObjectResult>(updatedData);
        }

        [Fact]
        public void Put_Categoria_Update_InvalidData_Return_BadRequest()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var catId = 1;

            //Act  
            var existingPost = controller.Get(catId).Result;
            //var okResult = existingPost.Should().BeOfType<CategoriaDTO>().Subject;
            var result = existingPost.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;
            //var result = okResult.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = result.CategoriaId;
            catDto.Nome = "Categoria Atualizada - Testes 1 com nome muiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiitttttttttttttttttttttttttttttttttooooooooooooooo looooooooooooooooooooooooooooooooooooooooooooooonnnnnnnnnnnnnnnnnnnnnnnnnnnngo";
            catDto.ImagemUrl = result.ImagemUrl;


            var data = controller.Put(catId, catDto).Result;

            var putResult = data.Should().BeAssignableTo<ObjectResult>().Subject;


            //Assert  
            Assert.Equal(putResult.StatusCode.Value, 500);
        }
        //=======================================Delete ===================================
        [Fact]
        public async Task Delete_Categoria_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var catId = 1;

            //Act  
            var data = await controller.Delete(catId);

            //Assert  
            Assert.IsType<OkObjectResult>(data.Result);
        }

        [Fact]
        public async Task Delete_Categoria_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            var catId = 999999;

            //Act  
            var data = await controller.Delete(catId);

            //Assert  
            Assert.IsType<NotFoundResult>(data.Result);
        }

        [Fact]
        public async Task Delete_Categoria_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new CategoriasController(_unitOfWork, _mapper);
            int? catId = null;

            //Act  
            var data = await controller.Delete(catId);

            //Assert  
            Assert.IsType<BadRequestResult>(data.Result);
        }
    }
}
