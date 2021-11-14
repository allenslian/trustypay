#! /usr/bin/env bash

# unit test
dotnet test --collect "XPlat Code Coverage" -v m

# generate coverage report
for test_dir in $(find . -type d -name "TestResults")
do
    all_tests=$(ls -c $test_dir)
    lastest_test=${all_tests:0:36}
    reportgenerator -reports:"$test_dir/$lastest_test/coverage.cobertura.xml" -targetdir:"$test_dir/coveragereport" -reporttypes:Html    
done