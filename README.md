# Duplicate SCPs (LabAPI Plugin)

這個 Repo 內含一個 LabAPI 插件範例，用於在回合開始時計算玩家人數，並額外生成隨機的「重複 SCP」。

## 功能概述

- 回合開始時檢查玩家總數。
- 若達到設定門檻，額外生成 1 隻隨機重複 SCP。
- 若超過門檻後每增加指定人數，再追加 1 隻重複 SCP。
- 可排除不參與重複的 SCP 類型。
- 直接在回合開始時指派角色，不受 `team_respawn_queue` 影響。

## 專案結構

```
DuplicateScpPlugin/
  DuplicateScpPlugin.csproj
  src/
    DuplicateScpConfig.cs
    DuplicateScpPlugin.cs
```

## 安裝與編譯

1. 進入專案資料夾：
   ```bash
   cd DuplicateScpPlugin
   ```
2. 還原並編譯：
   ```bash
   dotnet build
   ```
3. 將輸出的 DLL 放入伺服器的 `LabAPI/plugins` 目錄。

## Config 設定說明

插件使用 `DuplicateScpConfig` 設定：

- `IsEnabled`：是否啟用插件。
- `MinimumPlayerCount`：回合開始時玩家總數達到此數值時，生成第一隻重複 SCP。
- `AdditionalPlayersPerDuplicate`：超過門檻後，每增加多少玩家就再生成 1 隻重複 SCP。
- `ExcludedScps`：不參與重複生成的 SCP 列表。

範例設定：

```yaml
IsEnabled: true
MinimumPlayerCount: 40
AdditionalPlayersPerDuplicate: 10
ExcludedScps:
  - Scp079
```

## 行為邏輯

- 會從本局已存在的 SCP 類型中挑選一種來複製。
- 從非 SCP 玩家中隨機挑選目標，直接轉換成該 SCP。
- 若無可用 SCP 類型或無可轉換玩家，則不生成。

## 後續可擴充

- 新增最大重複 SCP 數量限制。
- 增加黑名單玩家或角色。
- 在 RoundStart 之外的特定事件觸發。
