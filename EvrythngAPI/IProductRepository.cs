using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvrythngAPI
{
    public interface IProductRepository
    {
        void CreateProduct(Product product);

        Product GetProduct(string productId);

        List<Product> GetProducts();

        void UpdateProduct(Product product);

        void DeleteProduct(string productId);
    }
}
