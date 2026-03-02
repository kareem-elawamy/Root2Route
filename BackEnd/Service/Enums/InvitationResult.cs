namespace Service.Enums
{
    public enum InvitationResult
    {
        Success,
        NotFound,
        Expired,
        InvalidUser,
        AlreadyMember,
        InvalidRole,
        AlreadyInvited,
        OrganizationNotFound
    }
}