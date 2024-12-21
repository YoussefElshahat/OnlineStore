using JOStore.Data;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using System.Linq.Expressions;

namespace Store.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _appDbContext;
        internal DbSet<T> dbSet;
        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            this.dbSet = _appDbContext.Set<T>();
            
        }
        public void Add(T entity)
        {
            _appDbContext.Add(entity);
        }
        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }
        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }
        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

        

        
    }
}
