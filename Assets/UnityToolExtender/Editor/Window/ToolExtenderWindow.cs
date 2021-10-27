using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Wing.Tools.Editor
{
    public class ToolExtenderWindow : EditorWindow
    {
        private ListView _itemConainer;
        private List<ExtendItem> _newItems = new List<ExtendItem>();

        private Button _btnAdd, _btnDel, _btnUp,
            _btnDown, _btnOK, _btnAccept, _btnTest,
            _btnOpenCmd, _btnOpenWs;
        
        private VisualElement _inputContainer;

        private TempExtendItem _tempExtendItem;
        private SerializedObject _editorItem;
        private ExtendItem _prevItem;
        
        [MenuItem("Tools/外部工具 %E")]
        public static void ShowExample()
        {
            ToolExtenderWindow wnd = GetWindow<ToolExtenderWindow>();
            wnd.titleContent = new GUIContent("外部工具");
            wnd.minSize = wnd.maxSize = new Vector2(300, 350);
            // 如果窗口不显示，请注释下面的if语句，并打开几次就行...
            if (wnd.position.width == 0 || wnd.position.height == 0)
            {
                wnd.position = new Rect((Screen.currentResolution.width - 300) * 0.5f,
                    (Screen.currentResolution.height - 350) * 0.5f, wnd.minSize.x, wnd.minSize.y);
            }

            wnd.Show();
        }

        public void CreateGUI()
        {
            _newItems.Clear();
            _tempExtendItem = CreateInstance<TempExtendItem>();
            _editorItem = new SerializedObject(_tempExtendItem);
            _newItems.AddRange(ToolExtends.Instance.items);
            
            VisualElement root = rootVisualElement;
            
            var relDir = AssetDatabase.GetAssetPath(ToolExtends.Instance).Replace("Data/default.asset", "");
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(relDir + "Window/ToolExtenderWindow.uxml");
            VisualElement container = visualTree.CloneTree();
            root.Add(container);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(relDir + "Window/ToolExtenderWindow.uss");
            root.styleSheets.Add(styleSheet);
            
            _itemConainer = container.Q<ListView>("items-container");
            _itemConainer.selectionType = SelectionType.Single;

            _itemConainer.makeItem = () => new Label();
            _itemConainer.bindItem = OnBindItem;
            _itemConainer.itemsSource = _newItems;
#if UNITY_2021_2_OR_NEWER
            _itemConainer.onItemsChosen += OnItemChose;
            _itemConainer.onSelectionChange += OnSelectionChanged;
#else
            _itemConainer.onItemChosen += OnItemChose;
            _itemConainer.onSelectionChanged += OnSelectionChanged;
#endif

            _btnAdd = container.Q<Button>("btn-add");
            _btnAdd.clicked += OnAddClick;

            _btnDel = container.Q<Button>("btn-del");
            _btnDel.clicked += OnDelClick;
            
            _btnUp = container.Q<Button>("btn-up");
            _btnUp.clicked += OnUpClick;
            
            _btnDown = container.Q<Button>("btn-down");
            _btnDown.clicked += OnDownClick;
            
            _btnOK = container.Q<Button>("btn-ok");
            _btnOK.clicked += () => Save(true);

            _btnAccept = container.Q<Button>("btn-accept");
            _btnAccept.clicked += () => Save(false);

            container.Q<Button>("btn-cancel").clicked += () => Close();

            _btnTest = container.Q<Button>("btn-test");
            _btnTest.clicked += OnTest;

            _btnOpenCmd = container.Q<Button>("btn-open-cmd");
            _btnOpenCmd.clicked += OnOpenCmdClick;
            
            _btnOpenWs = container.Q<Button>("btn-open-ws");
            _btnOpenWs.clicked += OnOpenWSClick;

            _inputContainer = container.Q("input-container");
            _inputContainer.Bind(_editorItem);

            // container.Q<TextField>().RegisterValueChangedCallback(
            //     (val) =>
            //     {
            //         if (_itemConainer.selectedItem != null)
            //         {
            //             (_itemConainer.selectedItem as ExtendItem).title = val.newValue;
            //             _itemConainer.Refresh();
            //         }
            //     });

            if (_newItems.Count > 0)
            {
                _itemConainer.selectedIndex = 0;
            }

            CheckButtons();
        }

        void CheckButtons()
        {
            if (_newItems.Count == 0)
            {
                _btnDel.SetEnabled(false);
                _btnUp.SetEnabled(false);
                _btnDown.SetEnabled(false);
            }
            else
            {
                _btnDel.SetEnabled(true);
                _btnUp.SetEnabled(_newItems.Count > 1 && _itemConainer.selectedIndex > 0);
                _btnDown.SetEnabled(_newItems.Count > 1 && _itemConainer.selectedIndex < _newItems.Count - 1);
            }

            _btnTest.SetEnabled(_itemConainer.selectedIndex >= 0);
        }

        void OnBindItem(VisualElement e, int index)
        {
            var label = e as Label;
            label.userData = _newItems[index];
            label.text = _newItems[index].title;
        }

        void OnItemChose(object obj)
        {
            
        }

#if UNITY_2021_2_OR_NEWER
        void OnSelectionChanged(IEnumerable<object> objs)
#else
        void OnSelectionChanged(List<object> objs)
#endif
        {
            if (_prevItem != null)
            {
                _tempExtendItem.To(_prevItem);
            }
            
            if (_itemConainer.selectedIndex >= 0 && _newItems.Count > 0)
            {
                var item = _newItems[_itemConainer.selectedIndex];
                if (item != null)
                {
                    _tempExtendItem.From(item);
                    _prevItem = item;
                }
            }
            
            CheckButtons();
        }

        void OnAddClick()
        {
            _newItems.Add(new ExtendItem()
            {
                title = "Tools/New Tool " + _newItems.Count
            });

            _itemConainer.Refresh();

            _itemConainer.selectedIndex = _newItems.Count - 1;

            CheckButtons();
        }
        
        void OnDelClick()
        {
            var oldIndex = _itemConainer.selectedIndex;
            if (oldIndex < 0)
            {
                return;
            }

            _newItems.RemoveAt(oldIndex);
            
            _itemConainer.Refresh();
            
            if (_newItems.Count > 0)
            {
                _itemConainer.selectedIndex = Mathf.Max(oldIndex - 1, 0);
            }

            CheckButtons();
        }

        void OnUpClick()
        {
            if (_itemConainer.selectedIndex > 0)
            {
                var obj = _newItems[_itemConainer.selectedIndex];
                _newItems.RemoveAt(_itemConainer.selectedIndex);
                _newItems.Insert(_itemConainer.selectedIndex-1, obj);
                _itemConainer.selectedIndex--;
                
                _itemConainer.Refresh();
                CheckButtons();
            }
        }

        void OnDownClick()
        {
            if (_itemConainer.selectedIndex < _newItems.Count)
            {
                var obj = _newItems[_itemConainer.selectedIndex];
                _newItems.RemoveAt(_itemConainer.selectedIndex);
                _newItems.Insert(_itemConainer.selectedIndex+1, obj);
                _itemConainer.selectedIndex++;
                
                _itemConainer.Refresh();
                CheckButtons();
            }
        }

        void OnOpenCmdClick()
        {
            _tempExtendItem.command = EditorUtility.OpenFilePanel("select command", Application.dataPath, "");
        }

        void OnOpenWSClick()
        {
            _tempExtendItem.workspace = EditorUtility.OpenFolderPanel("select workspace", Application.dataPath, "");
        }

        void OnTest()
        {
            _tempExtendItem.ToItem().Run();
        }

        void Save(bool close)
        {
            if (_prevItem != null)
            {
                _tempExtendItem.To(_prevItem);
            }
            
            ToolExtends.Instance.items = _newItems;
            ToolExtends.Instance.Save();
            
            if (close)
            {
                Close();
            }
        }
    }
}