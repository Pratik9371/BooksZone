﻿using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository.IRespository;
using BooksZone.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooksZone.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            // _db.Set<T>().Add(entity); if we use normally
            dbSet.Add(entity);
        }

        public T Get(int id)
        {
           return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
           return dbSet.ToList();
        }

        public void Remove(int id)
        {
           T entity = dbSet.Find(id);
           dbSet.Remove(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}