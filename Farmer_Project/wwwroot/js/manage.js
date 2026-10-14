// 通用函數：切換顯示區域
function toggleDisplay(activeId) {
    const sections = ["messageForm", "createForm", "memberTable", "FarmersInfoTable", "FarmersArticlesList"];
    sections.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.style.display = (id === activeId) ? "block" : "none";
        }
    });

    const welcomeMessage = document.getElementById("welcomeMessage");
    if (welcomeMessage) {
        if (activeId !== "welcomeMessage") {
            welcomeMessage.style.display = "none";
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // 檢查 URL 查詢參數，根據參數顯示對應區塊
    const urlParams = new URLSearchParams(window.location.search);
    const activeIdFromUrl = urlParams.get('show'); // 获取查询参数 show

    // 如果有查詢參數，則顯示對應區塊
    if (activeIdFromUrl) {
        toggleDisplay(activeIdFromUrl);
    }
    else
    {
        toggleDisplay("welcomeMessage");
    }

    // 添加點擊事件監聽器
    addClickListener("showActions", function () {
        toggleDisplay("memberTable");
    });

    addClickListener("messageBtn", function () {
        toggleDisplay("messageForm");
    });

    addClickListener("showCreateForm", function () {
        toggleDisplay("createForm");
    });

    addClickListener('FarmersInfo', function () {
        toggleDisplay('FarmersInfoTable');
    });

    addClickListener('FarmersArticle', function () {
        toggleDisplay('FarmersArticlesList');
    });
});

// 添加按鈕事件處理
function addClickListener(id, callback) {
    const element = document.getElementById(id);
    if (element) {
        element.addEventListener("click", callback);
    }
}

// 發送訊息處理
addClickListener("sendMessageBtn", function () {
    const messageInput = document.getElementById("Message");
    const message = messageInput ? messageInput.value : "";

    if (!message) {
        alert("請輸入有效的訊息內容！");
        return;
    }

    const sendButton = document.getElementById("sendMessageBtn");
    sendButton.disabled = true;
    sendButton.innerText = "發送中...";

    const xhr = new XMLHttpRequest();
    xhr.open("POST", "/Manage/SendEmail", true); // 更新為正確的 API 路徑
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    xhr.onload = function () {
        sendButton.disabled = false;
        sendButton.innerText = "發送郵件";

        if (xhr.status === 200) {
            alert("郵件發送成功！");
            if (messageInput) messageInput.value = "";
        } else {
            alert("郵件發送失敗：" + xhr.responseText);
        }
    };

    const data = "message=" + encodeURIComponent(message) + "&actionType=yes";
    xhr.send(data);
});





document.querySelectorAll('.delete-btn').forEach(button => {
    button.addEventListener('click', function () {
        // 根據按鈕的 data-id 屬性構建刪除表單
        const memberId = this.getAttribute('data-id');

        // 動態創建一個表單
        const form = document.createElement('form');
        form.method = 'POST';  // 設置表單提交方式為 POST
        form.action = '/Manage/Delete/' + memberId + '?show=memberTable';  // 在表單的 URL 上加入 show=memberTable 參數

        // 將表單添加到 body 並提交
        document.body.appendChild(form);
        form.submit();  // 提交表單來執行刪除
    });
});

