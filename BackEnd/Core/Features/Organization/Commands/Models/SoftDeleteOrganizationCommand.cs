public record SoftDeleteOrganizationCommand(
    Guid OrganizationId,
    Guid OwnerId
) : IRequest<Response<string>>;