using JOStore.Data;
using Store.DataAccess.Repository.IRepository;

namespace Store.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        public ICategoryRepository Category { get; private set; }

        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
          _appDbContext = appDbContext;
            Category = new CategoryRepository(_appDbContext);
            Product = new ProductRepository(_appDbContext);
            Company = new CompanyRepository(_appDbContext);
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }
    }
}
