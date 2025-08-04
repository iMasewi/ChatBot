using AutoMapper;
using Chat.DTOS;
using Chat.Models;
using Chat.Repository.Interfaces;
using Chat.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BotChat.Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UserService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> manager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = manager;
        }
        public async Task<UserDTO> AddUserAsync(UserDTO userDTO)
        {
            try
            {
                var user = _mapper.Map<User>(userDTO);
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(userDTO), "User data cannot be null.");
                }
                user.UserName = userDTO.Email;
                var createUser = await _userManager.CreateAsync(user, userDTO.PasswordHash);
                if (!createUser.Succeeded)
                {
                    throw new InvalidOperationException("User creation failed. Please check the provided data.");
                }

                var userRole = await _userManager.AddToRoleAsync(user, "User");
                if (!userRole.Succeeded)
                {
                    throw new Exception("Failed to add user to role: ");
                }

                return userDTO;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while adding the user.", ex);
            }
        }
        public async Task<bool> CheckDuplicateUserAsync(string email)
        {
            return await _unitOfWork.UserRepository.CheckDuplicateUserAsync(email);
        }

        public async Task<bool> CheckDuplicateUserAsync(string email, int id)
        {
            return await _unitOfWork.UserRepository.CheckDuplicateUserAsync(email, id);
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetById(id);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                }
                await _unitOfWork.UserRepository.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllUSersAsync()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetAllAsync();
                if (users == null || !users.Any())
                {
                    throw new KeyNotFoundException("No users found.");
                }
                var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);
                return userDTOs;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving all users.", ex);
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetById(id);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                }
                var userDTO = _mapper.Map<UserDTO>(user);
                return userDTO;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while retrieving the user with ID {id}.", ex);
            }
        }

        public async Task UpdateUserAsync(UserDTO userDTO, int id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetById(id);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                }
                MapUserData(user, userDTO);
                var updateUser = await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while updating the user with ID {id}.", ex);
            }
        }
        private void MapUserData(User user, UserDTO userDTO)
        {
            if( userDTO.Name != null && userDTO.Name != "") user.Name = userDTO.Name;
            if (userDTO.Email != null && userDTO.Email != "") user.Email = userDTO.Email;
        }
    }
}