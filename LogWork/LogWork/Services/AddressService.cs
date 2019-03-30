using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class AddressService : BaseService, IAddressService
    {
        public Task<List<Address>> GetAddresses(int userId, int offset = 0, int limit = 0)
        {
            var result = App.LocalDb.Table<Address>().ToList().FindAll(a => a.UserId == userId && a.IsActif == 1).OrderBy(c => c.Code).ToList();

            if (offset > 0)
                result = result.Skip(offset).ToList();

            if (limit > 0)
                result = result.Take(limit).ToList();

            return Task.FromResult(result);
        }

        public Task<List<Address>> GetAddressesByClient(Client client, int offset = 0, int limit = 0)
        {
            var result = App.LocalDb.Table<Address>().ToList().FindAll(a => ((a.FkClientServerId > 0 && a.FkClientServerId == client.ServerId) || (!a.Id.Equals(Guid.Empty) && !client.Id.Equals(Guid.Empty) && a.FkClientAppliId.Equals(client.Id))) && a.IsActif == 1).OrderBy(c => c.Code).ToList();

            if (offset > 0)
                result = result.Skip(offset).ToList();

            if (limit > 0)
                result = result.Take(limit).ToList();

            return Task.FromResult(result);
        }

        public Task<List<Address>> SearchAddress(int userId, string searchKey, int limit = 0)
        {
            var result = App.LocalDb.Table<Address>().ToList().FindAll(a => a.UserId == userId && ((a.Code > 0 && a.Code.ToString().Contains(searchKey)) || (!string.IsNullOrWhiteSpace(a.FullAddress) && a.FullAddress.UnSignContains(searchKey))) && a.IsActif == 1).OrderBy(c => c.Code).ToList();

            if (limit > 0)
                result = result.Take(limit).ToList();

            return Task.FromResult(result);
        }
    }
}