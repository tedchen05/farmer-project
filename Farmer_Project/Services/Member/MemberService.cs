using Farmer_Project.Entities;
using Farmer_Project.Models.Member;
using Farmer_Project.Repositories.Member;

namespace Farmer_Project.Services.Member
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        // 搜尋會員
        public List<MemberInformation> Search(string? keyword)
        {
            return _memberRepository.Search(keyword);
        }

        // 依Id取得會員
        public MemberInformation? GetById(int id)
        {
            return _memberRepository.GetById(id);
        }

        // 新增會員
        public MemberResult Create(string sort, string nickName, string account, string passwd)
        {
            if (string.IsNullOrEmpty(sort) || string.IsNullOrEmpty(nickName) ||
                string.IsNullOrEmpty(account) || string.IsNullOrEmpty(passwd))
            {
                return new MemberResult { Valid = false, Message = "請填寫所有欄位。" };
            }

            try
            {
                _memberRepository.Add(new MemberInformation
                {
                    Sort = sort,
                    NickName = nickName,
                    Account = account,
                    Passwd = passwd
                });
                _memberRepository.Save();
                return new MemberResult { Valid = true, Message = "新增成功！" };
            }
            catch (Exception ex)
            {
                return new MemberResult { Valid = true, Message = "新增失敗：" + ex.Message };
            }
        }

        // 更新會員
        public MemberResult Update(int id, string sort, string nickName, string account, string passwd)
        {
            var member = _memberRepository.GetById(id);
            if (member == null)
            {
                return new MemberResult { Valid = true, Message = "找不到該會員資料" };
            }

            member.Sort = sort;
            member.NickName = nickName;
            member.Account = account;
            member.Passwd = passwd;

            try
            {
                _memberRepository.Save();
                return new MemberResult { Valid = true, Message = "資料更新成功！" };
            }
            catch (Exception ex)
            {
                return new MemberResult { Valid = true, Message = "更新失敗：" + ex.Message };
            }
        }

        // 刪除會員
        public void Delete(int id)
        {
            var member = _memberRepository.GetById(id);
            if (member != null)
            {
                _memberRepository.Remove(member);
                _memberRepository.Save();
            }
        }
    }
}
