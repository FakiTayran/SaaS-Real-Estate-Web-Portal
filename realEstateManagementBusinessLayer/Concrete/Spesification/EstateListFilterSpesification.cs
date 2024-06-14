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
        public EstateFilterSpesification(EstateType? estateType, PropertyType? propertyType, int? numberOfRooms, string? city, string? postCode, string? searchText, string? adminUserId)
        {
            if (estateType.HasValue)
            {
                Query.Where(x => x.EstateType == estateType.Value);
            }

            if (propertyType.HasValue)
            {
                Query.Where(x => x.PropertyType == propertyType.Value);
            }

            if (numberOfRooms.HasValue)
            {
                Query.Where(x => x.NumberOfRooms == numberOfRooms.Value);
            }

            if (!string.IsNullOrEmpty(city))
            {
                Query.Where(x => x.City == city);
            }

            if (!string.IsNullOrEmpty(postCode))
            {
                Query.Where(x => x.PostCode == postCode);
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                Query.Where(x => x.Id.ToString().Contains(searchText) || x.Description.ToLower().Contains(searchText));
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

