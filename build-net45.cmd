rem run builds for .NET4.5/Android
"C:\Program Files (x86)\NuGet\Visual Studio 2015\nuget.exe" restore "OpenNETCF.IO.Serial/OpenNETCF.IO.Serial.Net45.sln"
rem NET 4.5 build
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" "OpenNETCF.IO.Serial/OpenNETCF.IO.Serial.Net45.csproj" /t:Build /p:Configuration=Release /p:TargetFramework=v4.5
