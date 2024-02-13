dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:JsonSerializerIsReflectionEnabledByDefault=true /p:PublishTrimmed=true --output ".\Publish"
pause