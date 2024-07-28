using MediatR;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RL.Backend.Commands
{
    public class CreatePlanCommand : IRequest<ApiResponse<Plan>>
    {
        public int PlanId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public class Handler : IRequestHandler<CreatePlanCommand, ApiResponse<Plan>>
        {
            private readonly RLContext _dbContext;

            public Handler(RLContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<ApiResponse<Plan>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var plan = new Plan
                    {
                        PlanId = request.PlanId,
                        CreateDate = request.CreateDate,
                        UpdateDate = request.UpdateDate
                    };

                    _dbContext.Plans.Add(plan);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return ApiResponse<Plan>.Succeed(plan);
                }
                catch (Exception ex)
                {
                    return ApiResponse<Plan>.Fail(ex);
                }
            }
        }
    }
}
