#!/bin/bash

echo "========================================"
echo "Epic Flowchart Builder - Build Script"
echo "========================================"
echo ""

echo "Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo ""
    echo "========================================"
    echo "Restore failed! Check errors above."
    echo "========================================"
    exit 1
fi

echo ""
echo "Building project..."
dotnet build --configuration Release
if [ $? -ne 0 ]; then
    echo ""
    echo "========================================"
    echo "Build failed! Check errors above."
    echo "========================================"
    exit 1
fi

echo ""
echo "========================================"
echo "Build completed successfully!"
echo "========================================"
echo ""
echo "To run the application:"
echo "    cd bin/Release/net8.0-windows"
echo "    dotnet EpicFlowchartBuilder.dll"
echo ""
