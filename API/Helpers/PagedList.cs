
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T>:List<T>
    {
        public PagedList(IEnumerable<T> items,int Count,int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(Count/(double)pageSize);
            PageSize = pageSize;
            TotalCount = Count;
            AddRange(items);
        }

       

        public int CurrentPage{set;get;}
      public int TotalPages{set;get;}
       public int PageSize{set;get;}
        public int TotalCount{set;get;}

        

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,int pageNumber,int pageSize)
        {
            var count = await source.CountAsync();
            var items=await source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
                    
        }


    }
}