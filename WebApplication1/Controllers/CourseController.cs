﻿using Microsoft.AspNetCore.Mvc;
using WebAppRepositoryWithUOW.Core;
using WebAppRepositoryWithUOW.Core.Models;

namespace WebApplication1.Controllers
{
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        //httpGet: get all departments
        public IActionResult Index()
        {
            var courses = _unitOfWork.CourseRepository.GetAll(x => x.Department);
            return View(courses);
        }


        //httpGet: get detail of object
        public IActionResult Details(int id)
        {
            var course = _unitOfWork.CourseRepository.Find(x => x.Id == id, x => x.Department);
            return View(course);
        }


        //httpGet: create view to add new object
        public IActionResult Create()
        {
            var newCourse = new Course();
            return View(newCourse);
        }


        //httpPost: check for validation and confirm save data
        [HttpPost]
        public IActionResult Create([Bind("Name, MaxDegree, MinDegree, DepartmentId")] Course newCourse)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CourseRepository.Create(newCourse);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(newCourse);
        }

        //httpGet: create view to edit object
        public IActionResult Update(int id)
        {
            var course = _unitOfWork.DepartmentRepository.Find(x => x.Id == id);
            return View(course);
        }


        //httpPost: check for validation and confirm save data
        [HttpPost]
        public IActionResult Update(Course modifiedCourse)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CourseRepository.Update(modifiedCourse);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(modifiedCourse);
        }


        //httpGet: delete
        public IActionResult Delete(int id)
        {
            try
            {
                var course = _unitOfWork.CourseRepository.Find(x => x.Id == id);
                _unitOfWork.CourseRepository.Delete(course);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(null, ex.InnerException.Message);
                return View(nameof(Index));
            }
        }

    }
}
