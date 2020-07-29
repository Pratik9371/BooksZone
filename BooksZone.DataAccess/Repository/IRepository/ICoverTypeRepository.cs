﻿using BooksZone.DataAccess.Repository.IRespository;
using BooksZone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooksZone.DataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}