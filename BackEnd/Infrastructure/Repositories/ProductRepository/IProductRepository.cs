using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ProductRepository
{
    public interface IProductRepository : IGenericRepositoryAsync<Product>
    {

    }
}