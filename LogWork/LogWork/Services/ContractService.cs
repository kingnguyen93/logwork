using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class ContractService : BaseService, IContractService
    {
        public Task<List<Contract>> GetContracts(int userId, int offset = 0, int limit = 0)
        {
            IEnumerable<Contract> result = App.LocalDb.Table<Contract>().ToList().FindAll(a => a.UserId == userId && a.IsActif == 1).OrderBy(c => c.Title);

            if (offset > 0)
                result = result.Skip(offset);

            if (limit > 0)
                result = result.Take(limit);

            return Task.FromResult(result.ToList());
        }

        public Task<List<Contract>> SearchContract(int userId, string searchKey, int limit = 0)
        {
            IEnumerable<Contract> result;

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                result = App.LocalDb.Table<Contract>().ToList().FindAll(con => con.UserId == userId && !string.IsNullOrWhiteSpace(con.Title) && con.IsActif == 1).OrderBy(c => c.Title);
            }
            else
            {
                result = App.LocalDb.Table<Contract>().ToList().FindAll(con => con.UserId == userId && ((!string.IsNullOrWhiteSpace(con.ConCode) && con.ConCode.Contains(searchKey)) || (!string.IsNullOrWhiteSpace(con.Title) && con.Title.UnSignContains(searchKey))) && con.IsActif == 1).OrderBy(c => c.Title);
            }

            if (limit > 0)
                result = result.Take(limit);

            return Task.FromResult(result.ToList());
        }

        public Task<List<Contract>> SearchContractByClient(Client client,int userId, string searchKey, int limit = 0)
        {
            IEnumerable<Contract> result;

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                result = App.LocalDb.Table<Contract>().ToList().FindAll(con => con.UserId == userId && con.ClientId == client.ServerId && !string.IsNullOrWhiteSpace(con.Title) && con.IsActif == 1).OrderBy(c => c.Title);
            }
            else
            {
                result = App.LocalDb.Table<Contract>().ToList().FindAll(con => con.UserId == userId && con.ClientId == client.ServerId && ((!string.IsNullOrWhiteSpace(con.ConCode) && con.ConCode.Contains(searchKey)) || (!string.IsNullOrWhiteSpace(con.Title) && con.Title.UnSignContains(searchKey))) && con.IsActif == 1).OrderBy(c => c.Title);
            }

            if (limit > 0)
                result = result.Take(limit);

            return Task.FromResult(result.ToList());
        }


    }
}
