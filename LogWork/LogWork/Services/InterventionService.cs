using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LogWork.Constants;
using LogWork.IServices;
using LogWork.Models;
using LogWork.Models.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Extensions;

namespace LogWork.Services
{
    public class InterventionService : BaseService, IInterventionService
    {
        public Task<List<Intervention>> GetInterventions(DateTime SelectedDate, bool showNotDone, int limit = 0)
        {
            var assigned = App.LocalDb.Table<Intervention>().ToList().FindAll(i => i.UserId == CurrentUser.Id && (i.FkUserServerlId == CurrentUser.Id || (!i.FkUserAppId.Equals(Guid.Empty) && i.FkUserAppId.Equals(CurrentUser.Uuid)))
                                                                                && (!i.FkClientAppId.Equals(Guid.Empty) || i.FkClientServerId != 0) && i.IsActif == 1);
            IEnumerable<Intervention> unfinished;
            if (showNotDone)
                unfinished = assigned.FindAll(i => i.IsDone == 0 && ((i.PlanningDateStart.HasValue && i.PlanningDateEnd.HasValue && i.PlanningDateStart.Value.Date <= SelectedDate.Date && SelectedDate.Date <= i.PlanningDateEnd.Value.Date)
                                                                    || ((i.PlanningDateStart.HasValue || i.PlanningDateEnd.HasValue || i.EditDate.HasValue || i.AddDate.HasValue) ? ((i.PlanningDateStart.HasValue && i.PlanningDateStart.Value.Date <= SelectedDate.Date)
                                                                    || (i.PlanningDateEnd.HasValue && i.PlanningDateEnd.Value.Date <= SelectedDate.Date) || (i.AddDate.HasValue && i.AddDate.Value.Date <= SelectedDate.Date) || (i.EditDate.HasValue && i.EditDate.Value.Date <= SelectedDate.Date)) : true)))
                                                                    .OrderByDescending(i => i.PlanningDateStart).ThenByDescending(i => i.PlanningDateEnd).ThenBy(i => i.AddDate).ThenBy(i => i.EditDate).ThenBy(i => i.PlanningHourStart).ThenBy(i => i.PlanningHourEnd);
            else
                unfinished = assigned.FindAll(i => i.IsDone == 0 && ((i.PlanningDateStart.HasValue && i.PlanningDateEnd.HasValue && i.PlanningDateStart.Value.Date <= SelectedDate.Date && SelectedDate.Date <= i.PlanningDateEnd.Value.Date)
                                                                    || (i.PlanningDateStart.HasValue && i.PlanningDateStart.Value.Date == SelectedDate.Date) || (i.PlanningDateEnd.HasValue && i.PlanningDateEnd.Value.Date == SelectedDate.Date)
                                                                    || (i.AddDate.HasValue && i.AddDate.Value.Date == SelectedDate.Date) || (i.EditDate.HasValue && i.EditDate.Value.Date == SelectedDate.Date)))
                                                                    .OrderByDescending(i => i.PlanningDateStart).ThenByDescending(i => i.PlanningDateEnd).ThenBy(i => i.AddDate).ThenBy(i => i.EditDate).ThenBy(i => i.PlanningHourStart).ThenBy(i => i.PlanningHourEnd);

            var finished = assigned.FindAll(i => i.IsDone == 1 && ((i.DoneDateStart.HasValue && i.DoneDateEnd.HasValue && i.DoneDateStart.Value.Date <= SelectedDate.Date && SelectedDate.Date <= i.DoneDateEnd.Value.Date)
                                                                    || (i.DoneDateStart.HasValue && i.DoneDateStart.Value.Date == SelectedDate.Date) || (i.DoneDateEnd.HasValue && i.DoneDateEnd.Value.Date == SelectedDate.Date) || (i.EditDate.HasValue && i.EditDate.Value.Date == SelectedDate.Date)))
                                                                    .OrderByDescending(i => i.DoneDateStart).ThenByDescending(i => i.DoneDateEnd).ThenBy(i => i.AddDate).ThenBy(i => i.EditDate).ThenBy(i => i.DoneHourStart).ThenBy(i => i.DoneHourEnd);

            var result = unfinished.Union(finished);

            if (limit > 0)
                result = result.Take(limit);

            foreach (var item in result)
            {
                GetClientAddress(item);
            }

            return Task.FromResult(result.ToList());
        }

        public Task<List<Intervention>> GetNotAssignedInterventions(DateTime? SelectedDate = null, int offset = 0, int limit = 0)
        {
            IEnumerable<Intervention> result = null;

            if (SelectedDate.HasValue)
            {
                result = App.LocalDb.Table<Intervention>().ToList().FindAll(i => i.UserId == CurrentUser.Id && (i.FkUserAppId.Equals(Guid.Empty) && i.FkUserServerlId == 0) && (!i.FkClientAppId.Equals(Guid.Empty) || i.FkClientServerId != 0)
                                                                            && ((i.PlanningDateStart.HasValue && i.PlanningDateEnd.HasValue && i.PlanningDateStart.Value.Date <= SelectedDate?.Date && SelectedDate?.Date <= i.PlanningDateEnd.Value.Date)
                                                                                || ((i.PlanningDateStart.HasValue || i.PlanningDateEnd.HasValue || i.EditDate.HasValue) ? ((i.PlanningDateStart.HasValue && i.PlanningDateStart.Value.Date <= SelectedDate?.Date)
                                                                                || (i.PlanningDateEnd.HasValue && i.PlanningDateEnd.Value.Date == SelectedDate?.Date) || (i.EditDate.HasValue && i.EditDate.Value.Date <= SelectedDate?.Date)) : true))
                                                                            && i.IsActif == 1)
                                                                            .OrderByDescending(i => i.PlanningDateStart).ThenByDescending(i => i.PlanningDateEnd).ThenBy(i => i.PlanningHourStart).ThenBy(i => i.PlanningHourEnd);
            }
            else
            {
                result = App.LocalDb.Table<Intervention>().ToList().FindAll(i => i.UserId == CurrentUser.Id && (i.FkUserAppId.Equals(Guid.Empty) && i.FkUserServerlId == 0) && (!i.FkClientAppId.Equals(Guid.Empty) || i.FkClientServerId != 0) && i.IsActif == 1)
                                                                            .OrderByDescending(i => i.PlanningDateStart).ThenByDescending(i => i.PlanningDateEnd).ThenBy(i => i.PlanningHourStart).ThenBy(i => i.PlanningHourEnd);
            }

            if (offset > 0)
                result = result.Skip(offset);

            if (limit > 0)
                result = result.Take(limit);

            foreach (var item in result)
            {
                GetClientAddress(item);
            }

            return Task.FromResult(result.ToList());
        }

        public Task<Intervention> GetIntervention(Guid id)
        {
            if (App.LocalDb.Table<Intervention>().ToList().Find(i => !i.Id.Equals(Guid.Empty) && !id.Equals(Guid.Empty) && i.Id.Equals(id)) is Intervention intervention)
            {
                GetClientAddress(intervention);
                intervention = GetRelations(intervention).Result;
                return Task.FromResult(intervention);
            }
            return null;
        }

        private void GetClientAddress(Intervention intervention)
        {
            intervention.Client = App.LocalDb.Table<Client>().FirstOrDefault(c => c.UserId == CurrentUser.Id && ((c.ServerId > 0 && c.ServerId == intervention.FkClientServerId) || (!c.Id.Equals(Guid.Empty) && !intervention.FkClientAppId.Equals(Guid.Empty) && c.Id.Equals(intervention.FkClientAppId))) && c.IsActif == 1);
            intervention.Address = App.LocalDb.Table<Address>().FirstOrDefault(a => a.UserId == CurrentUser.Id && ((a.ServerId > 0 && a.ServerId == intervention.FkAddressServerId) || (!a.Id.Equals(Guid.Empty) && !intervention.FkAddressAppId.Equals(Guid.Empty) && a.Id.Equals(intervention.FkAddressAppId))) && a.IsActif == 1);
            intervention.Contract = App.LocalDb.Table<Contract>().FirstOrDefault(con => con.UserId == CurrentUser.Id && ((con.ServerId > 0 && con.ServerId == intervention.FkContratServerId) || (!con.Id.Equals(Guid.Empty) && !intervention.FkContratAppId.Equals(Guid.Empty) && con.Id.Equals(intervention.FkContratAppId))) && con.IsActif == 1);
        }

        public Task<Intervention> GetRelations(Intervention intervention)
        {
            intervention.User = App.LocalDb.Table<User>().ToList().FirstOrDefault(user => user.UserId == CurrentUser.Id && ((intervention.FkUserServerlId > 0 && intervention.FkUserServerlId == user.ServerId) || (!intervention.FkUserAppId.Equals(Guid.Empty) && !user.Id.Equals(Guid.Empty) && intervention.FkUserAppId.Equals(user.Id))) && user.IsActif == 1);

            intervention.ChildInterventions = App.LocalDb.Table<Intervention>().ToList().FindAll(inter => inter.UserId == CurrentUser.Id && ((inter.FkParentServerlId > 0 && inter.FkParentServerlId == intervention.ServerId) || (!inter.FkParentAppId.Equals(Guid.Empty) && inter.FkParentAppId.Equals(intervention.Id))) && inter.IsActif == 1).ToObservableCollection();

            foreach (var child in intervention.ChildInterventions)
            {
                child.User = App.LocalDb.Table<User>().ToList().FirstOrDefault(user => user.UserId == CurrentUser.Id && ((child.FkUserServerlId > 0 && child.FkUserServerlId == user.ServerId) || (!child.FkUserAppId.Equals(Guid.Empty) && !user.Id.Equals(Guid.Empty) && child.FkUserAppId.Equals(user.Id))) && user.IsActif == 1);
            }

            intervention.LinkInterventionTasks = App.LocalDb.Table<LinkInterventionTask>().ToList().FindAll(lit => lit.UserId == CurrentUser.Id && ((lit.FkInterventionServerId > 0 && lit.FkInterventionServerId == intervention.ServerId) || (!lit.FkInterventionAppliId.Equals(Guid.Empty) && lit.FkInterventionAppliId.Equals(intervention.Id))) && lit.IsActif == 1);

            foreach (var lit in intervention.LinkInterventionTasks)
            {
                lit.Task = App.LocalDb.Table<Tasks>().ToList().FirstOrDefault(ta => ta.UserId == CurrentUser.Id && ((lit.FkTaskServerId > 0 && lit.FkTaskServerId == ta.ServerId) || (!lit.FkTaskAppliId.Equals(Guid.Empty) && !ta.Id.Equals(Guid.Empty) && lit.FkTaskAppliId.Equals(ta.Id))) && ta.IsActif == 1);
            }

            intervention.UniteLinks = App.LocalDb.Table<UniteLink>().ToList().FindAll(unl => unl.UserId == CurrentUser.Id && ((unl.FkColumnServerId > 0 && unl.FkColumnServerId == intervention.ServerId) || (!unl.FkColumnAppliId.Equals(Guid.Empty) && unl.FkColumnAppliId.Equals(intervention.Id))) && unl.IsActif == 1);
            foreach (var unl in intervention.UniteLinks)
            {
                unl.Unite = App.LocalDb.Table<Unite>().ToList().FirstOrDefault(un => un.UserId == CurrentUser.Id && ((unl.FkUniteServerId > 0 && unl.FkUniteServerId == un.ServerId) || (!unl.FkUniteAppliId.Equals(Guid.Empty) && !un.Id.Equals(Guid.Empty) && unl.FkUniteAppliId.Equals(un.Id))) && un.IsActif == 1);

                if (unl.Unite != null)
                {
                    unl.UniteTitle = unl.Unite.Nom;
                    unl.UniteDisplay = (unl.Unite.FieldType == 3 && !string.IsNullOrWhiteSpace(unl.UniteValue)) ? unl.UniteValue.Equals("1") ? TranslateExtension.GetValue("yes") : TranslateExtension.GetValue("no") : unl.UniteDisplay = unl.UniteValue;
                }
            }

            intervention.LinkInterventionProducts = App.LocalDb.Table<LinkInterventionProduct>().ToList().FindAll(lip => lip.UserId == CurrentUser.Id && ((lip.FkInterventionServerId > 0 && lip.FkInterventionServerId == intervention.ServerId) || (!lip.FkInterventionAppliId.Equals(Guid.Empty) && lip.FkInterventionAppliId.Equals(intervention.Id))) && lip.IsActif == 1).ToObservableCollection();
            foreach (var lip in intervention.LinkInterventionProducts)
            {
                lip.Product = App.LocalDb.Table<Product>().ToList().FirstOrDefault(pro => pro.UserId == CurrentUser.Id && ((lip.FkProductServerId > 0 && lip.FkProductServerId == pro.ServerId) || (!lip.FkProductId.Equals(Guid.Empty) && !pro.Id.Equals(Guid.Empty) && lip.FkProductId.Equals(pro.Id))) && pro.IsActif == 1);
            }

            intervention.MediaLinks = App.LocalDb.Table<MediaLink>().ToList().FindAll(medl => medl.UserId == CurrentUser.Id && ((medl.FkColumnServerId > 0 && medl.FkColumnServerId == intervention.ServerId) || (!medl.FkColumnAppliId.Equals(Guid.Empty) && medl.FkColumnAppliId.Equals(intervention.Id))) && !medl.IsDelete && medl.IsActif == 1).ToObservableCollection();
            foreach (var medl in intervention.MediaLinks)
            {
                medl.Media = App.LocalDb.Table<Media>().ToList().FirstOrDefault(med => med.UserId == CurrentUser.Id && ((medl.FkMediaServerId > 0 && medl.FkMediaServerId == med.ServerId) || (!medl.FkMediaAppliId.Equals(Guid.Empty) && !med.Id.Equals(Guid.Empty) && medl.FkMediaAppliId.Equals(med.Id))) && med.IsActif == 1);
            }

            return Task.FromResult(intervention);
        }

        public async Task<List<Intervention>> GetHistory(int interventionId)
        {
            var response = await restClient.GetAsync(ApiURI.URL_BASE(Settings.CurrentAccount) + ApiURI.URL_GET_INTERVENTION_HISTORY(Settings.CurrentUserName, Settings.CurrentPassword, interventionId));

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<InterventionHistoryRespone>(content);
                if (result != null)
                {
                    List<Intervention> interventions = new List<Intervention>();
                    if (result.Interventions != null)
                    {
                        foreach (var intRes in result.Interventions)
                        {
                            interventions.Add(new Intervention(intRes));
                        }
                    }
                    return interventions;
                }
            }
            else
            {
                Debug.WriteLine("Get Intervention History failed: " + content);
                return null;
            }

            return null;
        }

        public async Task<bool> AssignToMe(int interventionId)
        {
            var response = await restClient.GetAsync(ApiURI.URL_BASE(Settings.CurrentAccount) + ApiURI.URL_SET_INTERVENTION_ASSIGN(Settings.CurrentUserName, Settings.CurrentPassword, interventionId));

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<JObject>(content);
                if (result != null)
                {
                    if (result.GetValue("MESSAGE") != null)
                    {
                        Debug.WriteLine("Assiggn Intervention successed: " + content);
                        return await Task.FromResult(true);
                    }
                    else if (result.GetValue("ERROR") != null)
                    {
                        Debug.WriteLine("Intervention already assigned: " + content);
                        return await Task.FromResult(false);
                    }
                }
                throw new Exception(content);
            }
            else
            {
                Debug.WriteLine("Assiggn Intervention failed: " + content);
                throw new Exception(content);
            }
        }

        public Task<bool> CalculateDoneTime(Intervention intervention)
        {
            try
            {
                switch (intervention.IsDone)
                {
                    case 0:
                        //intervention.DoneDateEnd = null;
                        //intervention.DoneHourEnd = "";
                        //intervention.DoneHour = "00:00";
                        break;

                    case 2:
                        break;

                    case 1:
                        intervention.DoneDateStart = intervention.DoneDateStart ?? DateTime.Now;
                        intervention.DoneDateEnd = DateTime.Now;

                        if (string.IsNullOrWhiteSpace(intervention.DoneHourStart))
                        {
                            if (Settings.LastDoneDate.Date == DateTime.Today && !string.IsNullOrWhiteSpace(Settings.LastDoneTime))
                            {
                                intervention.DoneHourStart = Settings.LastDoneTime;
                            }
                            else if (AppSettings.MobileHourStartEnable && TimeSpan.TryParse(AppSettings.MobileHourStartDefault, out TimeSpan defaultTime) && defaultTime <= DateTime.Now.TimeOfDay)
                            {
                                intervention.DoneHourStart = AppSettings.MobileHourStartDefault;
                            }
                            else
                            {
                                intervention.DoneHourStart = DateTime.Now.ToString("HH:mm");
                            }
                        }

                        intervention.DoneHourEnd = DateTime.Now.ToString("HH:mm");

                        Settings.LastDoneDate = intervention.DoneDateEnd.Value;
                        Settings.LastDoneTime = intervention.DoneHourEnd;

                        CalculateWorkingTime(intervention);

                        if (intervention.LinkInterventionTasks == null || intervention.LinkInterventionTasks.Count == 0)
                        {
                            intervention.LinkInterventionTasks = App.LocalDb.Table<LinkInterventionTask>().ToList().FindAll(lit => lit.UserId == CurrentUser.Id
                            && ((lit.FkInterventionServerId > 0 && lit.FkInterventionServerId == intervention.ServerId) || (!lit.FkInterventionAppliId.Equals(Guid.Empty) && lit.FkInterventionAppliId.Equals(intervention.Id))) && lit.IsActif == 1);
                        }

                        foreach (var lit in intervention.LinkInterventionTasks)
                        {
                            if (lit.DoneMinute == 0)
                            {
                                if (TimeSpan.TryParse(intervention.DoneHour, out TimeSpan workingTime))
                                {
                                    lit.DoneMinute = Convert.ToInt32(workingTime.TotalMinutes);
                                }
                                else
                                {
                                    lit.DoneMinute = lit.PlanningMinute;
                                }

                                lit.IsToSync = true;
                            }
                        }

                        break;
                }

                intervention.EditDate = DateTime.Now;
                intervention.IsToSync = true;

                UpdateIntervention(intervention);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Set Done failed: " + ex);
                return Task.FromResult(false);
            }
        }

        private void CalculateWorkingTime(Intervention intervention)
        {
            if (intervention.DoneDateStart.HasValue && intervention.DoneDateEnd.HasValue
                && TimeSpan.TryParse(string.IsNullOrWhiteSpace(intervention.DoneHourStart) ? "00:00" : intervention.DoneHourStart, out TimeSpan t1)
                && TimeSpan.TryParse(string.IsNullOrWhiteSpace(intervention.DoneHourEnd) ? "00:00" : intervention.DoneHourEnd, out TimeSpan t2))
            {
                var wt = intervention.DoneDateEnd.Value.Date.Add(t2).Subtract(intervention.DoneDateStart.Value.Date.Add(t1));
                if (wt.Ticks > 0)
                    intervention.DoneHour = string.Format("{0}:{1}", ((int)wt.TotalHours).ToString("00"), wt.Minutes.ToString("00"));
                else
                    intervention.DoneHour = "00:00";
            }
            else
            {
                intervention.DoneHour = "00:00";
            }
        }

        public Task<bool> CreateIntervention(Intervention intervention)
        {
            try
            {
                return Task.FromResult(App.LocalDb.Insert(intervention) > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Create intervention failed: " + ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> SaveIntervention(Intervention intervention)
        {
            try
            {
                return Task.FromResult(App.LocalDb.InsertOrReplace(intervention) > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Save intervention failed: " + ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateIntervention(Intervention intervention)
        {
            try
            {
                return Task.FromResult(App.LocalDb.Update(intervention) > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Update intervention failed: " + ex);
                return Task.FromResult(false);
            }
        }
    }
}