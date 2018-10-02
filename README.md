# PrestoCoverage
### The fast and free Visual Studio 2017 code coverage Extension. Best used with Coverlet.

* You should use the excellent Coverlet tool for .NET Core coverage reports.
* Then you should install this extension to help you visualise which lines are covered in Visual Studio 2017

![image](https://user-images.githubusercontent.com/1249683/46355804-6e512200-c659-11e8-862b-205401d8b555.png)

**Installation**

https://marketplace.visualstudio.com/items?itemName=PiotrKula.prestocoverage

1. Install the PrestoCoverage extension 
1. Run coverlet and save all results to `c:\coverlet\` (Check tips at the bottom)
1. PrestoCoverage will load any files that follow the convention of `*coverage.json`
1. When ANY one coverage file changes (LastWriteTime) it will purge the cache and reload everything as needed again from fresh
1. Coverage files can be for any project/solution and supports miltiple instances of Visual Studio

**Some other info** 

* File name examples: `coverage.json`, `domain-coverage.json`, `api-loadbalancer-coverage.json`
* The coverage files can be for any project. The Extension works with multiple opened IDE's - It will only work with the files (Filepaths) that it finds in the coverlet.json and with the current window that you are working with (across several IDE's)
 * Large amounts of files in the coverlet directory may cause some lag during reload process - Please let me know how you get on. 
* Large amounts of tests (in max several files) should not really be much of a problem.
* Adding, removing or changing files in the directory, will automatically reload the data in the extension but only once you switch a tab or open/close a file in VS
* If you have **^1** two or more coverage files that overlap with the same file name- and one says there is 0 coverage and another says there is coverage... it could be reported incorrectly as no coverage. I am aware this is an issue but it requires much more time from me to work out how to fix this issue

**Tips**

In the `c:\coverlet` directory I created a PowerShell Script which I use `dotnet test` to run tests then the next line is `coverlet` (assuming you have it installed globally) . I use coverlets `--output` argument to place the results into this directory with a specific name. Like this I can run multiple tests, with multiple results files and the extension will place the coverage boxes for the appropriate file. So each time I delete the files and rerun the tests my VS should show updated coverage now. Plus coverlet gives you some coverage percentages while running so that is a good metric to try and boost each time.

```
dotnet test C:\Code\domain\Domain.Tests.Integration\Domain.Tests.Integration.csproj
coverlet  C:\Code\domain\Domain.Tests.Integration\bin\Debug\Domain.Tests.Integration.dll --target "dotnet" --targetargs "test C:\Code\domain\Domain.Tests.Integration --no-build" --output "C:\coverlet\domain-integration-coverage" --format json

pause
```

**Disclaimer**

* I made this to help me during my daily work. 
* I am not a "Extensions developer" some things may bot be az they shud beatiful as they should. *doh*
* There will be bugs!
* It is not as user friendly as I would want it.. yet.
* It is practical for daily use. (Since I do...)

