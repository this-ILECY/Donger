using Donger.Data;
using Donger.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donger.Services
{
    public class PersonService : IPersonService
    {
        private readonly ApplicationDbContext _context;

        public PersonService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await _context.People.ToListAsync();
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            return await _context.People.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPersonAsync(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePersonAsync(Person person)
        {
            _context.Attach(person).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PersonExistsAsync(person.Id))
                {
                    throw new KeyNotFoundException($"Person with id {person.Id} not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeletePersonAsync(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PersonExistsAsync(int id)
        {
            return await _context.People.AnyAsync(e => e.Id == id);
        }
    }
}