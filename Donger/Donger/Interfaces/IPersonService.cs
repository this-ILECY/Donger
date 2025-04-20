using Donger.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donger.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task<Person> GetPersonByIdAsync(int id);
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int id);
        Task<bool> PersonExistsAsync(int id);
    }
}