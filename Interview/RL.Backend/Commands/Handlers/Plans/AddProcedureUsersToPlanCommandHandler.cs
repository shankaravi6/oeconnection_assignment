using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RL.Backend.Commands.Handlers.Plans
{
    public class AddProcedureUsersToPlanCommandHandler : IRequestHandler<AddProcedureUsersToPlanCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AddProcedureUsersToPlanCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AddProcedureUsersToPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = await _context.Plans
                    .Include(p => p.PlanProcedures)
                    .ThenInclude(pp => pp.PlanProcedureUsers)
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);

                if (plan == null)
                {
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
                }

                var procedure = await _context.Procedures
                    .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId, cancellationToken);

                if (procedure == null)
                {
                    return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));
                }

                var userIds = request.Users.Split('-').Select(int.Parse).ToList();

                var planProcedure = plan.PlanProcedures.FirstOrDefault(pp => pp.ProcedureId == request.ProcedureId);

                if (planProcedure == null)
                {
                    planProcedure = new PlanProcedure
                    {
                        PlanId = request.PlanId,
                        ProcedureId = request.ProcedureId,
                        PlanProcedureUsers = new List<PlanProcedureUser>(),
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    };
                    plan.PlanProcedures.Add(planProcedure);
                }

                var existingUserIds = planProcedure.PlanProcedureUsers.Select(ppu => ppu.UserId).ToHashSet();
                var newPlanProcedureUsers = userIds
                    .Where(userId => !existingUserIds.Contains(userId))
                    .Select(userId => new PlanProcedureUser
                    {
                        PlanId = request.PlanId,
                        ProcedureId = request.ProcedureId,
                        UserId = userId,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow
                    })
                    .ToList();

                if (planProcedure.PlanProcedureUsers == null)
                {
                    planProcedure.PlanProcedureUsers = new List<PlanProcedureUser>();
                }

                planProcedure.PlanProcedureUsers.AddRange(newPlanProcedureUsers);

                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }
    }
}
