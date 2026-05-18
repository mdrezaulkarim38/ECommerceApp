using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAppApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<List<CategoryDto>>.Ok(categories));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<CategoryDto>.Error("Category not found"));
        return Ok(ApiResponse<CategoryDto>.Ok(category));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await _categoryService.CreateAsync(request);
        return Ok(ApiResponse<CategoryDto>.Ok(category, "Category created"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(id, request);
            return Ok(ApiResponse<CategoryDto>.Ok(category, "Category updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CategoryDto>.Error(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        try
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<string>.Error("Category not found"));
            return Ok(ApiResponse<string>.Ok("", "Category deleted"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }
}
