// 會員刪除
document.querySelectorAll('.delete-btn').forEach(button => {
    button.addEventListener('click', function () {
        const memberId = this.getAttribute('data-id');

        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Member/Delete/' + memberId;

        document.body.appendChild(form);
        form.submit();
    });
});

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
