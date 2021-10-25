using APICatalogo.Repository;
using GraphQL;
using GraphQL.Types;

namespace APICatalogo.GraphQL
{
    public class CategoriaQuery : ObjectGraphType
    {

        public CategoriaQuery(IUnitOfWork _context)
        {
            Field<CategoriaType>("categoria",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType>() { Name = "id" }),
                    resolve: context =>
                    {
                        var id = context.GetArgument<int>("id");
                        return _context.CategoriaRepository
                                       .GetById(c => c.CategoriaId == id);
                    });

            Field<ListGraphType<CategoriaType>>("categorias",
                resolve: context =>
                {
                    return _context.CategoriaRepository.Get();
                });
        }
    }
}
