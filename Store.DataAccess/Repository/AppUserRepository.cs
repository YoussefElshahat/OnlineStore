using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;


namespace Store.DataAccess.Repository
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private readonly AppDbContext _appDbContext;
        public AppUserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }



    }
}
