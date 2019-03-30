using LogWork.Models.Response;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface ILoginService
    {
        Task<LoginResponse> Login(string account, string userName, string password);
    }
}