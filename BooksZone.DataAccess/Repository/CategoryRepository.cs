using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooksZone.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
           var result = _db.Categories.FirstOrDefault(i => i.Id == category.Id);
            if (result != null)
            {
                result.Name = category.Name;
                _db.SaveChanges();
            }
        }
    }
}
