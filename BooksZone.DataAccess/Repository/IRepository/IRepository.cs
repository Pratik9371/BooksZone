using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BooksZone.DataAccess.Repository.IRespository
{
    public interface IRepository<T> where T : class
    {
        //IEnumerable<T> GetAll(string includeProperties = null);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        T Get(int id);
       void Add(T entity);
       void Remove(int id);
       void Remove(T entity);
    }
}
