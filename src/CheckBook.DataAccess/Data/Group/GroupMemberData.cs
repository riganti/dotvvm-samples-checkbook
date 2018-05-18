namespace CheckBook.DataAccess.Data.Group
{
    public class GroupMemberData : IAvatarData
    {
        public int? UserId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public decimal Amount { get; set; }
    }
}