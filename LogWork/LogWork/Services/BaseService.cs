using LogWork.Models.Response;

namespace LogWork.Services
{
    public class BaseService
    {
        protected readonly IRestClient restClient;
        
        protected readonly LoginResponse CurrentUser = Settings.CurrentUser;
        protected readonly string CurrentAccount = Settings.CurrentAccount;
        protected readonly string CurrentUserName = Settings.CurrentUserName;
        protected readonly string CurrentPassword = Settings.CurrentPassword;

        public BaseService()
        {
            restClient = TinyIoC.TinyIoCContainer.Current.Resolve<IRestClient>();
        }
    }
}