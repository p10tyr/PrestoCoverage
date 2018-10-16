dotnet test
coverlet .\bin\Debug\netcoreapp2.1\PrestoCoverage.UnitTest.dll --target "dotnet" --targetargs "test PrestoCoverage.UnitTest.csproj --no-build" --output "C:\coverlet\presto-coverage-sample-test-coverage" --format json

pause
