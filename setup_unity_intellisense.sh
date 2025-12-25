#!/bin/bash

# Configuration
SOURCE_BASE_DIR="$HOME/.vscode/extensions"
DEST_BASE_DIR="$HOME/.antigravity/extensions"
VSCODE_EXTENSIONS_JSON="$SOURCE_BASE_DIR/extensions.json"
ANTIGRAVITY_EXTENSIONS_JSON="$DEST_BASE_DIR/extensions.json"

# List of extensions to migrate (ID prefix)
EXTENSIONS=(
    "ms-dotnettools.csharp"
    "ms-dotnettools.csdevkit"
    "ms-dotnettools.vscode-dotnet-runtime"
    "visualstudiotoolsforunity.vstuc"
)

# Function to detect VS Code / Antigravity version
detect_version() {
    if command -v code &> /dev/null; then
        CODE_VERSION=$(code --version | head -n 1)
        echo "$CODE_VERSION"
    else
        # Default fallback
        echo "1.104.0"
    fi
}

TARGET_VERSION=$(detect_version)

# Calculate a permissive target engine version (Target Version - 10 minor versions)
TARGET_ENGINE=$(python3 -c "
try:
    ver_parts = '$TARGET_VERSION'.split('.')
    major = int(ver_parts[0])
    minor = int(ver_parts[1])
    # Subtract 10 from minor version, ensure it doesn't go below 0
    new_minor = max(0, minor - 10)
    print(f'^{major}.{new_minor}.0')
except:
    print('^1.0.0') # Fallback if parsing fails
")

echo "Targeting VS Code Engine Version: $TARGET_ENGINE (Detected codebase version: $TARGET_VERSION)"

mkdir -p "$DEST_BASE_DIR"

# Ensure extensions exist in source
echo "Checking source extensions in $SOURCE_BASE_DIR..."
MISSING_EXTS=()
for ext_id in "${EXTENSIONS[@]}"; do
    # Find directory matching the extension ID
    FOUND=$(find "$SOURCE_BASE_DIR" -maxdepth 1 -name "${ext_id}-*" -type d -print -quit)
    if [ -z "$FOUND" ]; then
        echo "Missing extension: $ext_id"
        MISSING_EXTS+=("$ext_id")
    fi
done

if [ ${#MISSING_EXTS[@]} -ne 0 ]; then
    echo "Installing missing extensions via VS Code CLI..."
    if command -v code &> /dev/null; then
        for ext_id in "${MISSING_EXTS[@]}"; do
            echo "Installing $ext_id..."
            code --install-extension "$ext_id" --force
        done
    else
        echo "Error: 'code' command not found. Cannot install missing extensions automatically."
        echo "Please install the following extensions manually in VS Code first:"
        printf '%s\n' "${MISSING_EXTS[@]}"
        exit 1
    fi
fi

# Copy and Patch
for ext_id in "${EXTENSIONS[@]}"; do
    # Find directory again (in case it was just installed)
    SOURCE_DIR=$(find "$SOURCE_BASE_DIR" -maxdepth 1 -name "${ext_id}-*" -type d | sort -V | tail -n 1)
    
    if [ -z "$SOURCE_DIR" ]; then
        echo "Error: Could not locate $ext_id even after attempted installation."
        continue
    fi

    DIR_NAME=$(basename "$SOURCE_DIR")
    DEST_DIR="$DEST_BASE_DIR/$DIR_NAME"

    echo "Processing $ext_id..."
    echo "  Source: $SOURCE_DIR"
    echo "  Dest:   $DEST_DIR"

    # Remove old version if exists
    if [ -d "$DEST_DIR" ]; then
        rm -rf "$DEST_DIR"
    fi

    # Copy
    cp -r "$SOURCE_DIR" "$DEST_DIR"

    # Patch package.json
    PACKAGE_JSON="$DEST_DIR/package.json"
    if [ -f "$PACKAGE_JSON" ]; then
        echo "  Patching package.json engines..."
        python3 -c "
import json
import sys

try:
    with open('$PACKAGE_JSON', 'r') as f:
        data = json.load(f)
    
    if 'engines' not in data:
        data['engines'] = {}
    
    data['engines']['vscode'] = '$TARGET_ENGINE'
    
    with open('$PACKAGE_JSON', 'w') as f:
        json.dump(data, f, indent=4)
except Exception as e:
    print(f'  Error patching package.json: {e}')
    sys.exit(1)
"
    fi
done

# Merge extensions.json
echo "Merging extensions.json..."

if [ ! -f "$VSCODE_EXTENSIONS_JSON" ]; then
    echo "Warning: No source extensions.json found at $VSCODE_EXTENSIONS_JSON"
else
    python3 -c "
import json
import os
import sys

source_path = '$VSCODE_EXTENSIONS_JSON'
dest_path = '$ANTIGRAVITY_EXTENSIONS_JSON'

target_ids = [
    'ms-dotnettools.csharp',
    'ms-dotnettools.csdevkit',
    'ms-dotnettools.vscode-dotnet-runtime',
    'visualstudiotoolsforunity.vstuc'
]

try:
    with open(source_path, 'r') as f:
        source_data = json.load(f)
    
    dest_data = []
    if os.path.exists(dest_path):
        try:
            with open(dest_path, 'r') as f:
                dest_data = json.load(f)
        except json.JSONDecodeError:
            print('  Warning: Destination extensions.json was invalid, starting fresh.')
            dest_data = []
    
    # Create a map of existing destination extensions by ID for easy update
    dest_map = {item['identifier']['id'].lower(): item for item in dest_data if 'identifier' in item and 'id' in item['identifier']}

    # Extract target extensions from source
    for item in source_data:
        if 'identifier' in item and 'id' in item['identifier']:
            item_id = item['identifier']['id'].lower()
            # Check if this is one of the extensions we want to copy
            if any(target_id in item_id for target_id in target_ids):
                # Update paths in location/fsPath if necessary (optional, but good for correctness if absolute paths are used)
                # For simplicity, we just take the metadata entry. 
                # Antigravity might rely on 'relativeLocation' or just the presence in this list.
                
                # Update location paths to point to new directory
                if 'location' in item and 'fsPath' in item['location']:
                    old_path = item['location']['fsPath']
                    dirname = os.path.basename(old_path)
                    new_path = os.path.join('$DEST_BASE_DIR', dirname)
                    item['location']['fsPath'] = new_path
                    item['location']['path'] = new_path
                    if 'external' in item['location']:
                         item['location']['external'] = 'file://' + new_path

                dest_map[item_id] = item
                print(f'  Merged entry for {item_id}')

    # Reconstruct dest_data
    new_dest_data = list(dest_map.values())

    with open(dest_path, 'w') as f:
        json.dump(new_dest_data, f, indent=4)

    print('extensions.json updated successfully.')

except Exception as e:
    print(f'Error merging extensions.json: {e}')
    sys.exit(1)
"
fi

# Patch csdevkit checkHostApp
echo "Patching ms-dotnettools.csdevkit host check..."
find "$DEST_BASE_DIR" -type f -path '*/ms-dotnettools.csdevkit-*/dist/extension.js' -exec sed -i 's/checkHostApp=function(){return!!\["Visual Studio Code"/checkHostApp=function(){return!!\["Antigravity","Visual Studio Code"/' {} +

echo "Setup complete. Please reload Antigravity."
