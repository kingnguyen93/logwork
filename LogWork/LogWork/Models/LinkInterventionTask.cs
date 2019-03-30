using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("LinkInterventionTask")]
    public class LinkInterventionTask : TinyModel
    {
        [JsonProperty("litUUID")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("litId")]
        public int ServerId { get; set; }

        [JsonProperty("litUserId")]
        public int UserId { get; set; }

        [JsonProperty("litFkTaskUUID")]
        public Guid FkTaskAppliId { get; set; }

        [JsonProperty("litFkTaskId")]
        public int FkTaskServerId { get; set; }

        [JsonProperty("litFkIntUUID")]
        public Guid FkInterventionAppliId { get; set; }

        [JsonProperty("litFkIntId")]
        public int FkInterventionServerId { get; set; }

        [JsonProperty("litPlanningToDo")]
        public int IsPlanningToDo { get; set; }

        [JsonProperty("litIsDone")]
        public int IsDone { get; set; }

        [JsonProperty("litPlanningMinute")]
        public int PlanningMinute { get; set; }

        [JsonProperty("litDoneMinute")]
        public int DoneMinute { get; set; }

        [JsonProperty("litNonce")]
        public string Nonce { get; set; }

        [JsonProperty("litComment")]
        public string Comment { get; set; }

        [JsonProperty("litOn")]
        public int IsActif { get; set; }

        [JsonProperty("litSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("litAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("litModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("litLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [Ignore]
        public Tasks Task { get; set; }

        public bool IsToSync { get; set; }

        public LinkInterventionTask()
        {
        }

        public LinkInterventionTask(LinkInterventionTaskResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            FkTaskServerId = response.FkTaskServerId;
            FkInterventionServerId = response.FkInterventionServerId;
            IsPlanningToDo = response.IsPlanningToDo;
            IsDone = response.IsDone;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
            PlanningMinute = response.PlanningMinute;
            DoneMinute = response.DoneMinute;
            Nonce = response.Nonce;
        }

        public void UpdateFromResponse(LinkInterventionTaskResponse response)
        {
            FkTaskServerId = response.FkTaskServerId;
            FkInterventionServerId = response.FkInterventionServerId;
            IsPlanningToDo = response.IsPlanningToDo;
            IsDone = response.IsDone;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
            PlanningMinute = response.PlanningMinute;
            DoneMinute = response.DoneMinute;
            Nonce = response.Nonce;
        }
    }
}