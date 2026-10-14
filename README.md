# 小農分享平台

以 ASP.NET Core 8 MVC 開發的小農產品分享平台，提供小農介紹、作物分類瀏覽、部落格文章與後台管理功能。

## 功能

### 前台
- 首頁、關於我們、農產品分類瀏覽
- 小農列表（依作物類型：咖啡、芒果等）
- 小農故事頁與地圖定位
- 部落格與部落格文章
- 使用者見證、聯絡我們（可寄送 Email）

### 會員
- 註冊、登入、登出
- 忘記密碼、重設密碼
- 會員資料修改

### 後台管理
- 會員管理（新增、編輯、刪除）
- 小農基本資料 CRUD（含圖片上傳）
- 小農文章與段落 CRUD
- 寄送訊息給會員

## 技術

- ASP.NET Core 8 MVC
- Entity Framework Core 9（SQL Server LocalDB）
- MailKit / MimeKit（寄送 Email）
- Bootstrap、jQuery

## 安裝

需求：

- .NET 8 SDK
- SQL Server LocalDB（隨 Visual Studio 或 SQL Server Express 安裝）

取得原始碼：

```bash
git clone <repository-url>
cd Farmer_Project
```

## 資料庫連線設定

連線字串設定於 `Farmer_Project/appsettings.json` 的 `ConnectionStrings:DefaultConnection`，
使用 SQL Server 帳號登入（`User Id` / `Password`）。

本機開發使用 LocalDB，需先在資料庫建立對應的登入帳號，例如：

```sql
CREATE LOGIN admin WITH PASSWORD = '12345678', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;
USE Farmers;
CREATE USER admin FOR LOGIN admin;
ALTER ROLE db_owner ADD MEMBER admin;
```

部署時將連線字串的伺服器位址與帳密改為正式資料庫即可。

## 執行

1. 依上一節設定連線字串與登入帳號。
2. 建立資料庫（套用 Migration）：

   ```bash
   cd Farmer_Project
   dotnet ef database update
   ```

3. 啟動：

   ```bash
   dotnet run
   ```

4. 瀏覽器開啟 `http://localhost:5067`。

## 使用方式

- 前台可直接瀏覽小農、作物與部落格內容。
- 一般會員登入後可修改個人資料。
- 管理者登入後自動導向後台（`/Farmers/DashBoard`），管理小農與會員資料。
- 會員權限以 `MemberInformations.Sort` 區分：`0` 一般會員、`1` 管理者。

## 常見問題

**Q：啟動後出現資料庫連線錯誤？**
A：確認 LocalDB 已安裝並啟動、`appsettings.json` 連線字串正確、且已建立對應的登入帳號；首次執行需先跑 `dotnet ef database update` 建立資料表。

**Q：圖片上傳後找不到？**
A：上傳的圖片存放於 `wwwroot/images`，檔名以內容的 MD5 命名；請確認該資料夾存在且有寫入權限。

**Q：寄送 Email 失敗？**
A：寄信設定位於 `Helpers/EmailHelper.cs`，需填入正確的 SMTP 帳號與授權碼。
