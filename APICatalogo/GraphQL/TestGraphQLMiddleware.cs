using APICatalogo.Repository;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace APICatalogo.GraphQL
{
    public class TestGraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUnitOfWork _context;

        public TestGraphQLMiddleware(RequestDelegate next, IUnitOfWork contexto)
        {
            _next = next;
            _context = contexto;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // verifica se o caminho do request é /graphql
            if (httpContext.Request.Path.StartsWithSegments("/graphql"))
            {
                //tenta ler o corpo do request usando um StreamReader
                using (var stream = new StreamReader(httpContext.Request.Body))
                {
                    var query = await stream.ReadToEndAsync();

                    if (!String.IsNullOrWhiteSpace(query))
                    {
                        var schema = new Schema
                        {
                            Query = new CategoriaQuery(_context)
                        };

                        var result = await new DocumentExecuter().ExecuteAsync(options =>
                        {
                            options.Schema = schema;
                            options.Query = query;
                        });
                        await WriteResult(httpContext, result);
                    }
                }
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task WriteResult(HttpContext httpContext,
           ExecutionResult result)
        {
            var json = new DocumentWriter(indent: true).Write(result);
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }
    }
}
