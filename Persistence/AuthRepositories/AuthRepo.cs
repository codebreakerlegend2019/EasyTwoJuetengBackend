using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.AuthDto;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.AuthRepositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly ICrudPattern<User> _userRepo;
        private readonly IMapper _mapper;

        public AuthRepo(EasyTwoJuetengContext context,
            ICrudPattern<User> userRepo,
            IMapper mapper)
        {
            _context = context;
            _userRepo = userRepo;
            _mapper = mapper;
        }
        public async Task<LoginDetailsReadDto> LoginChecker(LoginCredentialsSaveDto model)
        {
            var user = await AuthEasyTwo(model);
            if (user == null)
                return new LoginDetailsReadDto()
                {
                    IsSuccess = false
                };
            return new LoginDetailsReadDto()
            {
                Nickname = user.Employee.NickName,
                IsSuccess = true,
                Role = user.UserRole.Name,
                UserName = user.UserName,
                UserFullName = $"{user.Employee.FirstName} {user.Employee.LastName}",
                UserId = user.Id,
            };

        }
        public async Task<ErrorValidator> ChangePassword(int userId, ChangePasswordSaveDto resourceSave)
        {
            if (resourceSave.NewPassword != resourceSave.ConfirmNewPassword)
                return new ErrorValidator("Password Mismatch");
            var user = await _userRepo.Get(userId);
            if (user == null)
                return new ErrorValidator("User Doesn't Exist");
            if (user.Password == AES.Encrypt(resourceSave.NewPassword))
                return new ErrorValidator("Use Different Password, Detected that it is same as your current password!");
            _mapper.Map(resourceSave,user);
            if (!await _userRepo.SaveChanges())
                return new ErrorValidator("Password Fail to Change!!");
            return new  ErrorValidator();
        }
        private async Task<User> AuthEasyTwo(LoginCredentialsSaveDto model)
        {
            var encrptyedPassword = AES.Encrypt(model.Password);
            return await _context.Users
                .Include(x => x.UserRole)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync
                (x => x.UserName.ToLower().Trim() == model.Username.Trim().ToLower()
                && x.Password == encrptyedPassword);
        }
    }

}
