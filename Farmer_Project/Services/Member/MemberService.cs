using Farmer_Project.Entities;
using Farmer_Project.Models.Member;
using Farmer_Project.Repositories.Member;
using Microsoft.Extensions.Logging;

namespace Farmer_Project.Services.Member
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ILogger<MemberService> _logger;

        public MemberService(IMemberRepository memberRepository, ILogger<MemberService> logger)
        {
            _memberRepository = memberRepository;
            _logger = logger;
        }

        // 搜尋會員
        public async Task<List<MemberInformation>> SearchAsync(string? keyword)
        {
            return await _memberRepository.SearchAsync(keyword);
        }

        // 依Id取得會員
        public async Task<MemberInformation?> GetByIdAsync(int id)
        {
            return await _memberRepository.GetByIdAsync(id);
        }

        // 新增會員
        public async Task<MemberResult> CreateAsync(string sort, string nickName, string account, string passwd)
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
                await _memberRepository.SaveAsync();
                return new MemberResult { Valid = true, Message = "新增成功！" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增會員失敗, 帳號: {Account}", account);
                return new MemberResult { Valid = true, Message = "新增失敗：" + ex.Message };
            }
        }

        // 更新會員
        public async Task<MemberResult> UpdateAsync(int id, string sort, string nickName, string account, string passwd)
        {
            var member = await _memberRepository.GetByIdAsync(id);
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
                await _memberRepository.SaveAsync();
                return new MemberResult { Valid = true, Message = "資料更新成功！" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新會員失敗, Id: {Id}", id);
                return new MemberResult { Valid = true, Message = "更新失敗：" + ex.Message };
            }
        }

        // 刪除會員
        public async Task DeleteAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member != null)
            {
                _memberRepository.Remove(member);
                await _memberRepository.SaveAsync();
            }
        }
    }
}
