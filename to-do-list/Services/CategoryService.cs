using Microsoft.EntityFrameworkCore;
using to_do_list.Data;
using to_do_list.Models;
using to_do_list.Services.Interfaces;

namespace to_do_list.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Category>> GetAllAsync()
            => _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

        public Task<Category?> GetByIdAsync(int id)
            => _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Categories.FindAsync(id);
            if (entity == null) return;
            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
