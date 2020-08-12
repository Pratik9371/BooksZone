using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooksZone.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.Update(obj);
        }
    }
}
