using Newtonsoft.Json;
using System;

namespace LogWork.Models.Response
{
    public class LoginResponse
    {
        [JsonProperty("u_id")]
        public int Id { get; set; }

        [JsonProperty("u_fk_account_id")]
        public int FkAccountId { get; set; }

        [JsonProperty("u_nonce")]
        public string Nonce { get; set; }

        [JsonProperty("u_uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("u_fk_filiale_id")]
        public string FkFilialeId { get; set; }

        [JsonProperty("u_fk_media_id_avatar")]
        public string FkMediaIdAvatar { get; set; }

        [JsonProperty("u_fk_team_id")]
        public string FkTeamId { get; set; }

        [JsonProperty("u_fk_disponibilite_id")]
        public string FkDisponibiliteId { get; set; }

        [JsonProperty("u_fk_fonction_id")]
        public string FkFonctionId { get; set; }

        [JsonProperty("u_fk_signature_id")]
        public string FkSignatureId { get; set; }

        [JsonProperty("u_code_id")]
        public string CodeId { get; set; }

        [JsonProperty("u_login")]
        public string Login { get; set; }

        [JsonProperty("u_rang")]
        public string Rang { get; set; }

        [JsonProperty("u_civilite")]
        public string Civilite { get; set; }

        [JsonProperty("u_prenom")]
        public string Prenom { get; set; }

        [JsonProperty("u_nom")]
        public string Nom { get; set; }

        [JsonProperty("u_email")]
        public string Email { get; set; }

        [JsonProperty("u_lang")]
        public string Lang { get; set; }

        [JsonProperty("u_phone")]
        public string Phone { get; set; }

        [JsonProperty("u_birthdate")]
        public DateTime? Birthdate { get; set; } = null;

        [JsonProperty("u_insee")]
        public string Insee { get; set; }

        [JsonProperty("u_color")]
        public string Color { get; set; }

        [JsonProperty("u_is_working_lundi")]
        public string IsWorkingLundi { get; set; }

        [JsonProperty("u_is_working_mardi")]
        public string IsWorkingMardi { get; set; }

        [JsonProperty("u_is_working_mercredi")]
        public string IsWorkingMercredi { get; set; }

        [JsonProperty("u_is_working_jeudi")]
        public string IsWorkingJeudi { get; set; }

        [JsonProperty("u_is_working_vendredi")]
        public string IsWorkingVendredi { get; set; }

        [JsonProperty("u_is_working_samedi")]
        public string IsWorkingSamedi { get; set; }

        [JsonProperty("u_is_working_dimanche")]
        public string IsWorkingDimanche { get; set; }

        [JsonProperty("u_week_working_hours")]
        public string WeekWorkingHours { get; set; }

        [JsonProperty("u_day_hour_start")]
        public string DayHourStart { get; set; }

        [JsonProperty("u_day_hour_end")]
        public string DayHourEnd { get; set; }

        [JsonProperty("u_taux_horaire")]
        public string TauxHoraire { get; set; }

        [JsonProperty("u_comment")]
        public string Comment { get; set; }

        [JsonProperty("u_inscription_date")]
        public DateTime? InscriptionDate { get; set; }

        [JsonProperty("u_inscription_date_gmt")]
        public DateTime? InscriptionDateGmt { get; set; }

        [JsonProperty("u_inscription_ip")]
        public string InscriptionIp { get; set; }

        [JsonProperty("u_inscription_nonce")]
        public string InscriptionNonce { get; set; }

        [JsonProperty("u_last_modif_date")]
        public DateTime? LastModifDate { get; set; }

        [JsonProperty("u_last_modif_date_gmt")]
        public DateTime? LastModifDateGmt { get; set; }

        [JsonProperty("u_last_modif_ip")]
        public string LastModifIp { get; set; }

        [JsonProperty("u_last_access_date")]
        public DateTime? LastAccessDate { get; set; }

        [JsonProperty("u_last_access_ip")]
        public string LastAccessIp { get; set; }

        [JsonProperty("u_is_favorite")]
        public string IsFavorite { get; set; }

        [JsonProperty("u_is_actif")]
        public string IsActif { get; set; }

        [JsonIgnore]
        public string FullName => (Prenom + " " + Nom)?.Trim();
    }
}