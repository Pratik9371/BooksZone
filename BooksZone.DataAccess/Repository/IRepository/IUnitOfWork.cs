using System;
using System.Collections.Generic;
using System.Text;

namespace BooksZone.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; }
        void Save();
    }
}
