using System;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface ISyncDataService
    {
        // method: 0: unknown, 1: auto, 2: manual
        Task<bool> SyncFromServer(int method, Action onSuccess, Action<string> onError = null, bool showOverlay = false);
        
        Task<bool> SyncFromServer(int method);

        Task<bool> SyncToServer(int method);

        void SyncMediaToServer(int method = 1);
    }
}