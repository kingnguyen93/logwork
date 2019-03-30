using LogWork.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogWork.IServices
{
    public interface IInterventionService
    {
        Task<List<Intervention>> GetInterventions(DateTime SelectedDate, bool showNotDone, int limit = 0);

        Task<List<Intervention>> GetNotAssignedInterventions(DateTime? SelectedDate, int offset = 0, int limit = 0);

        Task<Intervention> GetIntervention(Guid id);

        Task<Intervention> GetRelations(Intervention intervention);

        Task<List<Intervention>> GetHistory(int interventionId);

        Task<bool> AssignToMe(int interventionId);

        Task<bool> CalculateDoneTime(Intervention intervention);

        Task<bool> CreateIntervention(Intervention intervention);

        Task<bool> SaveIntervention(Intervention intervention);

        Task<bool> UpdateIntervention(Intervention intervention);
    }
}