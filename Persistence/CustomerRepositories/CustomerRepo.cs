using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.CustomerDto;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.CustomerRepositories
{
    public class CustomerRepo : ICrudPattern<Customer>, IValidator<Customer, CustomerSaveDto>
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly IMapper _mapper;

        public CustomerRepo(EasyTwoJuetengContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void Create(Customer model)
        {
            InputHelper.Trimmer(model);
            model.CellphoneNumber = $"+63{model.CellphoneNumber}";
            model.DateTimeCreated = DateTime.Now;
            _context.Add(model);
        }

        public void Delete(Customer model)
        {
            _context.Remove(model);
        }

        public async Task<Customer> Get(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<List<Customer>> GetAll()
        {
            return await _context.Customers
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(object newUpdate, Customer modelToBeUpdated)
        {
            InputHelper.Trimmer(newUpdate);
            _mapper.Map(newUpdate, modelToBeUpdated);
        }

        public async Task<bool> isCustomerAlreadyExisted(CustomerSaveDto customerSave)
        {
            return await _context.Customers
                                 .AnyAsync(x => x.FirstName.ToLower() == customerSave.FirstName.ToLower() &&
                                           x.MiddleName.ToLower() == customerSave.MiddleName.ToLower() &&
                                           x.LastName.ToLower() == customerSave.LastName.ToLower());
        }
        public async Task<bool> isContactNumberAlreadyExisted(string cellNum)
        {
            return await _context.Customers.AnyAsync(x => x.CellphoneNumber == $"+63{cellNum}");
        }
        public bool isValidEmailAddress(string emailAddress)
        {
            return emailAddress.Contains('@');
        }
        public async Task<bool> isEmailAddressAlreadyExisted(string emailAddress)
        {
            return await _context.Customers.AnyAsync(x => x.EmailAddress.ToLower() == emailAddress);
        }
        public async Task<ErrorValidator> Validate(CustomerSaveDto resourceSave)
        {
            if (await isCustomerAlreadyExisted(resourceSave))
                return new ErrorValidator("Customer Already Exist!");
            if (await isContactNumberAlreadyExisted(resourceSave.CellphoneNumber))
                return new ErrorValidator("Contact Number Already Existed");
            if (!isValidEmailAddress(resourceSave.EmailAddress))
                return new ErrorValidator("Invalid Email Address");
            if (await isEmailAddressAlreadyExisted(resourceSave.EmailAddress))
                return new ErrorValidator("Email Address Already Existed");
            return new ErrorValidator();
        }

        public async Task<ErrorValidator> Validate(Customer entity, CustomerSaveDto resourceSave)
        {
            if(entity.FirstName!= resourceSave.FirstName ||
                entity.MiddleName!=resourceSave.MiddleName ||
                entity.LastName != resourceSave.LastName)
                if (await isCustomerAlreadyExisted(resourceSave))
                    return new ErrorValidator("Customer Already Exist!");
            if(entity.CellphoneNumber!= resourceSave.CellphoneNumber)
                if (await isContactNumberAlreadyExisted(resourceSave.CellphoneNumber))
                    return new ErrorValidator("Contact Number Already Existed");
            if (entity.EmailAddress != resourceSave.EmailAddress)
            {
                if (!isValidEmailAddress(resourceSave.EmailAddress))
                    return new ErrorValidator("Invalid Email Address");
                if (await isEmailAddressAlreadyExisted(resourceSave.EmailAddress))
                    return new ErrorValidator("Email Address Already Existed");
            }
            return new ErrorValidator();
        }
    }
}
