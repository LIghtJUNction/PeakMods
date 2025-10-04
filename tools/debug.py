from pathlib import Path

# 脚本文件硬编码算了
# 插件目录

plugin_dir = "C:\\Users\\light\\AppData\\Roaming\\com.kesomannen.gale\\peak\\profiles\\Default\\BepInEx\\plugins\\PeakChatOps"

assets_dir = "C:\\Users\\light\\Documents\\GitHub\\PeakMods\\unity\\PeakChatOps\\Library\\com.unity.addressables\\aa\\Windows\\StandaloneWindows64"

source_dir = "C:\\Users\\light\\Documents\\GitHub\\PeakMods"
# 找到最大的bundle 文件

def find_largest_bundle(directory:Path):
    largest_file = None
    largest_size = 0

    # 如果有peakbundle 直接返回
    for file in Path(directory).glob("*.peakbundle"):
        return file

    for file in Path(directory).glob("*.bundle"):
        size = file.stat().st_size
        if size > largest_size:
            largest_size = size
            largest_file = file

    return largest_file

# 重命名为PeakChatOps.peakbundle 返回文件路径
def rename_to_peakbundle(file_path: Path):
    new_file_path = file_path.with_name("PeakChatOpsUI.peakbundle")
    file_path.rename(new_file_path)
    return new_file_path

# 复制到插件目录
def copy_to_plugin_directory(file_path: Path, plugin_dir: Path):
    destination = Path(plugin_dir) / file_path.name
    with open(file_path, "rb") as src_file:
        with open(destination, "wb") as dst_file:
            dst_file.write(src_file.read())
    return destination

# 复制到源代码目录
def copy_to_source_directory(file_path: Path, source_dir: Path):
    destination = Path(source_dir) / "src" / "PeakChatOps" / "assets" / file_path.name
    with open(file_path, "rb") as src_file:
        with open(destination, "wb") as dst_file:
            dst_file.write(src_file.read())
    return destination

# 确保插件目录下的dll文件为debug版本
# 将com.github.LIghtJUNction.PeakChatOps.dll 链接到插件目录
def ensure_debug_dll(source__dir: Path, plugin_dir: Path):
    source_dll = source__dir / "artifacts" / "bin" / "PeakChatOps" / "debug" / "com.github.LIghtJUNction.PeakChatOps.dll"
    destination_dll = plugin_dir / "com.github.LIghtJUNction.PeakChatOps.dll"
    
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
    # 查找最大的 bundle 文件
    largest_bundle = find_largest_bundle(Path(assets_dir))
    if largest_bundle is None:
        print("No .bundle files found in the specified directory.")
        return
    # 重命名为 PeakChatOps.peakbundle
    renamed_file = rename_to_peakbundle(largest_bundle)
    print(f"Renamed file: {renamed_file}")

    # 输出文件大小方便调试
    file_size = renamed_file.stat().st_size
    print(f"File size: {file_size} bytes")

    # 复制到插件目录
    copied_plugin_file = copy_to_plugin_directory(renamed_file, Path(plugin_dir))
    print(f"Copied to plugin directory: {copied_plugin_file}")

    # 复制到源代码目录
    copied_source_file = copy_to_source_directory(renamed_file, Path(source_dir))
    print(f"Copied to source directory: {copied_source_file}")




if __name__ == "__main__":
    main()