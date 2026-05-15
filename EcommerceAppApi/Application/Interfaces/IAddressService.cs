using EcommerceAppApi.Application.DTOs;

namespace EcommerceAppApi.Application.Interfaces;

public interface IAddressService
{
    Task<List<AddressDto>> GetUserAddressesAsync(int userId);
    Task<AddressDto> AddAddressAsync(int userId, AddressDto request);
    Task<AddressDto> UpdateAddressAsync(int userId, int addressId, AddressDto request);
    Task<bool> DeleteAddressAsync(int userId, int addressId);
    Task SetDefaultAddressAsync(int userId, int addressId);
}
