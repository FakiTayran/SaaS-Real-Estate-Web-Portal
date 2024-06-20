using System;
using realEstateManagementEntities.Models;

namespace realEstateManagementBusinessLayer.Abstract
{
    public interface IEstateCompanyService
    {
        Task<RealEstateCompany> AddEstateCompany(RealEstateCompany estateCompany);
        Task<RealEstateCompany> GetEstateCompanyById(int estateCompanyId);

    }
}

