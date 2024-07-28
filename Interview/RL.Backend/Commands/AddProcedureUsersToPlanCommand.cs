using MediatR;
using RL.Backend.Models;
using System;

namespace RL.Backend.Commands
{
    public class AddProcedureUsersToPlanCommand : IRequest<ApiResponse<Unit>>
    {
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }
        public string Users { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
