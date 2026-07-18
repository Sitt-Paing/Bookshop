using Bookshop.Entities;
using Bookshop.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bookshop.Controller;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(IRepositoryWrapper repo) : ControllerBase
{
    private readonly IRepositoryWrapper _repo = repo ?? throw new Exception("Repo is null");

    [HttpGet]
    [EndpointSummary("Get Category List")]
    public async Task<ActionResult> GetAsync()
    {
        var data = await _repo.Categories.GetAsync(x => !x.DeletedOn.HasValue);
        return Ok(data);    
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get Category by Id")]
    public async Task<ActionResult<Category>> GetAsync(int id) 
    {
        var data = await _repo.Categories.GetByIdAsync(id);
        if (data == null) return NotFound(); 
        return Ok(data);
    }

    [HttpPost]
    [EndpointSummary("Create Category")]
    public async Task<ActionResult> CreateAsync([FromBody] Category model)
    {
        try
        {
            Category data = new Category()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now
            };
            _repo.Categories.Create(data);
        
            return await _repo.SaveAsync() ? Ok(data) : BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message); 
        }
    }
}