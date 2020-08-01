using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooksZone.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
           var result = _db.Products.FirstOrDefault(i => i.Id == product.Id);
            if (result != null)
            {
                if (product.ImageUrl != null)
                {
                    result.ImageUrl = product.ImageUrl;
                }

                result.Title = product.Title;
                result.Description = product.Description;
                result.Author = product.Author;
                result.Price = product.Price;
                result.ListPrice = product.ListPrice;
                result.Price50 = product.Price50;
                result.Price100 = product.Price100;
                result.ISBN = product.ISBN;
                result.CategoryId = product.CategoryId;
                result.CoverTypeId = product.CoverTypeId;

                _db.SaveChanges();
            }
        }
    }
}
