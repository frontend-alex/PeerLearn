#!/bin/bash

# Server Test Script
# Runs all integration tests for the PeerLearn API server

echo "Running PeerLearn API Tests..."
echo "=============================="

cd "$(dirname "$0")/../app/server"

# Run tests
dotnet test Tests/PeerLearn.Tests.csproj --logger "console;verbosity=normal"

# Capture exit code
TEST_EXIT_CODE=$?

if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo ""
    echo "✅ All tests passed!"
else
    echo ""
    echo "❌ Tests failed with exit code: $TEST_EXIT_CODE"
fi

exit $TEST_EXIT_CODE
