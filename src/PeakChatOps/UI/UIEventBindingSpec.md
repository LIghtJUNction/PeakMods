# PeakChatOps UI 事件绑定规范

本规范旨在统一 PeakChatOps 前端（UXML/USS）与后端（C#）的 UI 事件绑定方式，确保两端实现的一致性与兼容性，便于维护和扩展。

## 1. 命名规范

### 元素命名
- 所有需要绑定事件的 UI 元素（如按钮、输入框等）必须在 UXML 中设置唯一且语义化的 `name` 属性。
- 推荐使用小写字母和短横线分隔，例如：`send-button`、`close-button`、`minimize-button`、`message-input`。

### 事件方法命名
- 后端事件处理方法应以 `On` 前缀加事件语义命名，如：`OnSendMessage`、`OnClose`、`OnMinimize`。

## 2. 前端实现要求（UXML/USS）

- UXML 文件需包含所有交互元素，并为其分配规范化的 `name`。
- USS 文件可为元素分配样式类，推荐为交互元素添加如 `button`、`input` 等基础类。
- 交互元素示例：
  ```xml
  <Button name="send-button" text="发送" />
  <Button name="close-button" text="关闭" />
  <Button name="maximize-button" text="最大化" />
  <Button name="minimize-button" text="最小化" />
  <TextField name="message-input" />
  ```

## 3. 后端实现要求（C#）

- 通过 `UIDocument.rootVisualElement.Q<T>("元素名")` 获取前端元素。
- 事件绑定需在统一入口方法（如 `BindUIEvents(GameObject uiGO)`）中完成。
- 事件绑定示例：
  ```csharp
  var sendButton = root.Q<Button>("send-button");
  if (sendButton != null)
      sendButton.clicked += OnSendMessage;
  ```
- 输入框等需保存引用，便于后续操作（如聚焦、取值等）。
- 所有事件绑定需有日志记录，便于调试。

## 4. 事件处理流程

- 前端负责定义交互元素及样式，后端负责事件逻辑。
- 元素名作为事件绑定的桥梁，前后端需保持一致。
- 事件处理方法需保证幂等性和异常处理。

## 5. 扩展与兼容性

- 新增交互元素时，需同步更新 UXML 和后端绑定方法。
- 保持元素命名规范，避免冲突。
- 后端事件绑定方法应支持元素缺失时的容错处理。

## 6. 示例

### UXML 片段
```xml
<Button name="send-button" text="发送" />
<Button name="close-button" text="关闭" />
<Button name="minimize-button" text="最小化" />
<TextField name="message-input" />
```

### C# 片段
```csharp
var sendButton = root.Q<Button>("send-button");
if (sendButton != null)
    sendButton.clicked += OnSendMessage;
```

---

如需扩展，请遵循本规范，确保前后端一致性与可维护性。