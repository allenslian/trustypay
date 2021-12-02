
# switch to current directory
Set-Location -Path $PSScriptRoot/..

# run unit test
dotnet test --collect "XPlat Code Coverage" -v m

# generate coverage report
if ($? -ne 0) {
    Get-ChildItem -Path . -Recurse -Include "TestResults" -Directory | ForEach-Object {
        $testFolderPath = $_.FullName
        Write-Output "test folder => $testFolderPath"
        foreach ($item in Get-ChildItem -Path $testFolderPath -Directory | Sort-Object -Descending -Property LastWriteTime)
        {
            $lastest = $item.Name
            if ($lastest.Length -eq 36) {
                Write-Output "lastest => $lastest"
                reportgenerator -reports:"$testFolderPath/$lastest/coverage.cobertura.xml" -targetdir:"$testFolderPath/coveragereport" -reporttypes:Html
                break
            }
        }
    }
}