# ----------------------------------------------------------------------
# O2ORef.bash - 自動設定 O2OBackend 專案引用關係的腳本
# 請確保在執行此腳本前，您已在當前目錄下創建了 O2OBackend 資料夾
# 且 O2OBackend 內部已包含所有空的專案資料夾和 .csproj 檔案。
# 例如： O2OBackend/O2OBackend.Api/O2OBackend.Api.csproj 等。
# ----------------------------------------------------------------------

# 進入 O2OBackend 目錄
#cd O2OBackend

# 1. O2OBackend.Api 專案的引用
# ----------------------------------------------------------------------
# O2OBackend.Api 依賴 O2OBackend.Application
# O2OBackend.Api 依賴 O2OBackend.Shared (如果有通用 DTOs 或 helper)
echo "Adding references for O2OBackend.Api..."
cd O2OBackend.Api
dotnet add reference ../O2OBackend.Application/O2OBackend.Application.csproj
dotnet add reference ../O2OBackend.Shared/O2OBackend.Shared.csproj
cd .. # 返回 O2OBackend 目錄


# 2. O2OBackend.Application 專案的引用
# ----------------------------------------------------------------------
# O2OBackend.Application 依賴 O2OBackend.Domain (使用 Domain Entities, Interfaces)
# O2OBackend.Application 依賴 O2OBackend.Infrastructure (使用 Infrastructure 提供的 Repository 介面等，但只依賴介面)
# O2OBackend.Application 依賴 O2OBackend.Shared
echo "Adding references for O2OBackend.Application..."
cd O2OBackend.Application
dotnet add reference ../O2OBackend.Domain/O2OBackend.Domain.csproj
dotnet add reference ../O2OBackend.Infrastructure/O2OBackend.Infrastructure.csproj
dotnet add reference ../O2OBackend.Shared/O2OBackend.Shared.csproj
cd .. # 返回 O2OBackend 目錄


# 3. O2OBackend.Domain 專案的引用
# ----------------------------------------------------------------------
# O2OBackend.Domain 不依賴任何其他專案
# 但如果 Shared 專案包含 Domain 層也需要用的通用 Enum 或 Value Object，則可引用。
echo "Adding references for O2OBackend.Domain..."
cd O2OBackend.Domain
dotnet add reference ../O2OBackend.Shared/O2OBackend.Shared.csproj # 如果 Shared 包含 Domain 需要的項目
cd .. # 返回 O2OBackend 目錄


# 4. O2OBackend.Infrastructure 專案的引用
# ----------------------------------------------------------------------
# O2OBackend.Infrastructure 依賴 O2OBackend.Domain (實作 Domain 的 Entities 和 Repositories)
# O2OBackend.Infrastructure 依賴 O2OBackend.Shared
echo "Adding references for O2OBackend.Infrastructure..."
cd O2OBackend.Infrastructure
dotnet add reference ../O2OBackend.Domain/O2OBackend.Domain.csproj
dotnet add reference ../O2OBackend.Shared/O2OBackend.Shared.csproj
cd .. # 返回 O2OBackend 目錄


# 5. O2OBackend.Shared 專案的引用
# ----------------------------------------------------------------------
# O2OBackend.Shared 不依賴任何其他專案
echo "O2OBackend.Shared does not require any references."
cd O2OBackend.Shared
# 無需添加任何引用
cd .. # 返回 O2OBackend 目錄


echo "所有專案引用已設定完成，並已更新為使用 O2OBackend.Infrastructure。"
echo "您現在可以在 VS Code 中打開 O2OBackendSolution.sln 檢查結果。"
echo "code O2OBackendSolution.sln"
