using AttarStore.Entities;
using System.Threading.Tasks;

namespace AttarStore.Repositories
{
    public interface IManufacturerRepository
    {
        Task<Manufacturer[]> GetAllManufacturers();
        Task<Manufacturer> GetManufacturerByIdAsync(int id);
        Task AddManufacturer(Manufacturer manufacturer);
        Task UpdateManufacturer(Manufacturer manufacturer);
        Task DeleteManufacturer(int id);
    }
}
