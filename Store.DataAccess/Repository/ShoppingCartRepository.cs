using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;


namespace Store.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDbContext _appDbContext;
        public ShoppingCartRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        

        public void Update(ShoppingCart shoppingCart)
        {
            _appDbContext.Update(shoppingCart);
        }
    }
}
