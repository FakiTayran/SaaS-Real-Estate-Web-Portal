using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using realEstateManagementEntities.Models;

namespace realEstateManagementBusinessLayer.Concrete.Spesification
{
    public class EstateFilterSpesification : Specification<Estate>
    {
        public EstateFilterSpesification(EstateType? estateType, PropertyType? propertyType,int? RealEstateCompanyId,int? numberOfBedRooms, int? numberOfBathRooms,int? minPrice,int? maxPrice,int? squaremeterMin,int? squaremeterMax, bool? garden,bool? balcony,string? city, string? postCode, string? searchText, string? adminUserId)
        {
            if (estateType.HasValue)
            {
                Query.Where(x => x.EstateType == estateType.Value);
            }

            if (propertyType.HasValue)
            {
                Query.Where(x => x.PropertyType == propertyType.Value);
            }

            if (numberOfBedRooms.HasValue)
            {
                Query.Where(x => x.NumberOfBedRooms == numberOfBedRooms.Value);
            }

            if (numberOfBathRooms.HasValue)
            {
                Query.Where(x => x.NumberOfBathRooms == numberOfBathRooms.Value);
            }

           
            Query.Where(x => x.RealEstateCompanyId == RealEstateCompanyId.Value);
            


            if (garden.HasValue)
            {
                Query.Where(x => x.Garden == garden);
            }

            if (balcony.HasValue)
            {
                Query.Where(x => x.Balcony == balcony);
            }


            if (squaremeterMin.HasValue)
            {
                Query.Where(x => x.SquareMeter > squaremeterMin);
            }

            if (squaremeterMax.HasValue)
            {
                Query.Where(x => x.SquareMeter < squaremeterMax);
            }

            if (minPrice.HasValue)
            {
                Query.Where(x => x.Price > minPrice);
            }

            if (maxPrice.HasValue)
            {
                Query.Where(x => x.Price < maxPrice);
            }


            if (!string.IsNullOrEmpty(city))
            {
                Query.Where(x => x.City.ToLower().Trim() == city.ToLower().Trim());
            }

            if (!string.IsNullOrEmpty(postCode))
            {
                Query.Where(x => x.PostCode.ToLower().Trim() == postCode.ToLower().Trim());
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                Query.Where(x => x.Headline.ToLower().Contains(searchText.ToLower()) || x.Description.ToLower().Contains(searchText.ToLower()));
            }

            if (!string.IsNullOrEmpty(adminUserId))
            {
                Query.Where(x => x.EstateAgent.Id == adminUserId);
            }

            Query.Include(x => x.EstateAgent);
            Query.Include(x => x.EstatePictures);
        }
    }
}

