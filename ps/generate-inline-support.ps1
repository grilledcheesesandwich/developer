################################################################################
## generate-inline-support.ps1
## Library support for inline C#
##
## Usage
##  1) In your script that uses the inline support, dot-source this script
##
##  2) Define just the body of a C# method, and store it in a string.  "Here
##     strings" work great for this.  The code can be simple:
##
##     $codeToRun = "Console.WriteLine(Math.Sqrt(337));"
##
##     or more complex:
##
##     $codeToRun = @"
##         string firstArg = (string) ((System.Collections.ArrayList) arg)[0];
##         int secondArg = (int) ((System.Collections.ArrayList) arg)[1];
##
##         Console.WriteLine("Hello {0} {1}", firstArg, secondArg );
##     
##         returnValue = secondArg * 3;
##     "@
##
##  3) (Optionally) Pack any arguments to your function into a single object.
##     This single object should be strongly-typed, so that PS does not treat
##     it as an PSObject.
##     An ArrayList works great for multiple elements.  If you have only one 
##     argument, you can pass it directly.
##   
##     [System.Collectionts.ArrayList] $arguments = `
##         new-object System.Collections.ArrayList
##     [void] $arguments.Add("World")
##     [void] $arguments.Add(337)
##
##  4) Invoke the inline code, optionally retrieving the return value.  You can
##     set the return value in your inline code by assigning it to the
##     "returnValue" variable as shown above.
##
##     $result = inline $codeToRun $arguments
##
##
##     If your code is simple enough, you can even do this entirely inline:
##
##     inline "Console.WriteLine(Math.Pow(337,2));"
##  
##
##     See the included unit tests for more examples.
##
################################################################################

## Stores a cache of generated inline objects.  If this library is dot-sourced
## from a script, these objects go away when the script exits.
${lee.holmes.inlineCache} = @{}

## The main function to execute inline C#.  
## Pass the argument to the function as a strongly-typed variable.  They will 
## be available from C# code as the Object variable, "arg".
## Any values assigned to the "returnValue" object by the C# code will be 
## returned to PS as a return value.
function inline([string] $code, [object] $arg)
{
    ## See if the code has already been compiled and cached
    $cachedObject = ${lee.holmes.inlineCache}[$code]
    
    ## The code has not been compiled or cached
    if($cachedObject -eq $null)
    {
        $codeToCompile = `
@"
    using System;

    public class InlineRunner
    {
        public Object Invoke(Object arg)
        {
            Object returnValue = null;
            
            $code
            
            return returnValue;
        }
    }
"@
        
        ## Obtains an ICodeCompiler from a CodeDomProvider class.
        $provider = new-object Microsoft.CSharp.CSharpCodeProvider

        ## Get the location for System.Management.Automation DLL
        #$dllName = Join-Path $PSHome "System.Management.Automation.dll"
        $dllName = $host.GetType().Assembly.Location
        
        ## Configure the compiler parameters
        $compilerParameters = `
            new-object System.CodeDom.Compiler.CompilerParameters

        $assemblies = @("System.dll", $dllName)
        $compilerParameters.ReferencedAssemblies.AddRange($assemblies)
        $compilerParameters.IncludeDebugInformation = $true
        $compilerParameters.GenerateInMemory = $true

        ## Invokes compilation. 
        $compilerResults = `
            $provider.CompileAssemblyFromSource($compilerParameters, $codeToCompile)

        ## Write any errors if generated.        
        if($compilerResults.Errors.Count -gt 0)
        {
            $errorLines = ""
            foreach($error in $compilerResults.Errors)
            {
                $errorLines += "`n`t" + $error.Line + ":`t" + $error.ErrorText
            }
            write-error $errorLines
        }
        ## There were no errors.  Store the resulting object in the object
        ## cache.
        else
        {
            ${lee.holmes.inlineCache}[$code] = `
                $compilerResults.CompiledAssembly.CreateInstance("InlineRunner")
        }
        
        $cachedObject = ${lee.holmes.inlineCache}[$code]
    }
    
    ## Finally invoke the C# code
    if($cachedObject -ne $null)
    {
        return $cachedObject.Invoke($arg)
    }
}

## A simple assert function.  Verifies that $condition
## is true.  If not, outputs the specified error message.
function assert 
     ( 
	[bool] $condition = $(Please specify a condition),
	[string] $message = "Test failed." 
     )
{
	if(-not $condition)
	{
		write-host "FAIL. $message"
	}
	else
	{
		write-host -NoNewLine ".";
	}
}

## A simple "assert equals" function.  Verifies that $expected
## is equal to $actual.  If not, outputs the specified error message.
function assertEquals
     ( 
	$expected = $(Please specify the expected object),
	$actual = $(Please specify the actual object),
	[string] $message = "Test failed." 
     )
{
	if(-not ($expected -eq $actual))
	{
		write-host "FAIL.  Expected: $expected.  Actual: $actual.  $message."
	}
	else
	{
		write-host -NoNewLine ".";
	}
}

## Unit tests for the inline support
function ut-inline
{
    ########################################
    ## Test that we can pass in a string
    ## Test that we can return a string
    ########################################
    $code = 
@"
    returnValue = (string) arg;
"@
    $returned = inline $code "test"
    assertEquals "test" $returned
    
    ########################################
    ## Test that we can pass in an ArrayList
    ## Test that we can return an int
    ########################################
    [System.Collections.ArrayList] $inputArrayList = 
        new-object System.Collections.ArrayList
    [void] $inputArrayList.Add(10)
    [void] $inputArrayList.Add(20)
    $code = 
@"
    int tmpReturnValue = 0;
    foreach(int value in (System.Collections.ArrayList) arg)
    {
        tmpReturnValue += value;
    }
    
    returnValue = tmpReturnValue;
"@
    $returned = inline $code $inputArrayList
    assertEquals 30 $returned
    
    ########################################
    ## Test that we can pass in an Array
    ## Test that we can return an Array
    ########################################
    [int[]] $inputArray = 1,2,3
    $code = 
@"
    int[] tmpReturnValue = new int[3];
    int[] argInput = (int[]) arg;
    for(int x = 0; x < tmpReturnValue.Length; x++)
    {
        tmpReturnValue[x] = argInput[x] * argInput[x];
    }
    
    returnValue = tmpReturnValue;
"@
    $returned = inline $code $inputArray
    assertEquals 1 $returned[0]
    assertEquals 4 $returned[1]
    assertEquals 9 $returned[2]
    
    write-host
}

write-host "Loading inline support."