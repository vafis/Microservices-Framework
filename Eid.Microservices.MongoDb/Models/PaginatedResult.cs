using System.Collections.Generic;

namespace Eid.Microservices.MongoDb.Models
{
    public class PaginatedResult<T> where T : class
    {
        public long TotalCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public IEnumerable<T> Data { get; private set; }

        public PaginatedResult(IEnumerable<T> data, long totalCount, int pageIndex, int pageSize)
        {
            Data = data ?? new List<T>();
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public static PaginatedResult<T> Empty(int pageIndex = 0, int pageSize = 0)
        {
            return new PaginatedResult<T>(null, 0, pageIndex, pageSize);
        }
    }
}
