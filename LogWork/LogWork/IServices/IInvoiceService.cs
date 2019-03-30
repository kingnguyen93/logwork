using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetQuotes(int offset = 0, int limit = 0);

        Task<List<Invoice>> GetInvoices(int offset = 0, int limit = 0);

        Task<List<Invoice>> SearchInvoices(string searchKey, int limit = 0);

        Task<Invoice> GetInvoiceDetail(Guid id);
    }
}