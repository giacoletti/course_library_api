namespace CourseLibrary.API.Models;

public class PaginationMetadataDto
{
    public int totalCount { get; set; }
    public int pageSize { get; set; }
    public int currentPage { get; set; }
    public int totalPages { get; set; }
}
