using System;
using realEstateManagementEntities.Models;

namespace realEstateManagementBusinessLayer.Abstract
{
	public interface IEstatePictureService
	{
        Task<EstatePicture> AddEstatePicture(EstatePicture estatePicture);
        Task Delete(EstatePicture estate);
    }
}