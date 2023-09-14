﻿using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.ResourceParameters;

public class AuthorsResourceParameters
{
    const int maxPageSize = 20;
    public string? MainCategory { get; set; }

    [FromQuery(Name = "q")]
    public string? SearchQuery { get; set; }
    public int PageNumber { get; set; } = 1;

    public int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
    }
}