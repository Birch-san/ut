#Function Clear-Host2 { [System.Console]::Clear() }

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
Set-Location $scriptDir

$filename = "$scriptDir\execute\bin\Debug\output.log"
$reader = new-object System.IO.StreamReader(New-Object IO.FileStream($filename, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [IO.FileShare]::ReadWrite))
#start at the end of the file
$lastMaxOffset = $reader.BaseStream.Length

Function li() {
    while ($true)
    {
        Start-Sleep -m 10

        #if the file size has not changed, idle
        if ($reader.BaseStream.Length -eq $lastMaxOffset) {
            continue;
        }

        #seek to the last max offset
        $reader.BaseStream.Seek($lastMaxOffset, [System.IO.SeekOrigin]::Begin) | out-null

        #read out of the file until the EOF
        $line = ""
        while (($line = $reader.ReadLine()) -ne $null) {
            if ($line -match 'Waiting for behaviours ready') {
                #Start-Sleep -m 3000
                Clear-Host
            }
            if ($line -match 'DEBUG 0') {
                write-output $line
            }
        }

        #update the last max offset
        $lastMaxOffset = $reader.BaseStream.Position
    }
}

Function careLine($line) {
    if (!$global:encounteredEnd) {
        if ($line -match 'DEBUG 0') {
            if ($line -match 'Waiting for behaviours ready') {
                #Start-Sleep -m 3000
                if ($global:encounteredStart) {
                    $global:encounteredEnd = $true
                }
                $global:encounteredStart = $true
                #Clear-Host
            }
            if ($global:encounteredStart) {
                 $line
            }
        }
    }
}

#Get-FileTail -Path "execute\bin\Debug\Test.txt" -Wait 
#while ($true) {Clear-Host; gc "execute\bin\Debug\Test.txt" -Tail 50; sleep 1 }
#Get-FileTail "execute\bin\Debug\Test.txt" -Wait | out-null

$global:encounteredStart = $false
$global:encounteredEnd = $false
Function yo() {
    $global:encounteredStart = $false
    $global:encounteredEnd = $false
    gc $filename -Tail 200 | %{careLine($_)}
}