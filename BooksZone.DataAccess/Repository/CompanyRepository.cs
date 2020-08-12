using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooksZone.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
           var result = _db.Companies.FirstOrDefault(i => i.Id == company.Id);
            if (result != null)
            {
                result.Name = company.Name;
                result.StreetAddress = company.StreetAddress;
                result.City = company.City;
                result.State = company.State;
                result.PostalCode = company.PostalCode;
                result.PhoneNumber = company.PhoneNumber;
                result.IsAuthorizedCompany = company.IsAuthorizedCompany;
                _db.SaveChanges();
            }
        }
    }
}
