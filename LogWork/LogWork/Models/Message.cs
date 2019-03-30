using Newtonsoft.Json;
using LogWork.Models.Response;
using SQLite;
using System;
using Xamarin.Forms;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Message")]
    public class Message : BaseModel
    {
        [JsonProperty("mesId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("mesIdServer")]
        public int ServerId { get; set; }

        [JsonProperty("mesUserId")]
        public int UserId { get; set; }

        [JsonProperty("mesFkUserFromUUID")]
        public Guid FkUserAppliIdFrom { get; set; }

        [JsonProperty("mesFkUserIdFrom")]
        public int FkUserServerIdFrom { get; set; }

        [JsonProperty("mesFkUserToUUID")]
        public Guid FkUserAppliIdTo { get; set; }

        [JsonProperty("mesFkUserIdTo")]
        public int FkUserServerIdTo { get; set; }

        [JsonProperty("mesFkMesToUUID")]
        public Guid FkMessagerieAppliId { get; set; }

        [JsonProperty("mesFkMesTo")]
        public int FkMessagerieServerId { get; set; }

        [JsonProperty("mesTitle")]
        public string Title { get; set; }

        [JsonProperty("mesContenu")]
        public string Content { get; set; }

        [JsonProperty("mesIsFavorite")]
        public int IsFavorite { get; set; }

        [JsonProperty("mesIsDraft")]
        public int IsDraft { get; set; }

        [JsonProperty("mesIsRead")]
        public int IsRead { get; set; }

        [JsonProperty("mesIsDelete")]
        public int IsDelete { get; set; }

        [JsonProperty("mesOn")]
        public int IsActif { get; set; }

        [JsonProperty("mesSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("mesAddDate")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("mesModifOn")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("mesLastViewDate")]
        public DateTime? LastViewDate { get; set; }

        [Ignore]
        public User UserFrom { get; set; }

        [Ignore]
        public User UserTo { get; set; }

        [Ignore]
        public bool UserFromIsCurrentUser => UserFrom?.ServerId == Settings.CurrentUserId;

        [Ignore]
        public bool UserToIsCurrentUser => UserTo?.ServerId == Settings.CurrentUserId;

        [Ignore]
        public Color BackgroundColor => UserFromIsCurrentUser ? Color.DeepSkyBlue : Color.LightGray;

        [Ignore]
        public Color TextColor => UserFromIsCurrentUser ? Color.White : Color.Black;

        public bool IsToSync { get; set; }

        public override string[] ExcludedProperties
        {
            get
            {
                return new string[]
                {
                    nameof(UserFromIsCurrentUser),
                    nameof(UserToIsCurrentUser),
                    nameof(BackgroundColor),
                    nameof(TextColor)
                };
            }
        }

        public Message()
        {
        }

        public Message(MessageResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            ServerId = response.ServerId;
            AddDate = response.AddDate;
            FkUserServerIdFrom = response.FkUserServerIdFrom;
            FkUserServerIdTo = response.FkUserServerIdTo;
            FkMessagerieServerId = response.FkMessagerieServerId;
            Title = response.Title;
            Content = response.Content;
            IsFavorite = response.IsFavorite;
            IsDraft = response.IsDraft;
            IsRead = response.IsRead;
            IsDelete = response.IsDelete;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }

        public void UpdateFromResponse(MessageResponse response)
        {
            AddDate = response.AddDate;
            FkUserServerIdFrom = response.FkUserServerIdFrom;
            FkUserServerIdTo = response.FkUserServerIdTo;
            FkMessagerieServerId = response.FkMessagerieServerId;
            Title = response.Title;
            Content = response.Content;
            IsFavorite = response.IsFavorite;
            IsDraft = response.IsDraft;
            IsRead = response.IsRead;
            IsDelete = response.IsDelete;
            IsActif = response.IsActif;
            SynchronizationDate = response.SynchronizationDate ?? DateTime.Now;
        }
    }

    public class MessageByUser : TinyModel
    {
        public string Title { get; set; }

        public Message LastMessage { get; set; }
    }
}