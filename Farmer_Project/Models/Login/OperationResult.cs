namespace Farmer_Project.Models.Login
{
    // 操作結果對應的欄位(用於畫面顯示錯誤位置)
    public enum MemberField
    {
        None,
        NickName,
        Account,
        Passwd
    }

    // 一般操作結果
    public class OperationResult
    {
        public bool Success { get; private set; }
        public MemberField Field { get; private set; }
        public string? Message { get; private set; }

        private OperationResult(bool success, MemberField field, string? message)
        {
            Success = success;
            Field = field;
            Message = message;
        }

        // 成功
        public static OperationResult Ok(string? message = null)
        {
            return new OperationResult(true, MemberField.None, message);
        }

        // 失敗，指定出錯欄位與訊息
        public static OperationResult Fail(MemberField field, string message)
        {
            return new OperationResult(false, field, message);
        }
    }
}
