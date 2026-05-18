using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<bool> DeleteAsync(int id);
}
