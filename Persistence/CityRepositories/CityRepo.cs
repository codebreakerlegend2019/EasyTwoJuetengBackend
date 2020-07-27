using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.CityDto;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using EasyTwoJuetengBackend.Helpers;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.CityRepositories
{
    public class CityRepo:ICrudPattern<City>,IValidator<City,CitySaveDto>
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly IMapper _mapper;

        public CityRepo(EasyTwoJuetengContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Create(City model)
        {
            InputHelper.Trimmer(model);
            _context.Add(model);
        }

        public void Delete(City model)
        {
            _context.Remove(model);
        }

        public async Task<City> Get(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task<List<City>> GetAll()
        {
            return await _context
                         .Cities
                         .AsNoTracking()
                         .ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(object newUpdate, City modelToBeUpdated)
        {
            InputHelper.Trimmer(newUpdate);
            _mapper.Map(newUpdate, modelToBeUpdated);
        }

        private async Task<bool> isCityAlreadyExisted(string cityName)
        {
            return await _context.Cities
                .AnyAsync(x => x.Name.ToLower() == $"{cityName.ToLower()} city");
        }
        public async Task<ErrorValidator> Validate(CitySaveDto resourceSave)
        {
            if (resourceSave.Name.ToLower().Contains("city"))
                return new ErrorValidator("Input has 'City' Keyword please remove it!");
            if (await isCityAlreadyExisted(resourceSave.Name))
                return new ErrorValidator("This City Already Existed");
            return new ErrorValidator();
        }

        public async Task<ErrorValidator> Validate(City entity, CitySaveDto resourceSave)
        {
            if(entity.Name != resourceSave.Name)
            {
                if (resourceSave.Name.ToLower().Contains("city"))
                    return new ErrorValidator("Input has 'City' Keyword please remove it!");
                if (await isCityAlreadyExisted(resourceSave.Name))
                    return new ErrorValidator("This City Already Existed");
            }
            return new ErrorValidator();
        }
    }
}
