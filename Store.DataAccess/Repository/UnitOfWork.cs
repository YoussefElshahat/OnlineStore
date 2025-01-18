using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        public ICategoryRepository Category { get; private set; }

        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
          _appDbContext = appDbContext;
            Category = new CategoryRepository(_appDbContext);
            Product = new ProductRepository(_appDbContext);
            Company = new CompanyRepository(_appDbContext);
            ShoppingCart = new ShoppingCartRepository(_appDbContext);
            AppUser = new AppUserRepository(_appDbContext);
            OrderDetail = new OrderDetailRepository(_appDbContext);
            OrderHeader = new OrderHeaderRepository(_appDbContext);
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }
    }
}
