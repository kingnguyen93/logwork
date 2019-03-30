using LogWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface IClientService
    {
        Task<List<Client>> GetClients(int userId, int offset = 0, int limit = 0);

        Task<List<Client>> SearchClient(int userId, string searchKey, int limit = 0);

        Task<List<Address>> GetAddresses(Client client);
    }
}