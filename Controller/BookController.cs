using Bookshop.Entities;
using Bookshop.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bookshop.Controller;

[Route("api/[controller]")]
[ApiController]
public class BookController(IRepositoryWrapper repo) : ControllerBase
{
    private readonly IRepositoryWrapper _repo = repo ?? throw new Exception("Repo is null");

    [HttpGet]
    [EndpointSummary("Get Book List")]
    public async Task<IActionResult> GetAsync()
    {
        IReadOnlyList<Book>? data = await _repo.Books.GetAsync(x => !x.DeletedOn.HasValue);
        return Ok(data);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get Book by Id")]
    public async Task<IActionResult> GetAsync(int id)
    {
        IReadOnlyList<Book>? data = await _repo.Books.GetAsync(x => x.Id == id);
        return Ok(data);
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
            ? Ok(data)
            : BadRequest(); 
    }
}