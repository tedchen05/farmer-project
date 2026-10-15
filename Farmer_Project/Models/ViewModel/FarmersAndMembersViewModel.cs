namespace Farmer_Project.Entities
{
    public class FarmersAndMembersViewModel
    {
        public List<FarmersInfo> FarmersInfos { get; set; } = new List<FarmersInfo>();
        public List<MemberInformation> MemberInformations { get; set; } = new List<MemberInformation>();
        public string SearchTerm { get; set; } = string.Empty;
    }
}

