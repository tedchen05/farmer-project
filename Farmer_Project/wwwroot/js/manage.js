// 需要確認的表單(刪除等)改用 SweetAlert2
document.addEventListener('submit', function (e) {
    const form = e.target.closest('form.js-confirm');
    if (!form) {
        return;
    }

    e.preventDefault();
    Swal.fire({
        title: form.dataset.confirm || '確定要執行嗎？',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        confirmButtonColor: '#d33'
    }).then(function (result) {
        if (result.isConfirmed) {
            form.submit();
        }
    });
}, true);

// 後台登出確認
(function () {
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function () {
            Swal.fire({
                title: '確定要登出嗎？',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: '登出',
                cancelButtonText: '取消'
            }).then(function (result) {
                if (result.isConfirmed) {
                    document.getElementById('logoutForm').submit();
                }
            });
        });
    }
})();
