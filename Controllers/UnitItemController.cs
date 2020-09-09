﻿using BataCMS.Data.Interfaces;
using BataCMS.Data.Models;
using BataCMS.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BataCMS.ViewModels;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using BataCMS.Data;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace BataCMS.Controllers
{
    public class UnitItemController : Controller 
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitItemRepository _unitItemRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        


        public UnitItemController(IUnitItemRepository unitItemRepository, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _unitItemRepository = unitItemRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment; 
        }

/*        public async Task<IActionResult> DeleteItemAsync(int id)
        {
            var unitItem = _unitItemRepository.GetItemById(id);

            if (unitItem == null)
            {
                ViewBag.ErrorMessage = $"Item with  id ={id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _unitItemRepository.DeleteItem(id)   ;

                //success
                if (result > 0)
                {
                    return RedirectToAction("List");
                }
                return View("List");

            }
        }*/

        [HttpGet]
        public IActionResult Create()
        {
            this.ViewData["categories"] = _categoryRepository.Categories.Select(x => new SelectListItem
            {
                Value = x.CategoryName,
                Text = x.CategoryName
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUnitItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                string photoPath = ProcessUploadedImage(model);

                var category = _categoryRepository.Categories.FirstOrDefault(p => p.CategoryName == model.Category);

                unitItem newUnitItem = new unitItem
                {
                    Name = model.Name,
                    Price = model.Price,
                    InStock = model.InStock,
                    DateModified = DateTime.Today,
                    CategoryId = category.CategoryId,
                    ImageUrl = photoPath,
                    OptionFormData = model.OptionFormData,
                    Description = model.Description
                };

                await _unitItemRepository.AddAsync(newUnitItem);
                return RedirectToAction("List", new { id = newUnitItem.unitItemId });

            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            this.ViewData["categories"] = _categoryRepository.Categories.Select(x => new SelectListItem
            {
                Value = x.CategoryName,
                Text = x.CategoryName
            }).ToList();

            unitItem unitItem = await _unitItemRepository.GetItemByIdAsync(id);
            Category category = _categoryRepository.Categories.FirstOrDefault(p => p.CategoryId == unitItem.CategoryId);

            EditUnitItemViewModel editUnitItemViewModel = new EditUnitItemViewModel
            {
                unitItemId = unitItem.unitItemId,
                Name = unitItem.Name,
                Price = unitItem.Price,
                InStock = unitItem.InStock,
                Category = category.CategoryName,
                ExistingImagePath = unitItem.ImageUrl,
                OptionFormData = unitItem.OptionFormData,
                Description = unitItem.Description
            };
            return View(editUnitItemViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditAsync(EditUnitItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                unitItem unitItem = await _unitItemRepository.GetItemByIdAsync(model.unitItemId);
                Category category = _categoryRepository.Categories.FirstOrDefault(p => p.CategoryName == model.Category);


                unitItem.Name = model.Name;
                unitItem.Price = model.Price;
                unitItem.CategoryId = category.CategoryId;
                unitItem.InStock = model.InStock;
                unitItem.OptionFormData = model.OptionFormData; 
                unitItem.DateModified = DateTime.Today;
                unitItem.Description = model.Description;

                if (model.Image != null)
                {
                    if (model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath + model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }

                    unitItem.ImageUrl = ProcessUploadedImage(model);

                }

                await _unitItemRepository.EditItemAsync(unitItem);
                return RedirectToAction("List");

            }
            return View();
        }

        private string ProcessUploadedImage(CreateUnitItemViewModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }

            }

            string photoPath = "/images/" + uniqueFileName;
            return photoPath;
        }

        public ViewResult List(string category, string searchString)
        {
            string _category = category;
            IEnumerable<unitItem> unitItems;

            string currentCategory = string.Empty;

            if (!string.IsNullOrEmpty(searchString))
            {
                unitItems = _unitItemRepository.unitItems.Where(p => p.Name.Contains(searchString));
            } else if (!string.IsNullOrEmpty(category))
            {
                unitItems = _unitItemRepository.unitItems.Where(p => p.Category.CategoryName.Equals(_category));
                currentCategory = _category;
            }
            else
            {
                unitItems = _unitItemRepository.unitItems.OrderBy(p => p.unitItemId);
                currentCategory = "All Items";
            }

            var vm = new UnitItemListViewModel
            {
                UnitItems = unitItems,
                CurrentCategory = currentCategory
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAsync(int itemId)
        {
            unitItem unitItem = await _unitItemRepository.GetItemByIdAsync(itemId);
            return View(unitItem);
        }
    }
}
