﻿using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Repository
{
    public class UserServices
    {
        public readonly ProgramDbContext _context;
        public UserServices(ProgramDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User.ToList();
        }
        public async Task<User> GetById(int id)
        {
            var record = await _context.User.FindAsync(id);
            return record;
        }
        public User GetByUserName(string UserName)
        {
            return _context.User.FirstOrDefault(x => x.UserName == UserName);

        }
        public User GetByUserEmail(string Email)
        {
            return _context.User.FirstOrDefault(x => x.Email == Email);

        }
        public async Task Insert(User user)
        {
            await _context.User.AddAsync(user);
        }
        public void Update(User user)
        {
            _context.User.Update(user);
        }
        public void Remove(User user)
        {
            _context.Remove(user);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> TotalBlogs(int id)
        {
            var TotalBlogsCount = await _context.Blogs.Where(b => b.UserId == id).CountAsync();
            return TotalBlogsCount;
        }
        public async Task<int> TotalLikes(int id)
        {
            var TotalLikesCount = await _context.Likes.Where(l => l.UserId == id).CountAsync();
            return TotalLikesCount;
        }
        public async Task<int> TotalComments(int id)
        {
            var TotalCommentsCount = await _context.Comments.Where(c => c.UserID == id).CountAsync();
            return TotalCommentsCount;
        }
    }
}
