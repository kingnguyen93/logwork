using LogWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface IAddressService
    {
        Task<List<Address>> GetAddresses(int userId, int offset = 0, int limit = 0);

        Task<List<Address>> GetAddressesByClient(Client client, int offset = 0, int limit = 0);

        Task<List<Address>> SearchAddress(int userId, string searchKey, int limit = 0);
    }
}