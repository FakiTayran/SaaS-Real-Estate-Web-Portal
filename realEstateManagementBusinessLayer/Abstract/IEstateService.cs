using System;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;

namespace realEstateManagementBusinessLayer.Abstract
{
    public interface IEstateService
	{
        Task<List<EstateDto>> ListEstatesAsync(ISpecification<Estate> spec);
        Task<Estate> AddEstate(Estate estate);
        Task EditEstate(Estate estate);
        Task Delete(Estate estate);
        Task<Estate> GetByIdAsync(int id);
       
    }
}

