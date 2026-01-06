#!/bin/bash
# Script to package the Miccore Clean Architecture Template

echo "📦 Creating NuGet Template Package..."
echo ""

# Navigate to the project directory
cd "$(dirname "$0")"

# Create temp directory for packaging
TEMP_DIR=".template-package"
rm -rf "$TEMP_DIR"
mkdir -p "$TEMP_DIR/content"

echo "✅ Copying template files..."

# Copy all files except excluded ones
rsync -av \
  --exclude='bin/' \
  --exclude='obj/' \
  --exclude='.vs/' \
  --exclude='.vscode/' \
  --exclude='logs/' \
  --exclude='*.user' \
  --exclude='.git/' \
  --exclude='.github/' \
  --exclude='.template-package/' \
  ./ "$TEMP_DIR/content/"

echo "✅ Files copied to temporary directory"
echo ""

# Create a simple manifest for the template
echo "📝 Creating package metadata..."

# Copy the nuspec file
cp Miccore.CleanArchitecture.Template.nuspec "$TEMP_DIR/"

echo ""
echo "✅ Package structure ready at: $TEMP_DIR"
echo ""
echo "📌 To create the NuGet package, you need nuget.exe:"
echo "   1. Download from: https://www.nuget.org/downloads"
echo "   2. Run: nuget pack $TEMP_DIR/Miccore.CleanArchitecture.Template.nuspec"
echo ""
echo "📌 Or test locally without packaging:"
echo "   dotnet new uninstall /Users/manher/Projects/Miccore/sample_project"
echo "   dotnet new install /Users/manher/Projects/Miccore/sample_project"
echo ""
echo "📌 To publish to NuGet.org:"
echo "   nuget push Miccore.CleanArchitecture.Template.1.0.0.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY"
echo ""
