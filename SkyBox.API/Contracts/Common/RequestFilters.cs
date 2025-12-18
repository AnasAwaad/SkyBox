namespace SkyBox.API.Contracts.Common;

public class RequestFilters
{
    /// <summary>Page number (starting from 1).</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Number of items per page.</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>Search by file or folder name.</summary>
    public string? SearchValue { get; set; }

    /// <summary>Column name to sort by (Name, CreatedAt).</summary>
    public string? SortColumn { get; set; }

    /// <summary>Sort direction (asc or desc).</summary>
    public string? SortDirection { get; set; } = "ASC";
}
