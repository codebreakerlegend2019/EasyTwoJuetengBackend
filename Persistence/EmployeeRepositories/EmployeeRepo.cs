using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.EmployeeDto;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.EmployeeRepositories
{
    public class EmployeeRepo : ICrudPattern<Employee>, IValidator<Employee, EmployeeSaveDto>
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly IMapper _mapper;

        public EmployeeRepo(EasyTwoJuetengContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Create(Employee model)
        {
            model.DateTimeCreated = DateTime.Now;
            _context.Add(model);
        }

        public void Delete(Employee model)
        {
            _context.Remove(model);
        }

        public async Task<Employee> Get(int id)
        {
            return await _context.Employees
                                 .Include(x => x.WorkLocation)
                                    .ThenInclude(x => x.City)
                                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees
                                   .Include(x => x.WorkLocation)
                                      .ThenInclude(x => x.City)
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(object newUpdate, Employee modelToBeUpdated)
        {
            _mapper.Map(newUpdate, modelToBeUpdated);
        }


        private async Task<bool> isEmployeeNameAlreadyExisted(EmployeeSaveDto resourceSave)
        {
            return await _context.Employees
                                 .AnyAsync(x => x.FirstName.ToLower() == resourceSave.FirstName.ToLower()
                                          && x.MiddleName.ToLower() == resourceSave.MiddleName.ToLower()
                                          && x.LastName.ToLower() == resourceSave.LastName.ToLower());
        }
        private async Task<bool> isEmailAddressTaken(string emailAddress)
        {
            return await _context.Employees
                              .AnyAsync(x => x.EmailAddress.ToLower() == emailAddress.ToLower());
        }
        private async Task<bool> isContactNumberTaken(string contactNumber)
        {
            return await _context.Employees
                              .AnyAsync(x => x.ContactNumber.ToLower() == contactNumber.ToLower());
        }
        public async Task<bool> isWorkLocationExisted(int workLocatioId)
        {
            return await _context.WorkLocations.AnyAsync(x => x.Id == workLocatioId);
        }
        public async Task<ErrorValidator> Validate(EmployeeSaveDto resourceSave)
        {
            if (await isEmployeeNameAlreadyExisted(resourceSave))
                return new ErrorValidator("Employee Name Already Existed");
            if (await isEmailAddressTaken(resourceSave.EmailAddress))
                return new ErrorValidator("Email Already Taken");
            if (await isContactNumberTaken(resourceSave.ContactNumber))
                return new ErrorValidator("Contact Number Already Existed");
            if (!await isWorkLocationExisted(resourceSave.WorkLocationId))
                return new ErrorValidator("Work Location Selected Doesn't Existed");
            return new ErrorValidator();
        }

        public async Task<ErrorValidator> Validate(Employee entity, EmployeeSaveDto resourceSave)
        {
            if (entity.FirstName != resourceSave.FirstName ||
                entity.MiddleName != resourceSave.MiddleName ||
                entity.LastName != resourceSave.LastName)
                if (await isEmployeeNameAlreadyExisted(resourceSave))
                    return new ErrorValidator("Employee Name Already Existed");
            if (entity.EmailAddress != resourceSave.EmailAddress)
                if (await isEmailAddressTaken(resourceSave.EmailAddress))
                    return new ErrorValidator("Email Already Taken");
            if (entity.ContactNumber != resourceSave.ContactNumber)
                if (await isContactNumberTaken(resourceSave.ContactNumber))
                    return new ErrorValidator("Contact Number Already Existed");
            if(entity.WorkLocationId != resourceSave.WorkLocationId)
                if (!await isWorkLocationExisted(resourceSave.WorkLocationId))
                    return new ErrorValidator("Work Location Selected Doesn't Existed");
            return new ErrorValidator();
        }
    }
}
