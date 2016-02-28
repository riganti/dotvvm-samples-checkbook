
namespace CheckBook.DataAccess.Enums
{
    public enum UserRole
    {
        /// <summary>
        /// A user who can view payments in groups he belongs to, and who can add and edit his own payments.
        /// </summary>
        User,

        /// <summary>
        /// A user who can manage which user belongs into which group and who can modify payments of other people.
        /// </summary>
        Admin
    }
}
