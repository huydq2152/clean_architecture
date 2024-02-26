﻿using CleanArchitecture.Application.Dtos.Auth.Users;
using Infrastructure.Common.Models.Paging;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.Interfaces.Services.Auth.User;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<PagedResult<UserDto>> GetAllUsersPagedAsync(UserPagingQueryInput input);
    Task CreateUserAsync(CreateUserDto input);
    Task UpdateUserAsync(UpdateUserDto input);
    Task DeleteUserAsync(int[] ids);
    Task ChangeMyPassWordAsync(ChangeMyPasswordRequest input, int currentUserId);
    Task SetPasswordAsync(SetPasswordRequest input);
    Task ChangeEmailAsync(ChangeEmailRequest input);
    Task AssignRolesToUserAsync(AssignRolesToUserRequest input);
    Task<IList<string>> GetUserRolesAsync(int userId);
}