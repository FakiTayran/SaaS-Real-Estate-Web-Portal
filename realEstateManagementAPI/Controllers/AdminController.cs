﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using realEstateManagementAPI.UtilityHelper;
using realEstateManagementBusinessLayer.Abstract;
using realEstateManagementBusinessLayer.Concrete.Spesification;
using realEstateManagementDataLayer.EntityFramework;
using realEstateManagementEntities.Models;
using realEstateManagementEntities.Models.Dtos;


namespace realEstateManagementAPI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //private readonly IAgentService _customerService;
        private readonly IEstateService _estateService;
        private readonly IEstatePictureService _estatePictureService;
        private readonly RealEstateManagementDbContext? _context;
        private readonly UserManager<AdminUser> _userManager;

        public AdminController(UserManager<AdminUser> userManager,IEstateService estateService,IEstatePictureService estatePictureService,RealEstateManagementDbContext realEstateManagementDbContext)
        {
            _estateService = estateService;
            _context = realEstateManagementDbContext;
            _estatePictureService = estatePictureService;
            _userManager = userManager;
        }



        // GET: /<controller>/
        [HttpGet("GetEstates")]
        public async Task<IActionResult> GetAllEstates(
            EstateType? estateType = null,
            PropertyType? propertyType = null,
            int? RealEstateCompanyId = null,
            int? numberOfBedRooms = null,
            int? numberOfBathRooms = null,
            int? minPrice = null,
            int? maxPrice = null,
            int? squareMeterMin = null,
            int? squareMeterMax = null,
            bool? garden = null,
            bool? balcony = null,
            string? city = null,
            string? postCode = null,
            string? searchText = null,
            string? adminUserId = null)
        {
            if (!RealEstateCompanyId.HasValue)
            {
                throw new ArgumentNullException(nameof(RealEstateCompanyId), "RealEstateCompanyId must be provided.");
            }

            var spec = new EstateFilterSpesification(
                estateType, propertyType, RealEstateCompanyId, numberOfBedRooms, numberOfBathRooms,
                minPrice, maxPrice, squareMeterMin, squareMeterMax, garden, balcony, city, postCode, searchText, adminUserId
            );
            var estates = await _estateService.ListEstatesAsync(spec);

            return Ok(estates);
        }



        [HttpPost("AddEstate")]
        public async Task<IActionResult> AddEstate([FromBody]Estate estate)
        {
            if (!string.IsNullOrEmpty(estate.City) || !string.IsNullOrEmpty(estate.PostCode) || !string.IsNullOrEmpty(estate.Description) || !string.IsNullOrEmpty(estate.Headline))
            {
                var addedEstate = await _estateService.AddEstate(estate);
                if(estate.EstateAgentId != null)
                {
                    var estateAgent = _userManager.Users.FirstOrDefault(x => x.Id == estate.EstateAgentId);
                    addedEstate.EstateAgent = estateAgent;
                }

                return Ok(addedEstate);
            }
            else
            {
                return Ok(new GeneralResponse<string>
                {
                    Result = "Some of these values might be null or empty (city,postcode,description and headline)",
                    IsError = true,
                    Code = 1
                });
            }
             
            
        }

        [HttpPut("EditEstate")]
        public async Task<IActionResult> EditEstate([FromBody] Estate estate)
        {
            var willBeUpdatedEstate = await _estateService.GetByIdAsync(estate.Id);
            willBeUpdatedEstate.EstateType = estate.EstateType;
            willBeUpdatedEstate.PropertyType = estate.PropertyType;
            willBeUpdatedEstate.PostCode = estate.PostCode;
            willBeUpdatedEstate.NumberOfBedRooms = estate.NumberOfBedRooms;
            willBeUpdatedEstate.NumberOfBathRooms = estate.NumberOfBathRooms;
            willBeUpdatedEstate.Garden = estate.Garden;
            willBeUpdatedEstate.Balcony = estate.Balcony;
            willBeUpdatedEstate.Price = estate.Price;
            willBeUpdatedEstate.SquareMeter = estate.SquareMeter;
            willBeUpdatedEstate.LandLordName = estate.LandLordName;
            willBeUpdatedEstate.LandLordPhone = estate.LandLordPhone;
            willBeUpdatedEstate.LandLordEmail = estate.LandLordEmail;


            willBeUpdatedEstate.Headline = estate.Headline;
            if (estate.EstateAgentId != willBeUpdatedEstate.EstateAgentId)
            {
                var estateAgent = _userManager.Users.FirstOrDefault(x => x.Id == estate.EstateAgentId);
                willBeUpdatedEstate.EstateAgent = estateAgent;
            }
            willBeUpdatedEstate.City = estate.City;
            if (!string.IsNullOrEmpty(estate.City) || !string.IsNullOrEmpty(estate.PostCode) || !string.IsNullOrEmpty(estate.Description) || !string.IsNullOrEmpty(estate.Headline))
            {
                await _estateService.EditEstate(willBeUpdatedEstate);
                return Ok(willBeUpdatedEstate);
            }
            else
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Result = "Some of these values might be null or empty (city,postcode,description and headline)",
                    IsError = true,
                    Code = 1
                });
            }
           
        }

        [HttpDelete("DeleteEstate/{estateId}")]
        public async Task<IActionResult> DeleteEstate(int estateId)
        {
            var willBeDeletedEstate = await _estateService.GetByIdAsync(estateId);
            if (willBeDeletedEstate == null)
            {
                return NotFound($"Estate with ID {estateId} not found.");
            }
            await _estateService.Delete(willBeDeletedEstate);
            return Ok(estateId);
        }

        [HttpGet("GetEstateDetail/{estateId}")]

        public async Task<IActionResult> GetEstateDetail(int estateId)
        {
            var estate = await _estateService.GetByIdAsync(estateId);
            return Ok(estate);
        }

        [HttpPost("AddEstatePhotos/{estateId}")]
        public async Task<IActionResult> AddEstatePhotos(int estateId, [FromForm] IFormCollection imgs)
        {
            var estate = await _estateService.GetByIdAsync(estateId);
            if (estate == null)
            {
                return NotFound($"Estate with ID {estateId} not found.");
            }

            if (imgs == null || imgs.Files.Count == 0)
            {
                return BadRequest("No images provided.");
            }

            foreach (var image in imgs.Files)
            {
                if (image.Length > 0)
                {
                    byte[] imageBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }

                    var estatePicture = new EstatePicture()
                    {
                        EstateId = estateId,
                        img = imageBytes,
                        Estate = estate
                    };

                    var addedEstatePicture = await _estatePictureService.AddEstatePicture(estatePicture);
                    estate.EstatePictures.Add(addedEstatePicture);
                    await _estateService.EditEstate(estate);
                }
            }

            return Ok(new { message = "Images added successfully." });
        }

        [HttpDelete("DeleteEstatePhoto/{photoId}")]
        public async Task<IActionResult> DeleteEstatePhoto( int photoId)
        {

            var picture = await _estatePictureService.GetEstatePictureById(photoId);
            if (picture!=null)
            {
                await _estatePictureService.Delete(picture);
                return Ok(new {message= "Photo removed successfully." });

            }
            return BadRequest("Photo cannot removed. An Error Occured");

        }
     

    }
}

