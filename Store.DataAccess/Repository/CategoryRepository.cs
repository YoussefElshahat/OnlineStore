using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;


namespace Store.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        

        public void Update(Category category)
        {
            _appDbContext.Categories.Update(category);
        }
    }
}
