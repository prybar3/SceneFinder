# Scene Finder 场景查找器

## 功能介绍
Scene Finder 是一个 Unity 编辑器工具，用于快速查找和打开项目中的场景文件。

## 主要功能
- 🔍 **场景搜索**：支持模糊搜索场景名称
- ⭐ **收藏功能**：可以将常用场景标记为收藏
- 🖱️ **快捷操作**：
  - 双击打开场景
  - 拖拽场景到 Hierarchy 叠加打开
- 💾 **状态保存**：窗口状态和收藏列表自动保存
- 🔄 **自动恢复**：重启 Unity 后自动恢复窗口状态

## 使用方法

### 打开窗口
1. 在 Unity 菜单栏选择 `Tools > Scene Finder`
2. 窗口会自动列出项目中所有场景

### 搜索场景
在搜索框中输入场景名称（支持模糊搜索），列表会自动过滤匹配的场景

### 收藏场景
点击场景名称前的 ☆ 图标，该场景会被标记为收藏（★），收藏的场景会显示在列表顶部

### 打开场景
双击场景名称即可打开该场景（如果当前场景有未保存的修改，会提示保存）

### 拖拽场景
可以直接拖拽场景到 Hierarchy 窗口或其他支持的位置

## 文件结构
```
Assets/Plugins/SceneFinder/
├── Editor/                              # 编辑器脚本
│   ├── SceneFinderWindow.cs            # 主窗口逻辑
│   ├── SceneFavoritesDataSO.cs         # 收藏数据定义
│   └── SceneFinderRestore.cs           # 窗口恢复逻辑
└── README.md                            # 说明文档

Assets/Settings/
└── SceneFinderFavorites.asset          # 收藏列表数据（项目设置）
```

## 使用方法
1. Unity 会自动在 `Assets/Settings/` 目录下创建收藏数据文件
2. 通过 `Tools > Scene Finder` 菜单打开窗口即可使用

## 特性说明
- 使用 ScriptableObject 存储收藏数据
- 使用 SessionState 保存窗口状态
- 支持自定义拖拽操作
- 自动创建缺失的数据文件

## 注意事项
- 收藏数据保存在 `Assets/Settings/SceneFinderFavorites.asset`
- 删除该文件后，工具会自动创建新的空数据文件
- 窗口关闭后，收藏列表会自动保存

## 版本信息
- 版本：1.0
- 兼容 Unity 版本：2020.3 及以上


