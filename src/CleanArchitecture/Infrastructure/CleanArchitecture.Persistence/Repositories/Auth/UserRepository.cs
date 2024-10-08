using AutoMapper;
using CleanArchitecture.Application.Dtos.Auth.Users;
using CleanArchitecture.Application.Interfaces.Repositories.Auth;
using CleanArchitecture.Domain.Entities.Auths;
using CleanArchitecture.Persistence.Contexts;
using Contracts.Common.Models.Paging;
using Contracts.Exceptions;
using Infrastructure.Extensions.Collection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Auth;

public class UserRepository : IUserRepository
{
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(IMapper mapper, UserManager<AppUser> userManager, ApplicationDbContext dbContext)
    {
        _mapper = mapper;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), id);
        }

        var result = _mapper.Map<UserDto>(user);
        return result;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(GetAllUsersInput input)
    {
        var users = await _userManager.Users
            .Where(o=>!o.IsDeleted)
            .WhereIf(input != null && !string.IsNullOrWhiteSpace(input.Keyword),
                o => o.FirstName.Contains(input.Keyword)
                     || o.UserName.Contains(input.Keyword)
                     || o.Email.Contains(input.Keyword)
                     || o.PhoneNumber.Contains(input.Keyword)).ToListAsync();
        var result = _mapper.Map<List<UserDto>>(users);

        return result;
    }

    public async Task<PagedResult<UserDto>> GetAllUsersPagedAsync(GetAllUsersInput input)
    {
        var query = _userManager.Users
            .Where(o=>!o.IsDeleted)
            .WhereIf(input != null && !string.IsNullOrWhiteSpace(input.Keyword),
                o => o.FirstName.Contains(input.Keyword)
                     || o.UserName.Contains(input.Keyword)
                     || o.Email.Contains(input.Keyword)
                     || o.PhoneNumber.Contains(input.Keyword));

        var objQuery = _mapper.ProjectTo<UserDto>(query);

        var result = await PagedResult<UserDto>.ToPagedListAsync(objQuery, input.PageIndex, input.PageSize);
        return result;
    }

    public async Task CreateUserAsync(CreateUserDto input)
    {
        if (await _userManager.FindByNameAsync(input.UserName) != null)
        {
            throw new BadRequestException("Username already exists");
        }

        if (await _userManager.FindByEmailAsync(input.Email) != null)
        {
            throw new BadRequestException("Email already exists");
        }

        var user = _mapper.Map<AppUser>(input);
        user.LockoutEnabled = false;
        await _userManager.CreateAsync(user, input.Password);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(UpdateUserDto input)
    {
        if (input.Id == null)
        {
            throw new BadRequestException("Id is required");
        }

        var user = await _userManager.FindByIdAsync(input.Id.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), input.Id);
        }
        input.UserName = user.UserName;
        input.Email = user.Email;

        _mapper.Map(input, user);
        await _userManager.UpdateAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteUsersAsync(int[] ids)
    {
        foreach (var id in ids)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(AppUser), id);
            }

            await _userManager.DeleteAsync(user);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeMyPassWordAsync(ChangeMyPasswordRequest input, int currentUserId)
    {
        var user = await _userManager.FindByIdAsync(currentUserId.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), currentUserId);
        }

        await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetPasswordAsync(SetPasswordRequest input)
    {
        var user = await _userManager.FindByIdAsync(input.CurrentUserId.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), input.CurrentUserId);
        }

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, input.NewPassword);
        await _userManager.UpdateAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeEmailAsync(ChangeEmailRequest input)
    {
        var user = await _userManager.FindByIdAsync(input.CurrentUserId.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), input.CurrentUserId);
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, input.Email);
        await _userManager.ChangeEmailAsync(user, input.Email, token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AssignRolesToUserAsync(AssignRolesToUserRequest input)
    {
        var user = await _userManager.FindByIdAsync(input.CurrentUserId.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), input.CurrentUserId);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRolesAsync(user, input.Roles);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IList<string>> GetUserRolesAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.GetRolesAsync(user);
        return result;
    }
}