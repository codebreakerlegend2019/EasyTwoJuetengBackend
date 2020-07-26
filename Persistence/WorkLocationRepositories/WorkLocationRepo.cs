using AutoMapper;
using EasyTwoJuetengBackend.DataContexts;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using EasyTwoJuetengBackend.Dtos.WorkLocationDto;
using EasyTwoJuetengBackend.Interfaces;
using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.WorkLocationRepositories
{
    public class WorkLocationRepo:ICrudPattern<WorkLocation>,IValidator<WorkLocation,WorkLocationSaveDto>
    {
        private readonly EasyTwoJuetengContext _context;
        private readonly IMapper _mapper;

        public WorkLocationRepo(EasyTwoJuetengContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Create(WorkLocation model)
        {
            _context.Add(model);
        }

        public void Delete(WorkLocation model)
        {
            _context.Remove(model);
        }

        public async Task<WorkLocation> Get(int id)
        {
            return await _context.WorkLocations
                                 .Include(x => x.City)
                                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<WorkLocation>> GetAll()
        {
            return await _context.WorkLocations
                                 .Include(x => x.City)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(object newUpdate, WorkLocation modelToBeUpdated)
        {
            _mapper.Map(newUpdate, modelToBeUpdated);
        }

        private async Task<bool> isWorkLocationAlreadyExist(string name)
        {
            return await _context.WorkLocations
                                 .AnyAsync(x=>x.Name.ToLower() == name.ToLower());
        }
        private async Task<bool> isWorkLocationAndCityAlreadyExist(string name,int cityId)
        {
            return await _context.WorkLocations
                                 .AnyAsync(x => x.Name.ToLower() == name.ToLower() &&
                                                x.CityId == cityId);
        }
        
        private async Task<bool> isBarangayAlreadyExist(string barangay)
        {
            return await _context.WorkLocations
                                 .AnyAsync(x => x.Barangay.ToLower() == barangay.ToLower());
        }
        public async Task<ErrorValidator> Validate(WorkLocationSaveDto resourceSave)
        {
            if (await isWorkLocationAlreadyExist(resourceSave.Name))
                return new ErrorValidator("Work Location Name Already Existed");
            if (await isWorkLocationAndCityAlreadyExist(resourceSave.Name,resourceSave.CityId))
                return new ErrorValidator("Work Location Name and City Already Existed");
            if (await isBarangayAlreadyExist(resourceSave.Barangay))
                return new ErrorValidator("Work Location Barangay Already Existed");
            return new ErrorValidator();
        }

        public async Task<ErrorValidator> Validate(WorkLocation entity, WorkLocationSaveDto resourceSave)
        {
            if(entity.Name!= resourceSave.Name)
                if (await isWorkLocationAlreadyExist(resourceSave.Name))
                    return new ErrorValidator("Work Location Name Already Existed");
            if(entity.Name!=resourceSave.Name && entity.CityId != resourceSave.CityId)
                if (await isWorkLocationAndCityAlreadyExist(resourceSave.Name, resourceSave.CityId))
                    return new ErrorValidator("Work Location Name and City Already Existed");
            if(entity.Barangay!= resourceSave.Barangay)
                if (await isBarangayAlreadyExist(resourceSave.Barangay))
                    return new ErrorValidator("Work Location Barangay Already Existed");
            return new ErrorValidator();
        }
    }
}
