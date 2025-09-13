namespace DidMark.Core.DTO.Roles
{
    public class UserRoleAssignDTO
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
    public enum UserRoleAssignResult
    {
        Success,
        AlreadyHasRole,
        Notfound
    }
    public enum UserRoleRemoveResult
    {
        Success,
        DoesNotHaveRole,
        Notfound
    }
}
