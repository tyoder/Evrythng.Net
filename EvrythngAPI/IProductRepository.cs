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
    }
}
