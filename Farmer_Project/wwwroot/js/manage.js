// 後台登出確認
(function () {
    var logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function () {
            if (confirm("確定要登出嗎？")) {
                document.getElementById('logoutForm').submit();
            }
        });
    }
})();
