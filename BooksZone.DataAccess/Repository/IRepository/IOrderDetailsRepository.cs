using BooksZone.DataAccess.Repository.IRespository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksZone.DataAccess.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails obj);
    }
}
