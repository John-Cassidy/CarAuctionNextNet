using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm, int PageNumber = 1, int PageSize = 4)
    {
        var query = DB.PagedSearch<Item>();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query.Match(Search.Full, searchTerm).SortByTextScore();
        }

        query.Sort(x => x.Ascending(a => a.Make));

        query.PageNumber(PageNumber);
        query.PageSize(PageSize);

        var results = await query.ExecuteAsync();
        return Ok(new
        {
            results = results.Results,
            pageCount = results.PageCount,
            totalCount = results.TotalCount
        });
    }
}
