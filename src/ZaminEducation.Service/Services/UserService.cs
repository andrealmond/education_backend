﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZaminEducation.Data.IRepositories;
using ZaminEducation.Domain.Configurations;
using ZaminEducation.Domain.Entities.Users;
using ZaminEducation.Domain.Enums;
using ZaminEducation.Service.DTOs.Commons;
using ZaminEducation.Service.DTOs.Users;
using ZaminEducation.Service.Exceptions;
using ZaminEducation.Service.Extensions;
using ZaminEducation.Service.Helpers;
using ZaminEducation.Service.Interfaces;

namespace ZaminEducation.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> userRepository;
        private readonly IMapper mapper;
        private readonly IAttachmentService attachmentService;
        public UserService(IRepository<User> userRepository, IMapper mapper, IAttachmentService attachmentService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.attachmentService = attachmentService;
        }

        public async ValueTask<User> CreateAsync(UserForCreationDto dto)
        {
            var user = await userRepository.GetAsync(u => u.Username == dto.Username
                                                            && u.State != ItemState.Deleted);

            if (user is not null)
                throw new ZaminEducationException(400, "User already exists");

            User mappedUser = mapper.Map<User>(dto);

            mappedUser.Password = dto.Password.Encrypt();
            mappedUser.Create();

            User newUser = await userRepository.AddAsync(mappedUser);

            await userRepository.SaveChangesAsync();

            return newUser;
        }

        public async ValueTask<bool> DeleteAsync(Expression<Func<User, bool>> expression)
        {
            var user = await userRepository.GetAsync(expression);

            if (user is null || user.State == ItemState.Deleted)
                throw new ZaminEducationException(404, "User not found");

            userRepository.Delete(user);

            await userRepository.SaveChangesAsync();

            return true;
        }

        public async ValueTask<IEnumerable<User>> GetAllAsync(PaginationParams @params, Expression<Func<User, bool>> expression = null, string search = null)
        {
            var users = userRepository.GetAll(expression, new string[] { "Address", "Image" }, isTracking: false);

            return !string.IsNullOrEmpty(search)
                ? await users.Where(u => u.FirstName == search ||
                        u.LastName == search ||
                        u.Username == search ||
                        u.Bio.Contains(search)).ToPagedList(@params).ToListAsync()
                : (IEnumerable<User>)await users.ToPagedList(@params).ToListAsync();
        }

        public async ValueTask<User> GetAsync(Expression<Func<User, bool>> expression)
        {
            var user = await userRepository.GetAsync(expression, new string[] { "Address", "Image" });

            if (user is null)
                throw new ZaminEducationException(404, "User not found");

            return user;
        }

        public async ValueTask<User> UpdateAsync(long id, UserForUpdateDto dto)
        {
            var user = await userRepository.GetAsync(u => u.Id == id && u.State != ItemState.Deleted);

            if (user is null)
                throw new ZaminEducationException(404, "User not found!");

            var alredyExistsUser = await userRepository.GetAsync(u => u.Username == dto.Username
                                                                    && u.State != ItemState.Deleted && u.Id != id);

            if (alredyExistsUser is not null)
                throw new ZaminEducationException(400, "Login or Password is incorrect!");

            user = mapper.Map(dto, user);

            user.Password = user.Password.Encrypt();
            user.Update();

            userRepository.Update(user);

            await userRepository.SaveChangesAsync();

            return user;
        }

        public async ValueTask<User> GetInfoAsync()
            => await userRepository.GetAsync(u => u.Id == HttpContextHelper.UserId);

        public async ValueTask<User> AddAttachmentAsync(long userId, AttachmentForCreationDto attachmentForCreationDto)
        {
            var attachment = await attachmentService.UploadAsync(attachmentForCreationDto);

            var user = await userRepository.GetAsync(u => u.Id == userId);

            if (user == null)
                throw new ZaminEducationException(404, "User not found");

            user.ImageId = attachment.Id;

            userRepository.Update(user);
            await userRepository.SaveChangesAsync();

            return user;
        }

        public async ValueTask<User> ChangePasswordAsync(UserForChangePassword dto)
        {
            User existUser = await userRepository.GetAsync(user => user.Username == dto.Username);

            if (existUser is null)
                throw new Exception("This Username is not exists");

            else if (dto.NewPassword != dto.ComfirmPassword)
                throw new Exception("New password and comfirm password is not equal");


            else if (existUser.Password != dto.OldPassword.Encrypt())
                throw new Exception("Password is incorrect!");

            existUser.Password = dto.NewPassword.Encrypt();
            await userRepository.SaveChangesAsync();

            return existUser;
        }
    }
}
