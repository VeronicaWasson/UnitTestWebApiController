using System.Collections.Generic;

namespace UnitTestWebApiController.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> Get();
        Product GetById(int id);
        void Add(Product product);
        bool Delete(int id);
    }
}
