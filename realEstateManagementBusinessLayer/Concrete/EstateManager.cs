using System;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementDataLayer.Abstract;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;

namespace realEstateManagementBusinessLayer.Concrete
{
	public class EstateManager : IEstateService
    {
        private readonly IAsyncRepository<Estate> _asyncRepository;

        public EstateManager(IAsyncRepository<Estate> asyncRepository)
		{
            _asyncRepository = asyncRepository;
        }

        public async Task<Estate> AddEstate(Estate estate)
        {
            return await _asyncRepository.AddAsync(estate);
        }

        public async Task Delete(Estate estate)
        {
            await _asyncRepository.DeleteAsync(estate);
        }

        public async Task EditEstate(Estate estate)
        {
            await _asyncRepository.UpdateAsync(estate);
        }

        public async Task<Estate> GetByIdAsync(int id)
        {
            return await _asyncRepository.GetByIdAsync(id);
        }

        public async Task<List<EstateDto>> ListEstatesAsync(ISpecification<Estate> spec)
        {
            List<Estate> estates = await _asyncRepository.ListAsync(spec); // Ensure this call is awaited

            List<EstateDto> estateDtos = new List<EstateDto>();
            foreach (var estate in estates)
            {
                EstateDto estateDto = new EstateDto()
                {
                    id = estate.Id,
                    EstateType = estate.EstateType,
                    PropertyType = estate.PropertyType,
                    City = estate.City,
                    Description = estate.Description,
                    Headline = estate.Headline,
                    EstateAgent = estate.EstateAgent,
                    NumberOfRooms = estate.NumberOfRooms,
                    PostCode = estate.PostCode,
                    EstatePictures = estate.EstatePictures.Select(x => x.img).ToList(),
                };
                estateDtos.Add(estateDto);
            }

            return estateDtos;
        }


    }
}

