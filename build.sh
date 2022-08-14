curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh;
chmod +x dotnet-install.sh;
./dotnet-install.sh -c 6.0 -InstallDir ./dotnet6;
ls;
./dotnet6/dotnet publish ./NearCompanion/NearCompanion.sln -c Release -o output;