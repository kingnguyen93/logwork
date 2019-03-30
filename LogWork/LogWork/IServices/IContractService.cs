using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogWork.Models;

namespace LogWork.IServices
{
    public interface IContractService
    {
        Task<List<Contract>> GetContracts(int userId, int offset = 0, int limit = 0);

        Task<List<Contract>> SearchContract(int userId, string searchKey, int limit = 0);
        Task<List<Contract>> SearchContractByClient(Client client,int userId, string searchKey, int limit = 0);
    }
}
