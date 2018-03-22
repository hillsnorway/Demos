using SIS.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIS.Models.Data;
using SIS.Models.ViewModels;

namespace SIS.Controllers
{
    public class StudentController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult List()
        {
            var model = StudentRepository.GetAll();

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            var viewModel = new StudentVM();
            viewModel = GetStudentVMListItems(viewModel); //must be called after viewModel.Student is initialized
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(StudentVM viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel = SetStudentVMListItems(viewModel);
                StudentRepository.Add(viewModel.Student);

                return RedirectToAction("List", "Student");
            }
            else
            {
                viewModel = GetStudentVMListItems(viewModel);

                return View(viewModel);
            }

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            StudentVM viewModel = new StudentVM();
            viewModel.Student = StudentRepository.Get(id);
            viewModel = GetStudentVMListItems(viewModel); //must be called after viewModel.Student is populated


            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(StudentVM viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel = SetStudentVMListItems(viewModel);
                StudentRepository.Edit(viewModel.Student);

                return RedirectToAction("List", "Student");
            }
            else
            {
                viewModel = GetStudentVMListItems(viewModel);

                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var student = StudentRepository.Get(id);
            return View(student);
        }

        [HttpPost]
        public ActionResult Delete(Student student)
        {
            StudentRepository.Delete(student.StudentId);
            return RedirectToAction("List", "Student");
        }

        [HttpGet]
        public ActionResult AddAddress(int id)
        {
            //Get student ID
            Student student = new Student();
            student = StudentRepository.Get(id);

            AddressVM viewModel = new AddressVM();
            viewModel.StudentId = student.StudentId;
            viewModel = GetAddressVMListItems(viewModel);
            return View("AddEditAddress", viewModel);
        }

        [HttpPost]
        public ActionResult AddEditAddress(AddressVM viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel = SetAddressVMListItems(viewModel);
                StudentRepository.SaveAddress(viewModel.StudentId, viewModel.Address);
                return RedirectToAction("Edit", "Student", new { id = viewModel.StudentId });
            }
            else
            {
                viewModel = GetAddressVMListItems(viewModel);
                return View("AddEditAddress", viewModel);
            }
        }

        [HttpGet]
        public ActionResult EditAddress(int id)
        {
            //Get student ID
            Student student = new Student();
            student = StudentRepository.Get(id);

            AddressVM viewModel = new AddressVM();
            viewModel.StudentId = student.StudentId;
            viewModel.Address = student.Address;
            viewModel = GetAddressVMListItems(viewModel);
            return View("AddEditAddress", viewModel);
        }

        [HttpGet]
        public ActionResult DeleteAddress(int id)
        {
            var student = StudentRepository.Get(id);
            return View(student);
        }

        [HttpPost]
        public ActionResult DeleteAddress(Student student)
        {
            StudentRepository.DeleteAddress(student.StudentId);
            return RedirectToAction("Edit", "Student", new { id = student.StudentId });
        }

        private StudentVM GetStudentVMListItems(StudentVM viewModel)
        {
            //Load the Selected CourseIds from Student.Courses
            if (viewModel.Student.Courses!=null)
            {
                foreach (var course in viewModel.Student.Courses)
                viewModel.SelectedCourseIds.Add(course.CourseId);
            }
            viewModel.SetCourseItems(CourseRepository.GetAll());
            viewModel.SetMajorItems(MajorRepository.GetAll());
            viewModel.SetStateItems(StateRepository.GetAll());
            return viewModel;
        }

        private StudentVM SetStudentVMListItems(StudentVM viewModel)
        {
            //Rewrite Student.Courses with values from SelectedCourseIds
            viewModel.Student.Courses = new List<Course>();
            foreach (var id in viewModel.SelectedCourseIds)
                viewModel.Student.Courses.Add(CourseRepository.Get(id));

            viewModel.Student.Major = MajorRepository.Get(viewModel.Student.Major.MajorId);
            if(viewModel.Student.Address!=null)
                viewModel.Student.Address.State = StateRepository.Get(viewModel.Student.Address.State.StateAbbreviation);
            return viewModel;
        }

        private AddressVM GetAddressVMListItems(AddressVM viewModel)
        {
            viewModel.SetStateItems(AddrStates.GetAll());
            return viewModel;
        }

        private AddressVM SetAddressVMListItems(AddressVM viewModel)
        {
            if (viewModel.Address != null)
                viewModel.Address.State = AddrStates.Get(viewModel.Address.State.StateAbbreviation);
            return viewModel;
        }
    }
}