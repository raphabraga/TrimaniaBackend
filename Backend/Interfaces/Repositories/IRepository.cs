using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Backend.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null,
            int? page = null);
        public Task<T> GetBy(Expression<Func<T, bool>> predicated, string includeProperties = "");
        public void Insert(T entity);
        public void Update(T entity);
        public void Delete(int id);
    }
}