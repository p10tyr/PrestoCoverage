# PrestoCoverage

Extension
![ExtensionVersion](https://vsmarketplacebadge.apphb.com/version/PiotrKula.prestocoverage.svg) ![Installs](https://vsmarketplacebadge.apphb.com/installs/PiotrKula.prestocoverage.svg) ![Rating](https://vsmarketplacebadge.apphb.com/rating/PiotrKula.prestocoverage.svg)

Coverlet.Core NuGet
![Coverlet Core](
https://feeds.dev.azure.com/piotrkula/_apis/public/Packaging/Feeds/ed8ca68d-cdf8-41ec-8c30-0ba9dfb06f59@f93f76e1-7299-45e0-9728-9ddc6144099d/Packages/c65188b6-67f6-4888-827c-b3ca240118a5/Badge)



### The free and fast Visual Studio 2017 15.8+ code coverage tool built around Coverlet, the best .NET Core coverage tool!

* REQUIRES 2018 15.8 or later!
* Use Coverlet to generate .NET Core coverage reports.
* Install PrestoCoverage to instantly show you which lines were vistied by your tests.


**Coverage after tests completed**

![v114-demo](https://user-images.githubusercontent.com/1249683/47025539-9a8d9800-d15b-11e8-8a68-d3298e83548d.gif)


**Using Coverlet file**

![2018-10-03_14-55-38](https://user-images.githubusercontent.com/1249683/46415282-dcaae880-c71c-11e8-8c4f-76de5a3d0cb6.gif)

**Multiple coverage results merging**

![2018-10-09_21-54-14](https://user-images.githubusercontent.com/1249683/46698057-0036cd00-cc0e-11e8-9f77-15cf2e844007.gif)


**Installation**

https://marketplace.visualstudio.com/items?itemName=PiotrKula.prestocoverage

1. **Visual Studio 15.8+ is required.** Versions below this will not work.
1. Install the PrestoCoverage extension 
1. Run coverlet and save all results to `c:\coverlet\` (Check tips at the bottom)
1. PrestoCoverage will load any files that follow the convention of `*coverage.json`
1. Visual Studio UI will refresh as files get added, deleted, renamed or udpated.
1. Coverage files can be for any project/solution and supports miltiple instances of Visual Studio

**FEATURES ADDED SO FAR**

* Autoreload - https://github.com/ppumkin/PrestoCoverage/issues/2
* Detect source code change - https://github.com/ppumkin/PrestoCoverage/issues/6
* Visit Merging - https://github.com/ppumkin/PrestoCoverage/issues/3 

**TODO**

* Automatic Coverlet Execution - https://github.com/ppumkin/PrestoCoverage/issues/5

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
* It is practical for daily use. (Since I use it daily...)


