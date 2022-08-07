
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LinkVault.Models;

namespace LinkVault.Context
{

    public abstract class PagedResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
    }

    public class PagedResult<T> : PagedResultBase where T : class
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }

    public static class Extensions
    {
        public static PagedResult<P> GetPaged<P, T>(this DbSet<T> dbSet, Func<T, bool> predicate, int page, int pageSize)
            where P : class
            where T : ModelBase<P>
        {
            var result = new PagedResult<P>();
            result.CurrentPage = page;
            result.PageSize = pageSize;

            var pageCount = (double)dbSet.AsQueryable().Where(predicate).Count() / result.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * result.PageSize;
            result.Results = dbSet.AsQueryable().Skip(skip).Where(predicate).Select(x => x.AsDto()).ToList();

            return result;
        }
    }
}