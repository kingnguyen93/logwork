using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class ClientService : BaseService, IClientService
    {
        public Task<List<Client>> GetClients(int userId, int offset = 0, int limit = 0)
        {
            var result = App.LocalDb.Table<Client>().ToList().FindAll(c => c.UserId == userId && c.IsActif == 1).OrderBy(c => c.Code).ToList();

            if (offset > 0)
                result = result.Skip(offset).ToList();

            if (limit > 0)
                result = result.Take(limit).ToList();

            foreach (var client in result)
            {
                client.AddressesCount = GetAddressesCount(client)?.Result ?? 0;
            }

            return Task.FromResult(result);
        }

        public Task<List<Client>> SearchClient(int userId, string searchKey, int limit = 0)
        {
            var result = App.LocalDb.Table<Client>().ToList().FindAll(c => c.UserId == userId && ((c.Code > 0 && c.Code.ToString().Contains(searchKey)) || (!string.IsNullOrWhiteSpace(c.Title) && c.Title.UnSignContains(searchKey)) || (!string.IsNullOrWhiteSpace(c.FullName) && c.FullName.UnSignContains(searchKey))) && c.IsActif == 1).OrderBy(c => c.Code).ToList();

            if (limit > 0)
                result = result.Take(limit).ToList();

            foreach (var client in result)
            {
                client.AddressesCount = GetAddressesCount(client)?.Result ?? 0;
            }

            return Task.FromResult(result);
        }

        public Task<int> GetAddressesCount(Client client)
        {
            return Task.FromResult(App.LocalDb.Table<Address>().Count(adr => ((adr.FkClientServerId > 0 && adr.FkClientServerId == client.ServerId) || (!adr.FkClientAppliId.Equals(Guid.Empty) && !client.Id.Equals(Guid.Empty) && adr.FkClientAppliId.Equals(client.Id))) && adr.IsActif == 1));
        }

        public Task<List<Address>> GetAddresses(Client client)
        {
            return Task.FromResult(App.LocalDb.Table<Address>().ToList().FindAll(adr => adr.UserId == client.UserId && ((adr.FkClientServerId > 0 && adr.FkClientServerId == client.ServerId) || (!adr.FkClientAppliId.Equals(Guid.Empty) && !client.Id.Equals(Guid.Empty) && adr.FkClientAppliId.Equals(client.Id))) && adr.IsActif == 1).ToList());
        }
    }
}