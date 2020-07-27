using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.UserDto;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.UserRepositories
{
    public class UserRepo:ICrudPattern<User>,
        IValidator<User,UserSaveDto>,
        IValidator<User,UserUpdateDto>
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly IMapper _mapper;
        private const string _defaultPassword = "Agent12345!";

        public UserRepo(EasyTwoJuetengContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Create(User model)
        {
            InputHelper.Trimmer(model);
            model.Password = AES.Encrypt("_defaultPassword");
            model.DateTimeCreated = DateTime.Now;
            _context.Add(model);
        }

        public void Delete(User model)
        {
            _context.Remove(model);
        }

        public async Task<User> Get(int id)
        {
            return await _context.Users
                                 .Include(x => x.UserRole)
                                 .Include(x => x.Employee)
                                 .Include(x => x.Employee.WorkLocation)
                                 .Include(x => x.Employee.WorkLocation.City)
                                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users
                                  .Include(x => x.UserRole)
                                  .Include(x => x.Employee)
                                  .Include(x => x.Employee.WorkLocation)
                                  .Include(x => x.Employee.WorkLocation.City)
                                  .AsNoTracking()
                                  .ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(object newUpdate, User modelToBeUpdated)
        {
            InputHelper.Trimmer(newUpdate);
            _mapper.Map(newUpdate, modelToBeUpdated);
        }
        
        private async Task<bool> isEmployeeAlreadyExisted(int employeeId)
        {
            return await _context.Users.AnyAsync(x => x.EmployeeId == employeeId);
        }
        private async Task<bool> isUserNameTaken(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
        private  bool isUserHasWhiteSpaces(string userName)
        {
            return String.IsNullOrWhiteSpace(userName);
        }
        public async Task<ErrorValidator> Validate(UserSaveDto resourceSave)
        {
            if (await isEmployeeAlreadyExisted(resourceSave.EmployeeId))
                return new ErrorValidator("Employee Selected Already has an account");
            if (await isUserNameTaken(resourceSave.UserName))
                return new ErrorValidator("Username Already Taken");
            if (isUserHasWhiteSpaces(resourceSave.UserName))
                return new ErrorValidator("Username Invalid please remove spaces");
            return new ErrorValidator();
        }

        public Task<ErrorValidator> Validate(User entity, UserSaveDto resourceSave)
        {
            throw new NotImplementedException();
        }

        public Task<ErrorValidator> Validate(UserUpdateDto resourceSave)
        {
            throw new NotImplementedException();
        }

        public async Task<ErrorValidator> Validate(User entity, UserUpdateDto resourceSave)
        {
            if (entity.UserName != resourceSave.UserName)
            {
                if (await isUserNameTaken(resourceSave.UserName))
                    return new ErrorValidator("Username Already Taken");
                if (isUserHasWhiteSpaces(resourceSave.UserName))
                    return new ErrorValidator("Username Invalid please remove spaces");
            }
            return new ErrorValidator();
        }
    }
}
