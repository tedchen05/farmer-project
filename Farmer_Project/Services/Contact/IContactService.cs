using Farmer_Project.Models.Contact;

namespace Farmer_Project.Services.Contact
{
    // 聯絡我們商業邏輯
    public interface IContactService
    {
        bool SendAcknowledgement(ContactMessage model);
    }
}
