using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;


namespace Store.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly AppDbContext _appDbContext;
        public OrderDetailRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        

        public void Update(OrderDetail Obj)
        {
            _appDbContext.Update(Obj);
        }
    }
}
