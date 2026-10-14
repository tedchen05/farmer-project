namespace Farmer_Project.Models.ViewModel
{
    public class AccountViewModel
    {
        public string account { get; set; }
        public string Nickname { get; set; }
        public string Passwd { get; set; }
    }
    public class LoginModel
    {
        public string account { get; set; }
        public string passwd { get; set; }
    }
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class forgetPasswd
    {
        public string account { get; set; }
        public string passwd { get; set; }
        public string passwd2 { get; set; }
    }
    public class register
    {
        public string sort { get; set; }
        public string nickName { get; set; }
        public string account { get; set; }
        public string passwd { get; set; }
    }
}
