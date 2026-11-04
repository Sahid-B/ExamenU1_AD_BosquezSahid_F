using Microsoft.EntityFrameworkCore;

namespace PrimeVideoApp.Models;

public interface IPaginatedList
{
    int PageIndex { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}

public class PaginatedList<T> : List<T>, IPaginatedList
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        if (pageIndex < 1)
            pageIndex = 1;
        if (totalPages > 0 && pageIndex > totalPages)
            pageIndex = totalPages;

        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
