using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Backend.Data;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Backend.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _applicationContext = null;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationContext context)
        {
            _applicationContext = context;
            _dbSet = context.Set<T>();
            try
            {
                _applicationContext.Database.EnsureCreated();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null,
            int? page = null)
        {
            // TODO: Remove hardcoded magic number
            const int numResults = 10;
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);

            if (includes != null)
            {
                query = includes(query);
            }

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && page > 0)
                return query.Skip(numResults * (page.GetValueOrDefault() - 1)).Take(numResults).ToList();
            else
                return query.ToList();
        }

        public T GetBy(Expression<Func<T, bool>> predicate, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault(predicate);
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _applicationContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public void Delete(int id)
        {
            T entityToDelete = _dbSet.Find(id);
            if (_applicationContext.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }
    }
}