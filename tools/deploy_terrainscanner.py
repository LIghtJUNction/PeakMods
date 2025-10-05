from pathlib import Path

# TerrainScanner 部署脚本
# 将 Unity 构建的 AssetBundle 复制到游戏插件目录

plugin_dir = "C:\\Users\\light\\AppData\\Roaming\\com.kesomannen.gale\\peak\\profiles\\Default\\BepInEx\\plugins\\TerrainScanner"

assets_dir = "C:\\Users\\light\\Documents\\GitHub\\PeakMods\\unity\\PeakChatOps\\Library\\com.unity.addressables\\aa\\Windows\\StandaloneWindows64"

source_dir = "C:\\Users\\light\\Documents\\GitHub\\PeakMods"

# 找到 TerrainScanner 的 bundle 文件（文件名包含 terrainscanner）
def find_terrainscanner_bundle(directory: Path):
    # 如果已经有 peakbundle 文件，直接返回
    for file in Path(directory).glob("*.peakbundle"):
        if "terrainscanner" in file.name.lower():
            return file
    
    # 查找包含 terrainscanner 的 bundle 文件
    for file in Path(directory).glob("*terrainscanner*.bundle"):
        return file
    
    # 如果没找到，返回最新的 bundle 文件
    bundle_files = list(Path(directory).glob("*.bundle"))
    if bundle_files:
        return max(bundle_files, key=lambda f: f.stat().st_mtime)
    
    return None

# 重命名为 TerrainScanner.peakbundle
def rename_to_peakbundle(file_path: Path):
    new_file_path = file_path.with_name("TerrainScanner.peakbundle")
    file_path.rename(new_file_path)
    return new_file_path

# 复制到插件目录
def copy_to_plugin_directory(file_path: Path, plugin_dir: Path):
    destination = Path(plugin_dir) / file_path.name
    # 确保目标目录存在
    Path(plugin_dir).mkdir(parents=True, exist_ok=True)
    
    with open(file_path, "rb") as src_file:
        with open(destination, "wb") as dst_file:
            dst_file.write(src_file.read())
    return destination

# 复制到源代码目录（用于版本控制）
def copy_to_source_directory(file_path: Path, source_dir: Path):
    destination = Path(source_dir) / "src" / "TerrainScanner" / "Assets" / file_path.name
    # 确保目标目录存在
    destination.parent.mkdir(parents=True, exist_ok=True)
    
    with open(file_path, "rb") as src_file:
        with open(destination, "wb") as dst_file:
            dst_file.write(src_file.read())
    return destination

# 确保插件目录下的 DLL 文件为 Debug 版本（开发模式）
def ensure_debug_dll(source_dir: Path, plugin_dir: Path):
    source_dll = source_dir / "artifacts" / "bin" / "TerrainScanner" / "debug" / "com.github.LIghtJUNction.TerrainScanner.dll"
    destination_dll = Path(plugin_dir) / "com.github.LIghtJUNction.TerrainScanner.dll"
    
    # 检查目标文件是否为链接
    if destination_dll.exists() and destination_dll.is_symlink():
        print("Debug DLL link already exists.")
    else:
        if destination_dll.exists():
            destination_dll.unlink()  # 删除现有文件
        destination_dll.symlink_to(source_dll)
        print("Created symlink for Debug DLL.")
    
    return destination_dll

def main():
    # 查找 TerrainScanner bundle 文件
    bundle_file = find_terrainscanner_bundle(Path(assets_dir))
    if bundle_file is None:
        print("No TerrainScanner .bundle files found in the specified directory.")
        print(f"Searched in: {assets_dir}")
        return
    
    print(f"Found bundle file: {bundle_file.name}")
    
    # 重命名为 TerrainScanner.peakbundle
    renamed_file = rename_to_peakbundle(bundle_file)
    print(f"Renamed to: {renamed_file.name}")
    
    # 输出文件大小方便调试
    file_size = renamed_file.stat().st_size
    print(f"File size: {file_size:,} bytes ({file_size / 1024:.2f} KB)")
    
    # 复制到插件目录
    copied_plugin_file = copy_to_plugin_directory(renamed_file, Path(plugin_dir))
    print(f"✓ Copied to plugin directory: {copied_plugin_file}")
    
    # 复制到源代码目录
    copied_source_file = copy_to_source_directory(renamed_file, Path(source_dir))
    print(f"✓ Copied to source directory: {copied_source_file}")
    
    print("\n✅ TerrainScanner AssetBundle deployed successfully!")

if __name__ == "__main__":
    main()
