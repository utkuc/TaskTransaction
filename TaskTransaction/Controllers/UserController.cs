using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskTransaction.Models.User;
using TaskTransaction.Models.User.Dto;
using TaskTransaction.Services;

namespace TaskTransaction.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ILogger<UserController> logger, UserService userService, IMapper autoMapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        logger.LogInformation("Fetched all users, Fetch Count: {UserCount}", users.Count());
        return Ok(autoMapper.Map<List<UserDto>>(users));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User with id {Id} not found to fetch", userId);
            return StatusCode(404);
        }

        logger.LogInformation("Fetched user with ID: {Id}", user.UserID);
        return Ok(autoMapper.Map<UserDto>(user));
    }

    [HttpPost("createUser")]
    public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
    {
        if (createUserDto == null || string.IsNullOrWhiteSpace(createUserDto.UserID))
        {
            logger.LogWarning("CreateUser request body is not valid.");
            return StatusCode(400, "No user has been provided.");
        }

        var existingUser = await userService.GetUserByIdAsync(createUserDto.UserID);
        if (existingUser != null)
        {
            logger.LogWarning("User with ID {Id} already exists", createUserDto.UserID);
            return StatusCode(403, "User already exists");
        }

        User newUser;
        try
        {
            newUser = autoMapper.Map<User>(createUserDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Automapper could not map userDto to user");
            return StatusCode(500, e.Message);
        }

        await userService.CreateUserAsync(newUser);

        var createdUserDto = autoMapper.Map<UserDto>(newUser);

        logger.LogInformation("User with Id {UserId} created", newUser.UserID);
        return StatusCode(201, createdUserDto);
    }

    [HttpPost("updateUserId")]
    public async Task<IActionResult> UpdateUser(UpdateUserIdDto updateUserIdDto)
    {
        if (updateUserIdDto == null || string.IsNullOrEmpty(updateUserIdDto.PreviousUserID) ||
            string.IsNullOrEmpty(updateUserIdDto.NewUserID))
        {
            logger.LogWarning("UpdateUser request body is not valid");
            return StatusCode(400, "Invalid user data.");
        }

        var updatedUser =
            await userService.UpdateUserIdAsync(updateUserIdDto.PreviousUserID, updateUserIdDto.NewUserID);

        if (updatedUser == null)
        {
            logger.LogWarning("User Id Update failed, ids are not valid");
            return StatusCode(400);
        }

        var updatedUserDto = autoMapper.Map<UserDto>(updatedUser);

        logger.LogInformation("User with ID {Id} updated.", updatedUser.UserID);
        return StatusCode(200, updatedUserDto);
    }

    [HttpPost("deleteUserById")]
    public async Task<IActionResult> DeleteUser(DeleteUserDto deleteUserDto)
    {
        if (deleteUserDto == null || string.IsNullOrEmpty(deleteUserDto.UserID))
        {
            logger.LogWarning("DeleteUser request body is not valid");
            return StatusCode(400, "Invalid user data.");
        }

        var deleteSuccess = await userService.DeleteUserAsync(deleteUserDto.UserID);

        if (!deleteSuccess)
        {
            logger.LogWarning("User with Id {Id} not found", deleteUserDto.UserID);
            return StatusCode(400);
        }

        return StatusCode(200);
    }

    [HttpGet("getTotalTransactionAmountForUser/{userId}")]
    public async Task<IActionResult> GetTransactionById(string userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User with id {Id} not found to fetch", userId);
            return StatusCode(404);
        }

        var totalAmount = await userService.GetTotalTransactionAmountAsync(userId);
        return StatusCode(200, totalAmount);
    }
}