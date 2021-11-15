
# switch to current directory
Set-Location -Path $PSScriptRoot/..

# run unit test
dotnet test --collect "XPlat Code Coverage" -v m

# update code coverage
Get-ChildItem -Path . -Recurse -Include "TestResults" -Directory | ForEach-Object {
    $testFolderPath = $_.FullName
    Write-Output "root => $testFolderPath"
    Get-ChildItem -Path $testFolderPath -Directory | Sort-Object -Descending -Property LastWriteTime | ForEach-Object {
        $lastest = $_.Name
        if ($lastest.Length -eq 36) {
            Write-Output "$lastest"
            reportgenerator -reports:"$testFolderPath/$lastest/coverage.cobertura.xml" -targetdir:"$testFolderPath/coveragereport" -reporttypes:Html
            break
        }
        else {
            Write-Output "NOT => $lastest"
        }
    }
}
