using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using BlogProject.Data;
using BlogProject.Repository;

namespace BlogProject.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserServices _userServices;
        private readonly ProgramDbContext _context;

        public UsersController(ProgramDbContext context, UserServices userServices)
        {
            _userServices = userServices;
            _context = context;
        }

        // GET: Users
        public IActionResult Index()
        {
            return View(_userServices.GetAll());
        }

        // GET: Users/Details/5
        public IActionResult Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userServices.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserName,Email,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                await _userServices.Insert(user);
                await _userServices.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userServices.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserName,Email,Password,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _userServices.Update(user);
                    await _userServices.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userServices.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userServices.GetById(id);
            if (user != null)
            {
                _userServices.Remove(user);
            }

            await _userServices.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            var record = _userServices.GetById(id);
            return record != null;
        }
    }
}
