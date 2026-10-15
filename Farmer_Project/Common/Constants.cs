namespace Farmer_Project.Common
{
    // Session 鍵名
    public static class SessionKeys
    {
        public const string IsLoggedIn = "IsLoggedIn";
        public const string UserName = "UserName";
        public const string UserRole = "UserRole";
        public const string ResetPwdUserId = "ResetPwdUserId";
    }

    // 使用者角色(存於 Session)
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Member = "Member";
    }

    // 會員權限代碼(資料庫 Sort 欄位)
    public static class MemberSort
    {
        public const string Admin = "1";
        public const string Member = "0";
    }

    // 文章發佈動作
    public static class ArticleAction
    {
        public const string Publish = "true";
    }
}
