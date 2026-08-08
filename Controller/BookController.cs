using Bookshop.Data;
using Bookshop.Entities;
using Bookshop.Interfaces.Repositories;
using Bookshop.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.Controller;

[Route("api/[controller]")]
[ApiController]
public class BookController(IRepositoryWrapper repo, BookshopDbContext context) : ControllerBase
{
    private readonly IRepositoryWrapper _repo = repo ?? throw new Exception("Repo is null");
    private readonly BookshopDbContext _context = context ?? throw new Exception("Context is null");

    [HttpGet]
    [EndpointSummary("Get Book List with Pagination")]
    public async Task<IActionResult> GetAsync(int skipRows, int pageSize, string? q, string? sortField, int order)
    {

        IQueryable<Book> booksQuery = BookQuery( q, sortField, order);

        int recordsTotal = await booksQuery.CountAsync();
        List<Book> records = await booksQuery
            .AsNoTracking()
            .Skip(skipRows)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new DefaultResponseModel
        {
            Success = true,
            Statuscode = StatusCodes.Status200OK,
            Message = "success pagination",
            Data = new { records, recordsTotal }
        });
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get Book by Id")]
    public async Task<IActionResult> GetAsync(int id)
    {
        IReadOnlyList<Book>? data = await _repo.Books.GetAsync(x => x.Id == id);
        if (data == null || data.Count == 0)
        {
            return NotFound(new DefaultResponseModel
            {
                Success = false,
                Statuscode = 404,
                Message = "Book not found",
                Data = null
            });
        }
        return Ok(new DefaultResponseModel
        {
            Success = true,
            Statuscode = 200,
            Message = "Success",
            Data = data
        });
    }
    
    [HttpPost]
    [EndpointSummary("CreateBook")]
    public async Task<IActionResult> CreateBook([FromBody] Book model)
    {
        Book data = new Book()
        {
            Title = model.Title,
            Author = model.Author,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId,
            StockQuantity = model.StockQuantity,
            ImageUrl = model.ImageUrl,
            Isbn = model.Isbn,
            CreatedOn = DateTime.Now
        };
        _repo.Books.Create(data);
        return await _repo.SaveAsync()
            ? Ok(new DefaultResponseModel
            {
                Success = true,
                Statuscode = 200,
                Message = "Book created successfully",
                Data = data
            })
            : BadRequest(new DefaultResponseModel
            {
                Success = false,
                Statuscode = 400,
                Message = "Failed to create book",
                Data = null
            }); 
    }

    [NonAction]
    private IQueryable<Book> BookQuery(string? q, string? sortField, int order)
    {
        IQueryable<Book> query = _context.Books.Where(x => !x.DeletedOn.HasValue);

        //if (sDate.HasValue && eDate.HasValue)
        //{
        //    query = query.Where(x => x.CreatedOn >= sDate.Value && x.CreatedOn <= eDate.Value.AddDays(1));
        //}

        if (!string.IsNullOrWhiteSpace(q))
        {
            string search = q.Trim().ToLower();
            query = query.Where(x => (x.Title != null && x.Title.ToLower().Contains(search))
                                  || (x.Author != null && x.Author.ToLower().Contains(search))
                                  || (x.Isbn != null && x.Isbn.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(sortField))
        {
            query = query.OrderBy($"{sortField} {(order > 0 ? "ascending" : "descending")}");
        }

        return query;
    }
}