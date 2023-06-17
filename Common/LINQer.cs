namespace Helpers.Common
{
    /// <summary>
    /// LINQ Helper Class for working with LINQ objects.
    /// </summary>
    public class LINQer
    {
        /// <summary>
        /// Asynchronous paging for an IQueryable Object, with paging starting from 1 and choosing the page size you need.
        /// </summary>
        /// <typeparam name="T">A type that can be querable into.</typeparam>
        /// <param name="input">The object to be paged.</param>
        /// <param name="page">Page number, starting from 1.</param>
        /// <param name="pagesize">The number of objects to be returned in each page.</param>
        /// <returns>The number of objects specified in pagesize, in the given page number.</returns>
        public static async Task<List<T>> GetPageAsync<T>(IQueryable<T> input, int page, int pagesize)
        {
            return (List<T>)await Task.FromResult(input.Skip((page - 1) * pagesize).Take(pagesize));
        }

        /// <summary>
        /// Synchronous paging for an IQueryable Object, with paging starting from 1 and choosing the page size you need.
        /// </summary>
        /// <typeparam name="T">A type that can be querable into.</typeparam>
        /// <param name="input">The object to be paged.</param>
        /// <param name="page">Page number, starting from 1.</param>
        /// <param name="pagesize">The number of objects to be returned in each page.</param>
        /// <returns>The number of objects specified in pagesize, in the given page number.</returns>
        public static List<T> GetPage<T>(IQueryable<T> input, int page, int pagesize)
        {
            return (List<T>)input.Skip((page - 1) * pagesize).Take(pagesize);
        }
    }
}
