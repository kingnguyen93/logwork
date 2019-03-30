using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Extensions;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Intervention")]
    public class Intervention : TinyModel
    {
        [JsonProperty("intId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("intIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("intUserId")]
        public int UserId { get; set; }

        [JsonProperty("intCode")]
        public int Code { get; set; }

        [JsonProperty("intFkParentUUID")]
        public Guid FkParentAppId { get; set; }

        [JsonProperty("intFkParentId")]
        public int FkParentServerlId { get; set; }

        [JsonProperty("intFkUserUUID")]
        public Guid FkUserAppId { get; set; }

        [JsonProperty("intFkUserId")]
        public int FkUserServerlId { get; set; }

        [JsonProperty("intFkClientUUID")]
        public Guid FkClientAppId { get; set; }

        [JsonProperty("intFkClientId")]
        public int FkClientServerId { get; set; }

        [JsonProperty("intFkAdresseUUID")]
        public Guid FkAddressAppId { get; set; }

        [JsonProperty("intFkAdresseId")]
        public int FkAddressServerId { get; set; }

        [JsonProperty("intFkCheminUUID")]
        public Guid FkCheminAppId { get; set; }

        [JsonProperty("intFkCheminId")]
        public int FkCheminServerId { get; set; }

        [JsonProperty("intFkMediaUUIDSignature")]
        public Guid FkMediaAppIdSignature { get; set; }

        [JsonProperty("intFkMediaIdSignature")]
        public int FkMediaServerIdSignature { get; set; }

        [JsonProperty("intFkFilialeUUID")]
        public Guid FkFilialeAppId { get; set; }

        [JsonProperty("intFkFilialeId")]
        public int FkFilialeServerId { get; set; }
        
        [JsonProperty("intFkContratUUID")]
        public Guid FkContratAppId { get; set; }

        [JsonProperty("intFkContratId")]
        public int FkContratServerId { get; set; }
        
        [JsonProperty("intNom")]
        public string Nom { get; set; }

        [JsonProperty("intSociete")]
        public string Societe { get; set; }

        [JsonProperty("intPriority")]
        public int Priority { get; set; }

        private DateTime? planningDateStart;
        [JsonProperty("intPlanningDate")]
        public DateTime? PlanningDateStart { get => planningDateStart; set => SetProperty(ref planningDateStart, value, onChanged: () => OnPropertyChanged(nameof(PlanningDate), nameof(PlanningDates), nameof(Dates))); }

        private DateTime? planningDateEnd;
        [JsonProperty("intPlanningDateEnd")]
        public DateTime? PlanningDateEnd { get => planningDateEnd; set => SetProperty(ref planningDateEnd, value, onChanged: () => OnPropertyChanged(nameof(PlanningDate), nameof(PlanningDates), nameof(Dates))); }

        [Ignore]
        [JsonIgnore]
        public string PlanningDate => JoinDate(PlanningDateStart, PlanningDateEnd);

        [Ignore]
        [JsonIgnore]
        public string PlanningDates => JoinDate(PlanningDateStart, PlanningDateEnd, false, "dd/MM/yyyy");

        private string planningHourStart;
        [JsonProperty("intPlanningHourStart")]
        public string PlanningHourStart { get => planningHourStart; set => SetProperty(ref planningHourStart, value, onChanged: () => OnPropertyChanged(nameof(PlanningTime), nameof(Times))); }

        private string planningHourEnd;
        [JsonProperty("intPlanningHourEnd")]
        public string PlanningHourEnd { get => planningHourEnd; set => SetProperty(ref planningHourEnd, value, onChanged: () => OnPropertyChanged(nameof(PlanningTime), nameof(Times))); }

        [Ignore]
        [JsonIgnore]
        public string PlanningTime => JoinTime(PlanningHourStart, PlanningHourEnd);

        private string planningHour;
        [JsonProperty("intPlanningHour")]
        public string PlanningHour { get => planningHour; set => SetProperty(ref planningHour, value); }

        [JsonProperty("intPlanningComment")]
        public string PlanningComment { get; set; }

        private int isDone;
        [JsonProperty("intIsDone")]
        public int IsDone { get => isDone; set => SetProperty(ref isDone, value, onChanged: () => OnPropertyChanged(nameof(Status), nameof(Dates), nameof(Times))); }

        [Ignore]
        [JsonIgnore]
        public string IsDoneText => IsDone == 1 ? TranslateExtension.GetValue("yes") : TranslateExtension.GetValue("no");

        [Ignore]
        [JsonIgnore]
        public string Status
        {
            get
            {
                switch (IsDone)
                {
                    case 1:
                        return TranslateExtension.GetValue("completed");

                    case 2:
                        return TranslateExtension.GetValue("in_progress");

                    default:
                        return TranslateExtension.GetValue("planning");
                }
            }
        }

        private DateTime? doneDateStart;
        [JsonProperty("intDoneDate")]
        public DateTime? DoneDateStart { get => doneDateStart; set => SetProperty(ref doneDateStart, value, onChanged: () => OnPropertyChanged(nameof(DoneDate), nameof(DoneDates), nameof(Dates))); }

        private DateTime? doneDateEnd;
        [JsonProperty("intDoneDateEnd")]
        public DateTime? DoneDateEnd { get => doneDateEnd; set => SetProperty(ref doneDateEnd, value, onChanged: () => OnPropertyChanged(nameof(DoneDate), nameof(DoneDates), nameof(Dates))); }

        [Ignore]
        [JsonIgnore]
        public string DoneDate => JoinDate(DoneDateStart, DoneDateEnd);

        [Ignore]
        [JsonIgnore]
        public string DoneDates => JoinDate(DoneDateStart, DoneDateEnd, false, "dd/MM/yyyy");

        private string doneHourStart;
        [JsonProperty("intDoneHourStart")]
        public string DoneHourStart { get => doneHourStart; set => SetProperty(ref doneHourStart, value, onChanged: () => OnPropertyChanged(nameof(DoneTime), nameof(Times))); }

        private string doneHourEnd;
        [JsonProperty("intDoneHourEnd")]
        public string DoneHourEnd { get => doneHourEnd; set => SetProperty(ref doneHourEnd, value, onChanged: () => OnPropertyChanged(nameof(DoneTime), nameof(Times))); }

        [Ignore]
        [JsonIgnore]
        public string DoneTime => JoinTime(DoneHourStart, DoneHourEnd);

        private string doneHour;
        [JsonProperty("intDoneHour")]
        public string DoneHour { get => doneHour; set => SetProperty(ref doneHour, value); }

        [JsonProperty("intDoneComment")]
        public string DoneComment { get; set; }

        [JsonProperty("intDoneLong")]
        public double DoneLongitude { get; set; }

        [JsonProperty("intDoneLat")]
        public double DoneLatitude { get; set; }

        [JsonProperty("intDoneAlt")]
        public double DoneAltitude { get; set; }

        [JsonProperty("intDoneLocM")]
        public string DoneLocalisationMethod { get; set; }

        [JsonProperty("intNonce")]
        public string Nonce { get; set; }

        [JsonProperty("intOn")]
        public int IsActif { get; set; }

        [JsonProperty("intSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("intAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("intModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("intLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        private int sendMail;
        [JsonProperty("intSendToClient")]
        public int SendMail { get => sendMail; set => SetProperty(ref sendMail, value); }

        private User user;
        [Ignore]
        public User User { get => user; set => SetProperty(ref user, value); }

        private Client client;
        [Ignore]
        public Client Client { get => client; set => SetProperty(ref client, value); }

        private Address address;
        [Ignore]
        public Address Address { get => address; set => SetProperty(ref address, value); }
        
        private Contract contract;
        [Ignore]
        public Contract Contract { get => contract; set => SetProperty(ref contract, value); }

        private ObservableCollection<Intervention> childInterventions;
        [Ignore]
        public ObservableCollection<Intervention> ChildInterventions { get => childInterventions; set => SetProperty(ref childInterventions, value); }

        private List<LinkInterventionTask> linkInterventionTasks;
        [Ignore]
        public List<LinkInterventionTask> LinkInterventionTasks { get => linkInterventionTasks; set => SetProperty(ref linkInterventionTasks, value); }

        private List<UniteLink> uniteLinks;
        [Ignore]
        public List<UniteLink> UniteLinks { get => uniteLinks; set => SetProperty(ref uniteLinks, value); }

        [Ignore]
        public ObservableCollection<LinkInterventionProduct> LinkInterventionProducts { get; set; }

        [Ignore]
        [JsonIgnore]
        public decimal TotalHC => LinkInterventionProducts?.Sum(p => p.TotalPrice) ?? 0;

        [Ignore]
        [JsonIgnore]
        public decimal TotalTTC => LinkInterventionProducts?.Sum(p => p.TotalPriceWithTax) ?? 0;

        [Ignore]
        public ObservableCollection<MediaLink> MediaLinks { get; set; }

        [Ignore]
        public ObservableCollection<LinkInterventionTask> PlanningTasks { get; set; }

        [Ignore]
        public ObservableCollection<LinkInterventionTask> DoneTasks { get; set; }

        [Ignore]
        [JsonIgnore]
        public string Dates => IsDone == 1 ? DoneDate : PlanningDate;

        [Ignore]
        [JsonIgnore]
        public string Times => IsDone == 1 ? DoneTime : PlanningTime;

        [Ignore]
        [JsonIgnore]
        public string DateTime => Dates + " " + Times;

        private string JoinDate(DateTime? d1, DateTime? d2, bool removeSameValue = true, string format = "dd MMM")
        {
            if (removeSameValue && d1.ToString().Equals(d2.ToString()))
                return d1?.ToString(format);
            string result = (d1?.ToString(format) + " - " + d2?.ToString(format))?.Trim();
            if (result.StartsWith("- "))
                result = result.Replace("- ", "");
            return result.Trim().Equals("-") ? "" : result;
        }

        private string JoinTime(string t1, string t2)
        {
            string result = (t1 + " - " + t2)?.Trim();
            if (result.StartsWith("- "))
                result = result.Replace("- ", "");
            return result.Trim().Equals("-") ? "" : result;
        }

        public bool IsToSync { get; set; }

        public Intervention()
        {
            PropertyChanged += Intervention_PropertyChanged;
        }

        private void Intervention_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PlanningDateStart)) || e.PropertyName.Equals(nameof(PlanningDateEnd)) || e.PropertyName.Equals(nameof(PlanningHourStart)) || e.PropertyName.Equals(nameof(PlanningHourEnd))
                || e.PropertyName.Equals(nameof(DoneDateStart)) || e.PropertyName.Equals(nameof(DoneDateEnd)) || e.PropertyName.Equals(nameof(DoneHourStart)) || e.PropertyName.Equals(nameof(DoneHourEnd)))
            {
                OnPropertyChanged(nameof(PlanningDate), nameof(PlanningDates), nameof(PlanningTime), nameof(DoneDate), nameof(DoneDates), nameof(DoneTime), nameof(Dates), nameof(Times), nameof(DateTime));
            }
        }

        public Intervention(InterventionResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            Code = response.Code;
            FkUserServerlId = response.FkUserServerlId;
            FkClientServerId = response.FkClientServerId;
            FkAddressServerId = response.FkAdresseServerId;
            FkCheminServerId = response.FkCheminServerId;
            Nom = response.Nom;
            Priority = response.Priority;
            IsDone = response.IsDone;
            IsActif = response.IsActif;
            FkParentServerlId = response.FkParentServerlId;
            PlanningDateStart = response.PlanningDateStart;
            PlanningDateEnd = response.PlanningDateEnd;
            PlanningHourStart = response.PlanningHourStart;
            PlanningHourEnd = response.PlanningHourEnd;
            PlanningHour = response.PlanningHour;
            PlanningComment = response.PlanningComment;
            DoneDateStart = response.DoneDateStart;
            DoneDateEnd = response.DoneDateEnd;
            DoneHourStart = response.DoneHourStart;
            DoneHourEnd = response.DoneHourEnd;
            DoneHour = response.DoneHour;
            DoneComment = response.DoneComment;
            DoneLongitude = response.DoneLongitude;
            DoneLatitude = response.DoneLatitude;
            DoneAltitude = response.DoneAltitude;
            EditDate = response.EditDate;
            FkFilialeServerId = response.FkFilialeServerId;
            FkContratServerId = response.FkContratServerId;
            Nonce = response.Nonce;
        }

        public void UpdateFromResponse(InterventionResponse response)
        {
            Code = response.Code;
            FkUserServerlId = response.FkUserServerlId;
            FkClientServerId = response.FkClientServerId;
            FkAddressServerId = response.FkAdresseServerId;
            FkCheminServerId = response.FkCheminServerId;
            Nom = response.Nom;
            Priority = response.Priority;
            IsDone = response.IsDone;
            IsActif = response.IsActif;
            FkParentServerlId = response.FkParentServerlId;
            PlanningDateStart = response.PlanningDateStart;
            PlanningDateEnd = response.PlanningDateEnd;
            PlanningHourStart = response.PlanningHourStart;
            PlanningHourEnd = response.PlanningHourEnd;
            PlanningHour = response.PlanningHour;
            PlanningComment = response.PlanningComment;
            DoneDateStart = response.DoneDateStart;
            DoneDateEnd = response.DoneDateEnd;
            DoneHourStart = response.DoneHourStart;
            DoneHourEnd = response.DoneHourEnd;
            DoneHour = response.DoneHour;
            DoneComment = response.DoneComment;
            DoneLongitude = response.DoneLongitude;
            DoneLatitude = response.DoneLatitude;
            DoneAltitude = response.DoneAltitude;
            EditDate = response.EditDate;
            FkFilialeServerId = response.FkFilialeServerId;
            FkContratServerId = response.FkContratServerId;
            Nonce = response.Nonce;
        }
    }
}