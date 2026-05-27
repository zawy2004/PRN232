using BusinessObjects;

namespace DataAccessObjects
{
    public class AccountDAO
    {
        public static AccountMember GetAccountById(string accountID)
        {
            using var db = new MyStoreContext();
            return db.AccountMembers.FirstOrDefault(c => c.MemberId.Equals(accountID));
        }
    }
}
