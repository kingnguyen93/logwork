using Newtonsoft.Json;
using LogWork.Constants;
using LogWork.Models.Response;
using SQLite;
using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Forms;
using TinyMVVM;

namespace LogWork.Models
{
    [Table("Media")]
    public class Media : BaseModel
    {
        [JsonProperty("mAppId")]
        [PrimaryKey]
        public Guid Id { get; set; }

        [JsonProperty("mId")]
        public int ServerId { get; set; }

        [JsonProperty("mUserId")]
        public int UserId { get; set; }

        [JsonProperty("mAccountId")]
        public int AccountId { get; set; }

        [JsonProperty("mCode")]
        public int Code { get; set; }

        [JsonProperty("mFilePath")]
        public string FilePath { get; set; }

        [JsonProperty("mFileName")]
        public string FileName { get; set; }

        [JsonProperty("mFileSize")]
        public int FileSize { get; set; }

        [JsonProperty("mFileMime")]
        public string FileMime { get; set; }

        [JsonProperty("mYear")]
        public string Year { get; set; }

        [JsonProperty("mMonth")]
        public string Month { get; set; }

        private string fileData;
        [JsonProperty("mFileData")]
        public string FileData { get => fileData; set => SetProperty(ref fileData, value, nameof(ImageSource), ImageUri, nameof(ImageDisplay)); }

        [Ignore]
        [JsonIgnore]
        public ImageSource ImageSource => string.IsNullOrWhiteSpace(FileData) ? null : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(FileData)));

        [Ignore]
        [JsonIgnore]
        public string ImageUri => ApiURI.URL_GET_MEDIA(Settings.CurrentAccount, Settings.CurrentUser.FkAccountId, AccountId, Year, Month, FileName);

        [Ignore]
        [JsonIgnore]
        public ImageSource ImageDisplay => ImageSource ?? ImageSource.FromUri(new Uri(ImageUri));

        [JsonProperty("mImageHeight")]
        public int ImageHeight { get; set; }

        [JsonProperty("mImageWidth")]
        public int ImageWidth { get; set; }

        [JsonProperty("mLegend")]
        public string Legend { get; set; }

        [JsonProperty("mComment")]
        public string Comment { get; set; }

        [JsonProperty("mActif")]
        public int IsActif { get; set; }

        [JsonProperty("mSynchronizationDate")]
        public DateTime? SynchronizationDate { get; set; }

        [JsonProperty("mCreatedOn")]
        public DateTime? AddDate { get; set; }

        [JsonProperty("mModifDate")]
        public DateTime? EditDate { get; set; }

        [JsonProperty("mLastViewDate")]
        public DateTime? LastViewDate { get; set; }
        
        public bool IsToSync { get; set; }

        public Media()
        {
            PropertyChanged += Media_PropertyChanged;
        }

        private void Media_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(FileData)))
            {
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public Media(MediaResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.K) && Guid.TryParse(response.K, out Guid id))
                Id = id;
            else
                Id = Guid.NewGuid();
            AddDate = response.AddDate;
            ServerId = response.ServerId;
            AccountId = response.AccountId;
            Code = response.Code;
            FileName = response.FileName;
            Year = response.Year;
            Month = response.Month;
            FileSize = response.FileSize;
            FileMime = response.FileMime;
            ImageWidth = response.ImageWidth;
            ImageHeight = response.ImageHeight;
            Legend = response.Legend;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }

        public void UpdateFromResponse(MediaResponse response)
        {
            Code = response.Code;
            FileName = response.FileName;
            Year = response.Year;
            Month = response.Month;
            FileSize = response.FileSize;
            FileMime = response.FileMime;
            ImageWidth = response.ImageWidth;
            ImageHeight = response.ImageHeight;
            Legend = response.Legend;
            IsActif = response.IsActif;
            EditDate = response.EditDate;
        }
    }
}