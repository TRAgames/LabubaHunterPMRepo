//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickFinder.Editor
{
    public interface INaviTabWindow
    {
        public void Show(System.Object args);
        public VisualElement GetRootView();
    }

    public class NaviVerticalPad : VisualElement
    {
        List<NaviVerticalColumn> columns = new List<NaviVerticalColumn>();
        NaviPathHelper pathHelper;

        public Action<string> onTabChanged;

        public NaviVerticalPad(NaviPathHelper pathHelper, Action<string> onTabChanged)
        {
            this.style.flexDirection = FlexDirection.Row;

            this.pathHelper = pathHelper;
            this.onTabChanged = onTabChanged;

            var rootLevelPaths = pathHelper.GetRootLevelPaths();
            var naviColumn = new NaviVerticalColumn();
            this.Add(naviColumn);
            columns.Add(naviColumn);

            naviColumn.Build(rootLevelPaths, 1, OnNaviSelected);
        }

        void OnNaviSelected(NaviVerticalColumn naviColumn, string path)
        {
            var nextLevelPaths = pathHelper.GetNextLevelPaths(path);

            var removedLevelBegin = nextLevelPaths.Count == 0 ? naviColumn.Level : naviColumn.Level + 1;
            RemoveUnusedColumnsBehind(removedLevelBegin);
            if (nextLevelPaths.Count == 0)
            {
                onTabChanged?.Invoke(path.Substring(path.IndexOf("/")+1));
                return; 
            }

            var navColumn = GetOrCreateNaviColumn(naviColumn.Level + 1);
            navColumn.Build(nextLevelPaths, naviColumn.Level + 1, OnNaviSelected);

            onTabChanged?.Invoke(path);
        }

        void RemoveUnusedColumnsBehind(int level)
        {
            for (int i = columns.Count - 1; i >= 0; i--)
            {
                if (i >= level)
                {
                    var column = columns[i];
                    columns.RemoveAt(i);
                    this.Remove(column);
                }
            }
        }

        NaviVerticalColumn GetOrCreateNaviColumn(int level)
        {
            if (columns.Count < level)
            {
                var column = new NaviVerticalColumn();
                columns.Add(column);
                this.Add(column);
            }
            return columns[level - 1];
        }

        public void SetSelectionWithoutNotify(string path)
        {
            path = NaviPathHelper.ToInternalPath(path);
            for(int i = 0; i < columns.Count; i++)
            {
                columns[i].SetSelection(path, true);
            }
        }
    }

    public class NaviVerticalColumn : VisualElement
    {
        ListView listView;
        List<string> pathList = new List<string>();
        int level;

        public int Level { get { return level; } }

        public NaviVerticalColumn()
        {
            listView = new ListView();
            listView.selectionType = SelectionType.Single;
            listView.style.maxWidth = 200;

            listView.makeItem = () => { return new Label(); };
            listView.bindItem = OnBindNavigateItem;
            listView.itemsSource = pathList;

            this.Add(listView);
        }

        private void OnBindNavigateItem(VisualElement item, int index)
        {
            item.style.unityTextAlign = TextAnchor.MiddleLeft;
            (item as Label).text = pathList[index].Split("/", StringSplitOptions.RemoveEmptyEntries)[level];
        }

        public void SetSelection(int index, bool withoutNotify)
        {
            if (withoutNotify)
                listView.SetSelectionWithoutNotify(new List<int> { index });
            else
                listView.SetSelection(index);
        }

        public void SetSelection(string path, bool withouNitify)
        {
            var pathIndex = pathList.FindIndex(x=> x == path);
            SetSelection(pathIndex, withouNitify);
        }

        public void Build(List<string> pathList, int level, Action<NaviVerticalColumn, string> onItemSelected)
        {
            this.pathList.Clear();
            this.pathList.AddRange(pathList);
            this.level = level;

#if UNITY_2022_2_OR_NEWER
            listView.selectionChanged -= (Action<IEnumerable<object>>)listView.userData;

            Action<IEnumerable<object>> selectAction = (IEnumerable<object> selectList) =>
            {
                var selectedPath = NaviPathHelper.GetPathToDepth(selectList.First() as string, level);
                onItemSelected?.Invoke(this, selectedPath);
            };
            listView.userData = selectAction;
            listView.selectionChanged += selectAction;
            listView.Rebuild();

#elif UNITY_2021_2_OR_NEWER
            listView.onSelectionChange -= (Action<IEnumerable<object>>)listView.userData;
            Action<IEnumerable<object>> selectAction = (IEnumerable<object> selectList) =>
            {
                var selectedPath = TabPaths.GetPathToDepth(selectList.First() as string, level);
                onItemSelected?.Invoke(this, selectedPath);
            };
            listView.userData = selectAction;
            listView.onSelectionChange += selectAction;
            listView.Rebuild();

#else
            listView.onSelectionChanged -= (Action<List<object>>)listView.userData;
            Action<List<object>> selectAction = (List<object> selectList) =>
            {
                var selectedPath = TabPaths.GetPathToDepth(selectList.First() as string, level);
                onItemSelected?.Invoke(this, selectedPath);
            };
            listView.onSelectionChanged += selectAction;
            listView.userData = selectAction;
            listView.Refresh();
#endif
        }
    }

    /// <summary>
    /// internal path begin with "root/"
    /// </summary>
    public class NaviPathHelper
    {
        private List<string> paths = new List<string>();
        private Dictionary<string, Type> path2Types = new Dictionary<string, Type>();
        private Dictionary<Type, string> type2Paths = new Dictionary<Type, string>();

        public void Add<TWindow>(string path) where TWindow : INaviTabWindow
        {
            Add(path, typeof(TWindow));
        }

        public void Add(string path, Type windowType)
        {
            path = ToInternalPath(path);
            paths.Add(path);
            path2Types.Add(path, windowType);
            type2Paths.Add(windowType, path);
        }

        public Type GetWindow(string path)
        {
            path = ToInternalPath(path);
            if(path2Types.TryGetValue(path, out var type)) 
                return type;
            return null;
        }

        public string GetPath(Type windowType)
        {
            if(type2Paths.TryGetValue(windowType, out var path)) 
                return path;
            return null;
        }

        public static string ToInternalPath(string path)
        {
            var newPath = path.Replace("\\", "/");
            if (newPath.StartsWith("/"))
            {
                newPath = newPath.Remove(0);
            }
            if (!newPath.EndsWith('/'))
            {
                newPath += "/";
            }
            if (!newPath.StartsWith("root/"))
            { newPath = "root/" + newPath; }
            return newPath;
        }

        public static string GetPathToDepth(string path, int level)
        {
            var tokens = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var count = System.Math.Min(tokens.Length, level + 1);
            var r = string.Join("/", tokens, 0, count);
            return r;
        }

        public List<string> GetRootLevelPaths()
        {
            return GetNextLevelPaths("root/");
        }

        public List<string> GetChildFullPaths(string parentPath)
        {
            parentPath = ToInternalPath(parentPath);
            var targetPaths = paths.Where(x => x.StartsWith(parentPath)).ToList();

            return targetPaths;
        }

        public List<string> GetNextLevelPaths(string parentPath)
        {
            parentPath = ToInternalPath(parentPath);
            var targetPaths = paths.Where(x => x.StartsWith(parentPath)).ToList();
            var results = new List<string>();
            for (int i = 0; i < targetPaths.Count; i++)
            {
                var path = targetPaths[i].Replace(parentPath, "");
                var tokens = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0)
                { continue; }

                path = parentPath + tokens[0] + "/";
                if (!results.Contains(path))
                { results.Add(path); }
            }
            return results;
        }
    }
}
