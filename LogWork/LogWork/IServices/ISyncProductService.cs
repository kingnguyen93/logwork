using System;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface ISyncProductService
    {
        Task<bool> SyncFromServer(int method, Action onSuccess, Action<string> onError = null, bool showOverlay = false);

        Task<bool> SyncFromServer(int method);
    }
}