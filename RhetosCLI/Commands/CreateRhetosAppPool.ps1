Function CreateRhetosAppPool
{
	<#
	.SYNOPSIS
	    Creates Rhetos App pool and sets default props

	.DESCRIPTION
	    Use this command to create Rhetos app pool
		What it does:
		- Creates app pool
		- Sets specified app pool identity
		- Sets pool props
		
	.PARAMETER Name
		Pool name

	.PARAMETER PoolIdentity
		App pool identity

	.PARAMETER PoolIdentityPassword
		App pool identity password

	.EXAMPLE
		Create Rhetos app pool, basicly cerates app pool and sets Rhetos specific stuff 
	.NOTES
		Author: Vladimir Mašala

	.LINK
	#>


	Param
	(
		[Parameter(Mandatory=$True)][String]$PoolName,
		[Parameter(Mandatory=$True)][String]$PoolIdentity,
        [Parameter(Mandatory=$True)][String]$PoolIdentityPassword
	)
        try
        {
            if ((New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))
            {
                Import-Module WebAdministration
		        New-WebAppPool $PoolName
		        Set-ItemProperty IIS:\AppPools\vlado_Test -name processModel -value @{userName="$PoolIdentity";password="$PoolIdentityPassword";identitytype=3}
		        Set-ItemProperty IIS:\AppPools\$PoolName -name "enable32BitAppOnWin64" -Value "true"
            }
            else
            {
                Write-Host "Process should have elevated status to access IIS configuration data. You need to run script as admin." -ForegroundColor Red
            }
        }
        catch
        {
            Write-Output "Ran into an issue: $PSItem"
        }
        
	}
	CreateRhetosAppPool "vlado_test" "os\vladimir"
