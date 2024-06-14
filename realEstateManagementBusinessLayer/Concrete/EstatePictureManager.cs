using System;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementDataLayer.Abstract;
using realEstateManagementEntities.Models;

namespace realEstateManagementBusinessLayer.Concrete
{
	public class EstatePictureManager : IEstatePictureService
	{
        private readonly IAsyncRepository<EstatePicture> _asyncRepository;

        public EstatePictureManager(IAsyncRepository<EstatePicture> asyncRepository)
		{
            _asyncRepository = asyncRepository;
        }

     
        public async Task<EstatePicture> AddEstatePicture(EstatePicture estatePicture)
        {
            return await _asyncRepository.AddAsync(estatePicture);
        }

        public async Task Delete(EstatePicture estatePicture)
        {
            await _asyncRepository.DeleteAsync(estatePicture);
        }
    }
}

