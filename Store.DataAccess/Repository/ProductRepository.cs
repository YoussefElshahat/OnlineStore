using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(Product product)
        { 
            var productFromDb = _appDbContext.
                Products.FirstOrDefault(x => x.Id == product.Id);
            if (product != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.Description = product.Description;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.Price = product.Price;
                productFromDb.ProductImages = product.ProductImages;
                

            }
        }

    }
}
