using System.Collections.Generic;
using System.Linq;

namespace UnitTestWebApiController.Models
{
    public class FakeProductRepository
    {
        int _nextId = 1;
        Dictionary<int, Product> _items = new Dictionary<int, Product>();

        public IEnumerable<Product> Get()
        {
            return _items.Values.AsEnumerable();
        }

        public Product GetById(int id)
        {
            Product product = null;
            _items.TryGetValue(id, out product);
            return product;
        }

        public void Add(Product product)
        {
            product.Id = _nextId++;
            _items[product.Id] = product;
        }

        public bool Delete(int id)
        {
            return _items.Remove(id);
        }
    }
}