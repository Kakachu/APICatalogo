using APICatalogo.Models;
using GraphQL.Types;

namespace APICatalogo.GraphQL
{
    public class CategoriaType : ObjectGraphType<Categoria>
    {
        public CategoriaType()
        {
            Field(x => x.CategoriaId);
            Field(x => x.Nome);
            Field(x => x.ImagemUrl);

            Field<ListGraphType<CategoriaType>>("categorias");
        }
    }
}
