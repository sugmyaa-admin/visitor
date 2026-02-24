# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Sufinn.Visitor.Core/Sufinn.Visitor.Core.csproj
RUN dotnet publish Sufinn.Visitor/Sufinn.Visitor.csproj -c Release -o /app/publish

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app

ENV DEBIAN_FRONTEND=noninteractive

# Fix timezone (buster archive repo)
RUN sed -i 's|http://deb.debian.org/debian|http://archive.debian.org/debian|g' /etc/apt/sources.list \
    && apt-get update -o Acquire::Check-Valid-Until=false \
    && apt-get install -y --no-install-recommends tzdata \
    && ln -snf /usr/share/zoneinfo/Asia/Kolkata /etc/localtime \
    && echo "Asia/Kolkata" > /etc/timezone \
    && dpkg-reconfigure -f noninteractive tzdata \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Sufinn.Visitor.dll"]
