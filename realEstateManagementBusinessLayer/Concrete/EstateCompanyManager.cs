using System;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementDataLayer.Abstract;
using realEstateManagementEntities.Models;

namespace realEstateManagementBusinessLayer.Concrete
{
    public class EstateCompanyManager : IEstateCompanyService
    {
        private readonly IAsyncRepository<RealEstateCompany> _asyncRepository;

        public EstateCompanyManager(IAsyncRepository<RealEstateCompany> asyncRepository)
        {
            _asyncRepository = asyncRepository;
        }
        public async Task<RealEstateCompany> AddEstateCompany(RealEstateCompany estateCompany)
        {
            return await _asyncRepository.AddAsync(estateCompany);

        }

        public async Task<RealEstateCompany> GetEstateCompanyById(int estateCompanyId)
        {
            return await _asyncRepository.GetByIdAsync(estateCompanyId);
        }
    }
}

