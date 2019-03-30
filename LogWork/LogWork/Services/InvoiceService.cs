using LogWork.IServices;
using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class InvoiceService : BaseService, IInvoiceService
    {
        public Task<List<Invoice>> GetQuotes(int offset = 0, int limit = 0)
        {
            IEnumerable<Invoice> result = App.LocalDb.Table<Invoice>().ToList().FindAll(inv => inv.UserId == CurrentUser.Id && inv.IsInvoice != 1 && inv.IsInvoice != 2 && inv.IsActif == 1)
                .OrderByDescending(inv => inv.IDate).ThenByDescending(inv => inv.IDueDate).ThenByDescending(inv => inv.EditDate).ThenByDescending(inv => inv.AddDate);

            if (offset > 0)
                result = result.Skip(offset);

            if (limit > 0)
                result = result.Take(limit);

            foreach (var item in result)
            {
                GetRelations(item);
                GetListInvoiceProducts(item);
            }

            return Task.FromResult(result.ToList());
        }

        public Task<List<Invoice>> GetInvoices(int offset = 0, int limit = 0)
        {
            IEnumerable<Invoice> result = App.LocalDb.Table<Invoice>().ToList().FindAll(inv => inv.UserId == CurrentUser.Id && (inv.IsInvoice == 1 || inv.IsInvoice == 2) && inv.IsActif == 1)
                .OrderByDescending(inv => inv.IDate).ThenByDescending(inv => inv.IDueDate).ThenByDescending(inv => inv.EditDate).ThenByDescending(inv => inv.AddDate);

            if (offset > 0)
                result = result.Skip(offset);

            if (limit > 0)
                result = result.Take(limit);

            foreach (var item in result)
            {
                GetRelations(item);
                GetListInvoiceProducts(item);
            }

            return Task.FromResult(result.ToList());
        }

        public Task<List<Invoice>> SearchInvoices(string searchKey, int limit = 0)
        {
            IEnumerable<Invoice> result = App.LocalDb.Table<Invoice>().ToList().FindAll(inv => inv.UserId == CurrentUser.Id && (inv.IsInvoice == 1 || inv.IsInvoice == 2)
                && ((!string.IsNullOrWhiteSpace(inv.Nonce) && inv.Nonce.UnSignContains(searchKey)) || (!string.IsNullOrWhiteSpace(inv.InvoiceNumber) && inv.InvoiceNumber.UnSignContains(searchKey))) && inv.IsActif == 1)
                .OrderByDescending(inv => inv.IDate).ThenByDescending(inv => inv.IDueDate).ThenByDescending(inv => inv.EditDate).ThenByDescending(inv => inv.AddDate);

            if (limit > 0)
                result = result.Take(limit);

            foreach (var item in result)
            {
                GetRelations(item);
                GetListInvoiceProducts(item);
            }

            return Task.FromResult(result.ToList());
        }

        public Task<Invoice> GetInvoiceDetail(Guid id)
        {
            if (App.LocalDb.Table<Invoice>().ToList().Find(i => !i.Id.Equals(Guid.Empty) && !id.Equals(Guid.Empty) && i.Id.Equals(id)) is Invoice invoice)
            {
                GetRelations(invoice);
                GetListInvoiceProducts(invoice);

                return Task.FromResult(invoice);
            }
            return null;
        }

        private void GetRelations(Invoice invoice)
        {
            invoice.Client = App.LocalDb.Table<Client>().FirstOrDefault(c => c.UserId == CurrentUser.Id && ((c.ServerId > 0 && c.ServerId == invoice.FkClientServerId) || (!c.Id.Equals(Guid.Empty) && !invoice.FkClientAppId.Equals(Guid.Empty) && c.Id.Equals(invoice.FkClientAppId))) && c.IsActif == 1);
            invoice.Address = App.LocalDb.Table<Address>().FirstOrDefault(a => a.UserId == CurrentUser.Id && ((a.ServerId > 0 && a.ServerId == invoice.FkAddressServerId) || (!a.Id.Equals(Guid.Empty) && !invoice.FkAddressAppId.Equals(Guid.Empty) && a.Id.Equals(invoice.FkAddressAppId))) && a.IsActif == 1);
            invoice.Intervention = App.LocalDb.Table<Intervention>().FirstOrDefault(i => i.UserId == CurrentUser.Id && ((i.ServerId > 0 && i.ServerId == invoice.FkInterventionServerId) || (!i.Id.Equals(Guid.Empty) && !invoice.FkInterventionAppId.Equals(Guid.Empty) && i.Id.Equals(invoice.FkInterventionAppId))) && i.IsActif == 1);
        }

        private void GetListInvoiceProducts(Invoice invoice)
        {
            invoice.LinkInvoiceProducts = App.LocalDb.Table<InvoiceProduct>().ToList().FindAll(ip => ip.UserId == CurrentUser.Id && ip.PriceUnit > 0
                && ((ip.FkInvoiceServerId > 0 && ip.FkInvoiceServerId == invoice.ServerId) || (!ip.FKInvoiceAppId.Equals(Guid.Empty) && !invoice.Id.Equals(Guid.Empty) && ip.FKInvoiceAppId.Equals(invoice.Id)))
                && ip.IsActif == 1).ToObservableCollection();

            foreach (var item in invoice.LinkInvoiceProducts)
            {
                item.Product = App.LocalDb.Table<Product>().FirstOrDefault(p => p.UserId == CurrentUser.Id && ((p.ServerId > 0 && p.ServerId == item.FkProductServerId) || (!p.Id.Equals(Guid.Empty) && !item.FkProductAppId.Equals(Guid.Empty) && p.Id.Equals(item.FkProductAppId))) && p.IsActif == 1);
            }
        }
    }
}