# 固定到补丁版本：浮动标签 8.0 每月更新，每次都会让下一次发布重新跨境推送 86MB 基础层
# 升级运行时 = 改这里的版本号，按正常流程发布一次
FROM mcr.microsoft.com/dotnet/aspnet:8.0.31
WORKDIR /app
COPY publish/ .
# .NET 8 容器镜像默认监听 8080（不再是 80）
EXPOSE 8080
ENTRYPOINT ["dotnet", "Zhaoxi.Manage.MentApi.dll"]
