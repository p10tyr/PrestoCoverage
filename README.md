# PrestoCoverage
Visual Studio Extension for simple coverage visuals

Here is a very basic version of the extension. I have optimised it a bit so it doesn't slow down the IDE with some caching techniques. This is a very fast release for your testing convenience only and there are bugs in here..I use this for my self so I try and reduce bugs and every time I fix or tweak something i increment the version number. I use coverlet and this extension to help me with my work.

1. Install VSIX Extension v1.4 (compiled in Release mode) (download at bottom)
1. Run coverlet and save all results to `c:\coverlet\`
1. Extension will load any files that follow the convention of `*coverage.json`
1. When ANY one coverage file changes (LastWriteTime) it will purge the cache and reload everything as needed again from fresh

![image](https://user-images.githubusercontent.com/1249683/46355804-6e512200-c659-11e8-862b-205401d8b555.png)

* RED - No tests visited this line **^1**
* GREEN - Has been visited at least once

**Download**

[VSIXProject1.4.vsix.zip](https://github.com/tonerdo/coverlet/files/2438341/VSIXProject1.4.vsix.zip)

**Some other info** 

  * File name examples: `coverage.json`, `domain.coverage.json`, `api.loadbalancer.coverage.json`
  * The coverage files can be for any project. The Extension works with multiple opened IDE's - It will only work with the files (Filepaths) that it finds in the coverlet.json and with the current window that you are working with (across several IDE's)
 * Large amounts of files in the coverlet directory may cause some lag during reload process - Please let me know how you get on. 
* Large amounts of tests (in max several files) should not really be much of a problem.
* Adding, removing or changing files in the directory, will automatically reload the data in the extension but only once you switch a tab or open/close a file in VS
* If you have **^1** two or more coverage files that overlap with the same file name- and one says there is 0 coverage and another says there is coverage... it could be reported incorrectly as no coverage. I am aware this is an issue but it requires much more time from me to work out how to fix this issue

**Tips**

In the `c:\coverlet` directory I created a PowerShell Script which I use `dotnet test` to run tests then the next line is `coverlet` (assuming you have it installed globally) . I use coverlets `--output` argument to place the results into this directory with a specific name. Like this I can run multiple tests, with multiple results files and the extension will place the coverage boxes for the appropriate file. So each time I delete the files and rerun the tests my VS should show updated coverage now. Plus coverlet gives you some coverage percentages while running so that is a good metric to try and boost each time.

```
dotnet test C:\Code\domain\Domain.Tests.Integration\Domain.Tests.Integration.csproj
coverlet  C:\Code\domain\Domain.Tests.Integration\bin\Debug\Domain.Tests.Integration.dll --target "dotnet" --targetargs "test C:\Code\domain\Domain.Tests.Integration --no-build" --output "C:\coverlet\domain.integration.coverage.json"

pause
```
