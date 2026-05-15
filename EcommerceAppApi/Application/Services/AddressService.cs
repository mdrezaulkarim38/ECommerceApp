using EcommerceAppApi.Application.DTOs;
using EcommerceAppApi.Application.Interfaces;
using EcommerceAppApi.Domain.Entities;
using EcommerceAppApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAppApi.Application.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;
    public AddressService(ApplicationDbContext context) => _context = context;

    public async Task<List<AddressDto>> GetUserAddressesAsync(int userId)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                FullName = a.FullName,
                Street = a.Street,
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
                Country = a.Country,
                PhoneNumber = a.PhoneNumber,
                IsDefault = a.IsDefault
            })
            .ToListAsync();
    }

    public async Task<AddressDto> AddAddressAsync(int userId, AddressDto request)
    {
        var hasAddresses = await _context.Addresses.AnyAsync(a => a.UserId == userId);

        var address = new Address
        {
            UserId = userId,
            FullName = request.FullName,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            IsDefault = request.IsDefault || !hasAddresses
        };

        if (address.IsDefault)
        {
            var currentDefault = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
            if (currentDefault != null) currentDefault.IsDefault = false;
        }

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return new AddressDto
        {
            Id = address.Id,
            FullName = address.FullName,
            Street = address.Street,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault
        };
    }

    public async Task<AddressDto> UpdateAddressAsync(int userId, int addressId, AddressDto request)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null) throw new KeyNotFoundException("Address not found");

        address.FullName = request.FullName;
        address.Street = request.Street;
        address.City = request.City;
        address.State = request.State;
        address.ZipCode = request.ZipCode;
        address.Country = request.Country;
        address.PhoneNumber = request.PhoneNumber;

        if (request.IsDefault && !address.IsDefault)
        {
            var currentDefault = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault && a.Id != addressId);
            if (currentDefault != null) currentDefault.IsDefault = false;
            address.IsDefault = true;
        }

        await _context.SaveChangesAsync();

        return new AddressDto
        {
            Id = address.Id,
            FullName = address.FullName,
            Street = address.Street,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country,
            PhoneNumber = address.PhoneNumber,
            IsDefault = address.IsDefault
        };
    }

    public async Task<bool> DeleteAddressAsync(int userId, int addressId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null) return false;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SetDefaultAddressAsync(int userId, int addressId)
    {
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

        foreach (var addr in addresses)
            addr.IsDefault = addr.Id == addressId;

        await _context.SaveChangesAsync();
    }
}
