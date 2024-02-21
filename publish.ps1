dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:JsonSerializerIsReflectionEnabledByDefault=false /p:PublishTrimmed=true --output ".\Publish"
pause