
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public class PowerShellWrapper
{
    private PowerShell host = PowerShell.Create(InitialSessionState.CreateDefault());

    public object RunScript(string path)
    {
        return host.AddScript(path).Invoke();
    }

    public static Collection<PSObject> RunCommand(string command)
    {
        using (PowerShell PowerShellInstance = PowerShell.Create())
        {
            //// use "AddScript" to add the contents of a script file to the end of the execution pipeline.
            //// use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
            //PowerShellInstance.AddScript("param($param1) $d = get-date; $s = 'test string value'; " +
            //        "$d; $s; $param1; get-service");

            //// use "AddParameter" to add a single parameter to the last command/script on the pipeline.
            //PowerShellInstance.AddParameter("param1", "parameter 1 value!");

            

            PowerShellInstance.AddScript(command);

            var results = PowerShellInstance.Invoke();

            return results;

        }
        //return host.AddCommand("dotnet").AddArgument("test");
    }
}
