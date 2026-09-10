FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY publish/ .
# .NET 8 容器镜像默认监听 8080（不再是 80）
EXPOSE 8080
ENTRYPOINT ["dotnet", "Zhaoxi.Manage.MentApi.dll"]
