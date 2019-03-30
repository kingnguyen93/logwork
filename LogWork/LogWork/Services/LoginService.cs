using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models.Response;
using System.Threading.Tasks;

namespace LogWork.Services
{
    public class LoginService : BaseService, ILoginService
    {
        public async Task<LoginResponse> Login(string account, string userName, string password)
        {
            var result = await restClient.GetStringAsync<LoginResponse>(ApiURI.URL_BASE(account) + ApiURI.URL_GET_LOGIN(userName, password));

            return await Task.FromResult(result);
        }
    }
}