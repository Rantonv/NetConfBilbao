#!/usr/bin/env python3
"""
Simple script to validate Mermaid syntax by extracting and checking basic syntax patterns.
"""

import re
import sys

def validate_mermaid_syntax(file_path):
    """Extract and validate Mermaid chart syntax from markdown file."""
    
    try:
        with open(file_path, 'r') as f:
            content = f.read()
        
        # Extract Mermaid code blocks
        mermaid_pattern = r'```mermaid\n(.*?)\n```'
        mermaid_blocks = re.findall(mermaid_pattern, content, re.DOTALL)
        
        if not mermaid_blocks:
            print("❌ No Mermaid chart found in the file")
            return False
        
        print(f"✅ Found {len(mermaid_blocks)} Mermaid chart(s)")
        
        for i, block in enumerate(mermaid_blocks):
            print(f"\n🔍 Validating Mermaid chart {i+1}:")
            
            # Basic syntax validation
            lines = block.strip().split('\n')
            
            # Check for graph declaration
            graph_declaration = False
            for line in lines:
                if line.strip().startswith('graph '):
                    graph_declaration = True
                    print(f"  ✅ Graph declaration found: {line.strip()}")
                    break
            
            if not graph_declaration:
                print("  ❌ No graph declaration found")
                return False
            
            # Count nodes and connections
            nodes = []
            connections = []
            
            for line in lines:
                line = line.strip()
                if not line or line.startswith('%%') or line.startswith('classDef') or line.startswith('class '):
                    continue

                # Look for node definitions e.g., A[Label]
                node_pattern = r'(?<![\w])([A-Za-z]\w*)\s*\[(.*?)\]'
                node_matches = re.findall(node_pattern, line)
                for match in node_matches:
                    if match[0] not in nodes:
                        nodes.append(match[0])

                # Look for connections while ignoring edge labels, e.g., A -- text --> B
                edge_patterns = [
                    r'^\s*([A-Za-z]\w*)\s*--.*?-->\s*([A-Za-z]\w*)\s*;?\s*$',  # --label-->
                    r'^\s*([A-Za-z]\w*)\s*-\\.->\s*([A-Za-z]\w*)\s*;?\s*$',    # -.->
                    r'^\s*([A-Za-z]\w*)\s*<---->\s*([A-Za-z]\w*)\s*;?\s*$'      # <---->
                ]
                for pattern in edge_patterns:
                    m = re.match(pattern, line)
                    if m:
                        connections.append((m.group(1), m.group(2)))
            
            print(f"  📊 Found {len(nodes)} nodes: {', '.join(nodes)}")
            print(f"  🔗 Found {len(connections)} connections")
            
            # Validate that connections reference valid nodes
            valid_connections = True
            for conn in connections:
                if conn[0] not in nodes or conn[1] not in nodes:
                    print(f"  ❌ Invalid connection: {conn[0]} -> {conn[1]}")
                    valid_connections = False
            
            if valid_connections:
                print(f"  ✅ All connections are valid")
            
            # Check for styling
            has_styling = any('classDef' in line for line in lines)
            if has_styling:
                print(f"  🎨 Chart includes styling definitions")
            
        return True
        
    except Exception as e:
        print(f"❌ Error validating file: {e}")
        return False

def main():
    """Main validation function."""

    import os
    import glob

    # Allow overriding docs dir via CLI arg or environment variable; default to ./docs
    default_docs = os.path.join(os.getcwd(), "docs")
    docs_dir = (
        sys.argv[1]
        if len(sys.argv) > 1
        else os.environ.get("DOCS_DIR", default_docs)
    )

    if not os.path.isdir(docs_dir):
        print(f"❌ Docs directory not found: {docs_dir}")
        return False

    # Find the latest generated file
    pattern = os.path.join(docs_dir, "SolutionOverview-*.md")
    files = glob.glob(pattern)

    if not files:
        print("❌ No generated documentation files found")
        return False

    # Get the most recent file
    latest_file = max(files, key=os.path.getctime)
    print(f"🔍 Validating latest generated file: {os.path.basename(latest_file)} in {docs_dir}")

    return validate_mermaid_syntax(latest_file)

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)