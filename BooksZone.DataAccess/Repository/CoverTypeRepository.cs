using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooksZone.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CoverType coverType)
        {
           var result = _db.CoverTypes.FirstOrDefault(i => i.Id == coverType.Id);
            if (result != null)
            {
                result.Name = coverType.Name;
                _db.SaveChanges();
            }
        }
    }
}
