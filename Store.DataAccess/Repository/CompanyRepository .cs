using JOStore.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _appDbContext;
        public CompanyRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Update(Company company)
        { 
            var companyFromDb = _appDbContext.
                Companies.FirstOrDefault(x => x.Id == company.Id);
            if (company != null)
            {
                companyFromDb.Name = company.Name;
                companyFromDb.State = company.State;
                companyFromDb.City = company.City;
                companyFromDb.StreetAdress = company.StreetAdress;
                companyFromDb.PhoneNumber = company.PhoneNumber;
                companyFromDb.PostalCode = company.PostalCode;



            }
        }

    }
}
