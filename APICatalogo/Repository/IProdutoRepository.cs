using APICatalogo.Models;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    interface IProdutoRepository : IRepository<Produto>
    {
        IEnumerable<Produto> GetProdutosPorPreco();
    }
}
