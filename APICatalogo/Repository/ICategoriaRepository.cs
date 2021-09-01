using APICatalogo.Models;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    interface ICategoriaRepository : IRepository<Categoria>
    {
        IEnumerable<Categoria> GetCategoriasProdutos();
    }
}
