using Bookshop.Entities;
using Bookshop.Interfaces.Repositories;
using Bookshop.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookshop.Controller;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(IRepositoryWrapper repo) : ControllerBase
{
    private readonly IRepositoryWrapper _repo = repo ?? throw new Exception("Repo is null");

    [HttpGet]
    [EndpointSummary("Get Category List")]
    public async Task<IActionResult> GetAsync()
    {
        var data = await _repo.Categories.GetAsync(x => !x.DeletedOn.HasValue);
        return Ok(new DefaultResponseModel
        {
            Success = true,
            Statuscode = 200,
            Message = "Success",
            Data = data
        });    
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get Category by Id")]
    public async Task<IActionResult> GetAsync(int id) 
    {
        var data = await _repo.Categories.GetByIdAsync(id);
        if (data == null)
        {
            return NotFound(new DefaultResponseModel
            {
                Success = false,
                Statuscode = 404,
                Message = "Category not found",
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
    [EndpointSummary("Create Category")]
    public async Task<IActionResult> CreateAsync([FromBody] Category model)
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
        
            return await _repo.SaveAsync() 
                ? Ok(new DefaultResponseModel
                {
                    Success = true,
                    Statuscode = 200,
                    Message = "Category created successfully",
                    Data = data
                }) 
                : BadRequest(new DefaultResponseModel
                {
                    Success = false,
                    Statuscode = 400,
                    Message = "Failed to create category",
                    Data = null
                });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new DefaultResponseModel
            {
                Success = false,
                Statuscode = 500,
                Message = ex.Message,
                Data = null
            }); 
        }
    }
}