﻿using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Logement.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        public CityController(ApplicationDbContext context, IConfiguration configuration)
            : base(context, configuration)
        { 
        }
          
        private City AddCityFromViewModel(string method, CityViewModel c)
        {
            City city = new City();

            if (method == "AddCity")
            {
                city = new City()
                {
                    Id = c.Id,
                    Name = c.Name,
                    LocatedAt = c.LocatedAt,
                    NumbersOfApartment = c.NumbersOfApartment,
                    Floor = c.Floor,
                    NumberOfParkingSpaces = c.NumberOfParkingSpaces,
                    DateAdded = DateTime.UtcNow
                };
            }
            else if (method == "EditCity")
            {
                city = new City()
                {
                    Id = c.Id,
                    Name = c.Name,
                    LocatedAt = c.LocatedAt,
                    NumbersOfApartment = c.NumbersOfApartment,
                    Floor = c.Floor,
                    NumberOfParkingSpaces = c.NumberOfParkingSpaces,
                    DateAdded = c.DateAdded
                };
            }
            return city;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<CityViewModel> citiesModel = new List<CityViewModel>();
                var cities = await dbc.Cities.ToListAsync();

                if (cities != null)
                {
                    var creatorCities = await dbc.CityMembers
                            .Where(c => c.UserId == GetUser().Id && c.Role == CityMemberRoleEnum.Admin)
                            .Include(c => c.City)
                            .ToListAsync();

                    foreach (var city in creatorCities)
                    {
                            var cityImage = await dbc.CityPhotos
                                  .Where(p => p.CityId == city.CityId)
                                  .Select(p => p.ImageURL)
                                  .FirstOrDefaultAsync();

                            citiesModel.Add(GetCitiesFromModel(city.City, cityImage));                     
                    }
                }
                return View(citiesModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
     
        //Get: Admin/AddApartment
        [HttpGet]
        public IActionResult AddCity()
        {
            CityViewModel city = new CityViewModel();
            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel cityViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    City city = AddCityFromViewModel("AddCity", cityViewModel);
                    var fileModel = new FileModel();

                    city.LandLordId = GetUser().Id;

                    dbc.Cities.Add(city);
                    await dbc.SaveChangesAsync();
                    await SaveImageFile(cityViewModel.CityImage, city.Id, null, "AddCity");


                    CityMember cityMember = new CityMember
                    {
                        CityId = city.Id,
                        UserId = city.LandLordId,
                        Role = CityMemberRoleEnum.Admin
                    };
                    dbc.CityMembers.Add(cityMember);
                    await dbc.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return View(cityViewModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditCity(long id)
        {
            try
            {
                var city = await dbc.Cities.FindAsync(id);
                if (city == null)
                    return NotFound();

                var cityCreator = await GetCityCreator(id);
                if (cityCreator == null)
                    return Forbid();
              
                var cityImage = await dbc.CityPhotos
                    .Where(c => c.CityId == id)
                    .Select(c => c.ImageURL)
                    .FirstOrDefaultAsync();

                CityViewModel cityViewModel = GetCitiesFromModel(city, cityImage);
                return View(cityViewModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditCity(long id, CityViewModel cityViewModel)
        {
            try
            {
                if (id != cityViewModel.Id)
                    return BadRequest();

                var city = await dbc.Cities.FindAsync(id);
                if (city == null)
                    return NotFound();

                var cityCreator = await GetCityCreator(id);
                if (cityCreator == null)
                    return Forbid();

                if (ModelState.IsValid)
                {
                    var dateAdded = await dbc.Cities
                        .Where(c => c.Id == id)
                        .Select(c => c.DateAdded)
                        .FirstOrDefaultAsync();

                    cityViewModel.DateAdded = dateAdded;

                    city = AddCityFromViewModel("EditCity", cityViewModel);
                    city.LandLordId = cityCreator.UserId;

                    await dbc.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(cityViewModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }         
        }
    }
}