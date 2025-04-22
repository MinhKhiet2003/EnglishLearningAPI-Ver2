using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;
using EnglishLearningAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly IJwtTokenUtil _jwtTokenUtil;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly IUserRepository _userRepository;

    public UserService(IJwtTokenUtil jwtTokenUtil, IPasswordHasherService passwordHasher, ITokenBlacklistService tokenBlacklistService, IUserRepository userRepository)
    {

        _jwtTokenUtil = jwtTokenUtil;
        _passwordHasher = passwordHasher;
        _tokenBlacklistService = tokenBlacklistService;
        _userRepository = userRepository;
    }

    public async Task<User> CreateUserAsync(UserDTO userDTO)
    {
        var user = new User
        {
            Email = userDTO.Email,
            Password = userDTO.Password,
            Salt=userDTO.Salt,
            FullName = userDTO.FullName,
            SubscriptionPlan = userDTO.SubscriptionPlan,
            Role = userDTO.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (userDTO.SubscriptionPlan != "none")
        {
            user.SubscriptionStartDate = DateTime.UtcNow;
            user.SubscriptionEndDate = userDTO.SubscriptionPlan switch
            {
                "6_months" => DateTime.UtcNow.AddMonths(6),
                "1_year" => DateTime.UtcNow.AddYears(1),
                _ => DateTime.UtcNow.AddYears(3)
            };
        }

        await _userRepository.CreateUserAsync(user);
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _userRepository.GetUserByIdAsync(id) ?? throw new Exception($"User with id {id} not found");
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null)
        {
            throw new UserNotFoundException($"User with email {email} not found");
        }
        return user;
    }

    // Tạo một Exception cụ thể
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
    }

    public async Task<User> UpdateUserAsync(int id, UserDTO userDTO)
    {
        var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception($"User with id {id} not found");

        user.FullName = userDTO.FullName;
        user.Role = userDTO.Role;
        user.UpdatedAt = DateTime.UtcNow;

        if (user.SubscriptionPlan != userDTO.SubscriptionPlan)
        {
            user.SubscriptionPlan = userDTO.SubscriptionPlan;
            if (user.SubscriptionEndDate == null || user.SubscriptionEndDate <= DateTime.UtcNow)
            {
                user.SubscriptionStartDate = DateTime.UtcNow;
                user.SubscriptionEndDate = userDTO.SubscriptionPlan switch
                {
                    "6_months" => DateTime.UtcNow.AddMonths(6),
                    "1_year" => DateTime.UtcNow.AddYears(1),
                    _ => DateTime.UtcNow.AddYears(3)
                };
            }
            else
            {
                throw new Exception("User still has an active subscription");
            }
        }

        await _userRepository.UpdateUserAsync(user);
        return user;
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception($"User with id {id} not found");
        if (user.Role == "ROLE_ADMIN")
        {
            throw new Exception("Cannot delete admin user");
        }

        await _userRepository.DeleteUserAsync(id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userRepository.ExistsByEmailAsync(email);
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        
        if(user == null)
        {
            throw new Exception("Thông tin đăng nhập không chính xác");
        }

        var checkPass = _passwordHasher.VerifyPassword(password, user.Password);
        
        if (!checkPass)
        {
            throw new Exception("Thông tin đăng nhập không chính xác");
        }

        return _jwtTokenUtil.GenerateToken(user);
    }


    public async Task<User> RegisterAsync(UserDTO userDTO)
    {
        if (await ExistsByEmailAsync(userDTO.Email))
        {
            throw new Exception("Email already exists");
        }
        var user = new User
        {
            Email = userDTO.Email,
            Password = userDTO.Password,
            Salt = userDTO.Salt,
            FullName = userDTO.FullName,
            SubscriptionPlan = "none",
            Role = "ROLE_USER",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateUserAsync(user);
        return user;
    }

    public async Task LogoutAsync(string token)
    {
        _tokenBlacklistService.BlacklistToken(token);
    }

    public async Task<string> RefreshTokenAsync(string token)
    {
        var email = _jwtTokenUtil.GetEmailFromToken(token);
        var user = await _userRepository.FindByEmailAsync(email) ?? throw new Exception("Unable to refresh token");

        _tokenBlacklistService.BlacklistToken(token);
        return _jwtTokenUtil.GenerateToken(user);
    }

    public async Task<List<string>> CheckExistingEmailsAsync(List<UserDTO> userDTOList)
    {
        var emails = userDTOList.Select(dto => dto.Email).ToList();
        var existingEmails = await _userRepository.GetAllUsersAsync();
        return existingEmails.Where(u => emails.Contains(u.Email)).Select(u => u.Email).ToList();
    }

    public async Task SaveAllUsersAsync(List<UserDTO> userDTOList)
    {
        var users = userDTOList.Select(dto => new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Password = _passwordHasher.HashPassword(dto.Password),
            Salt=dto.Salt,
            SubscriptionPlan = "none",
            Role = "ROLE_USER",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        foreach (var user in users)
        {
            await _userRepository.CreateUserAsync(user);
        }
    }

    public async Task<List<User>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end)
    {
        return await _userRepository.FindUsersCreatedBetweenAsync(start, end);
    }

    public async Task<Dictionary<string, long>> GetUserSegmentsAsync()
    {
        var results = await _userRepository.FindSubscriptionPlanCountsAsync();
        return results.ToDictionary(result => result.SubscriptionPlan, result => result.Count);
    }

    public async Task<Dictionary<int, long>> CountUsersByMonthCurrentYearAsync()
    {
        var results = await _userRepository.CountUsersByMonthCurrentYearAsync();
        var monthUserCount = results.ToDictionary(result => result.Month, result => result.TotalUsers);

        for (int month = 1; month <= 12; month++)
        {
            if (!monthUserCount.ContainsKey(month))
            {
                monthUserCount[month] = 0;
            }
        }

        return monthUserCount;
    }

    public async Task<List<User>> GetUsersWithExpiringSubscriptionsAsync()
    {
        return await _userRepository.FindUsersWithExpiringSubscriptionsAsync();
    }

    public async Task<User> ChangePasswordAsync(User user)
    {
        user.Password = _passwordHasher.HashPassword(user.Password);
        await _userRepository.UpdateUserAsync(user);
        return user;
    }

    public async Task DeleteSubscriptionAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception($"User with id {id} not found");
        user.SubscriptionPlan = "none";
        user.SubscriptionEndDate = DateTime.UtcNow;
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task<User> FetchAsync(string token)
    {
        var email = _jwtTokenUtil.GetEmailFromToken(token);
        return await _userRepository.FindByEmailAsync(email) ?? throw new Exception("User not found");
    }

    public async Task<User> PaymentAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateUserAsync(user);
        return user;
    }
    public async Task<PagedResult<User>> GetUsersAsync(int page, int size, string email, string fullName, string role, string subscriptionPlan, string sort)
    {
        var query = (await _userRepository.GetAllUsersAsync()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(u => u.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            query = query.Where(u => u.FullName.Contains(fullName));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role.Contains(role));
        }

        if (!string.IsNullOrWhiteSpace(subscriptionPlan))
        {
            query = query.Where(u => u.SubscriptionPlan.Contains(subscriptionPlan));
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            switch (sort)
            {
                case "email":
                    query = query.OrderBy(u => u.Email);
                    break;
                case "-email":
                    query = query.OrderByDescending(u => u.Email);
                    break;
                case "fullName":
                    query = query.OrderBy(u => u.FullName);
                    break;
                case "-fullName":
                    query = query.OrderByDescending(u => u.FullName);
                    break;
                case "updatedAt":
                    query = query.OrderBy(u => u.UpdatedAt);
                    break;
                case "-updatedAt":
                    query = query.OrderByDescending(u => u.UpdatedAt);
                    break;
                case "subscriptionStartDate":
                    query = query.Where(u => u.SubscriptionStartDate != null).OrderBy(u => u.SubscriptionStartDate);
                    break;
                case "-subscriptionStartDate":
                    query = query.Where(u => u.SubscriptionStartDate != null).OrderByDescending(u => u.SubscriptionStartDate);
                    break;
                case "subscriptionEndDate":
                    query = query.Where(u => u.SubscriptionEndDate != null).OrderBy(u => u.SubscriptionEndDate);
                    break;
                case "-subscriptionEndDate":
                    query = query.Where(u => u.SubscriptionEndDate != null).OrderByDescending(u => u.SubscriptionEndDate);
                    break;
                default:
                    break;
            }
        }

        var pagedResult = await PagedResult<User>.CreateAsync(query.ToList(), page, size);
        return pagedResult;
    }

}
