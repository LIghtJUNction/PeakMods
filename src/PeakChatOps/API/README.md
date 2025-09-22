# PeakChatOps API

dotnet pack -c Release

dotnet nuget push ..\..\..\artifacts\package\release\PeakChatOps.API.1.0.4.nupkg --source https://api.nuget.org/v3/index.json --api-key xxx

