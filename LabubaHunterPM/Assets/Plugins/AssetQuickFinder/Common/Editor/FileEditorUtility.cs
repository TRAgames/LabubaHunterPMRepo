//------------------------------------------------------------//
// yanfei 2024.10.27
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

using IOFile = System.IO.File;
using IODirectory = System.IO.Directory;


namespace QuickFinder.Editor
{
    public static class FileEditorUtility
    {
        public static bool IsFile(string assetPath)
        {
            int iie = GetLastIndexOfString(assetPath, '.');
            int iif = GetLastIndexOfString(assetPath, '/');
            if (iie < 0 || iie < iif)
                return false;

            return true;
        }


        public static IEnumerable<string> GetFilesBySystem(string path, bool topDirectoryOnly, bool excludeMetaFile = true)
        {
            var pathList = IODirectory.GetFiles(path, "*", topDirectoryOnly ? System.IO.SearchOption.TopDirectoryOnly : System.IO.SearchOption.AllDirectories).ToList();
            if (excludeMetaFile)
            { pathList.RemoveAll(x => x.EndsWith(".meta")); }

            return pathList;
        }

        public static List<string> GetAssetsByUnity(string[] folders, bool includeFile = true, bool excludeFolder = true)
        {
            var pathList = AssetDatabase.FindAssets("*", folders).Distinct().Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
            if(!includeFile)
                pathList.RemoveAll(x => IsFile(x));
            if(excludeFolder)
                pathList.RemoveAll(x => !IsFile(x));
            return pathList;
        }

        public static List<string> GetAssetsInList(List<string> assetPaths)
        {
            var foundAssets = new List<string>();
            foreach(var assetPath in assetPaths)
            {
                if(IsFile(assetPath))
                {
                    foundAssets.Add(assetPath);
                }
                else
                {
                    var  assetsInFolder = GetAssetsByUnity(new string[] { assetPath });
                    foundAssets.AddRange(assetsInFolder);
                }
            }
            return foundAssets;
        }

        public static void MergeTable(Dictionary<string, HashSet<string>> desTable, Dictionary<string, HashSet<string>> srcTable)
        {
            foreach (var kv in srcTable)
            {
                if (!desTable.TryGetValue(kv.Key, out var assetList))
                {
                    assetList = kv.Value;
                    desTable.Add(kv.Key, assetList);
                }
                else
                {
                    foreach (var assetPath in kv.Value)
                    {
                        if (assetList.Contains(assetPath))
                            continue;
                        assetList.Add(assetPath);
                    }
                }
            }
        }


        public static void SortAssetPathList(IEnumerable<string> pathList, int sortMode)
        {
            if (sortMode == 0)
            {
                if (pathList is List<string> list)
                {
                    list.Sort(CompareExtensionThenName);
                }
                else if (pathList is System.Array array)
                {
                    System.Array.Sort(array as string[], CompareExtensionThenName);
                }
            }
            else if (sortMode == 1)
            {
                if (pathList is List<string> list)
                {
                    list.Sort();
                }
                else if (pathList is System.Array array)
                {
                    System.Array.Sort(array);
                }
            }
        }

        public static int CompareExtensionThenName(string x, string y)
        {
            int iex = GetLastIndexOfString(x, '.');
            int iey = GetLastIndexOfString(y, '.');
            var ifx = GetLastIndexOfString(x, '/') + 1;
            var ify = GetLastIndexOfString(y, '/') + 1;
            iex = iex < 0 ? ifx : iex + 1;
            iey = iey < 0 ? ify : iey + 1;

            var lex = x.Length - iex;
            var ley = y.Length - iey;
            int min = Mathf.Min(lex, ley);
            var re = string.Compare(x, iex, y, iey, min);
            if (re == 0)
                re = lex - ley;

            var lfx = iex - ifx;
            var lfy = iey - ify;
            min = Mathf.Min(lfx, lfy);
            var rf = string.Compare(x, ifx, y, ify, min);
            if (rf == 0)
                rf = lfx - lfy;

            return GetPathCompareScrore(re, rf);
        }

        public static int CompareNameThenExtensionThenFolder(string x, string y)
        {
            int iex = GetLastIndexOfString(x, '.');
            int iey = GetLastIndexOfString(y, '.');
            var ifx = GetLastIndexOfString(x, '/') + 1;
            var ify = GetLastIndexOfString(y, '/') + 1;
            iex = iex < 0 ? ifx : iex + 1;
            iey = iey < 0 ? ify : iey + 1;

            var lex = x.Length - iex;
            var ley = y.Length - iey;
            int min = Mathf.Min(lex, ley);
            var re = string.Compare(x, iex, y, iey, min);
            if (re == 0)
                re = lex - ley;

            var lfx = iex - ifx;
            var lfy = iey - ify;
            min = Mathf.Min(lfx, lfy);
            var rf = string.Compare(x, ifx, y, ify, min);
            if (rf == 0)
                rf = lfx - lfy;

            var ldx = x.Length - lfx;
            var ldy = y.Length - lfy;
            min = Mathf.Min(ldx, ldy);
            var rd = string.Compare(x, 0, y, 0, min);
            if (rd == 0)
                rd = ldx - ldy;

            return GetPathCompareScrore(rf, re, rd);
        }

        public static int CompareExtensionThenNameThenFolder(string x, string y)
        {
            int iex = GetLastIndexOfString(x, '.');
            int iey = GetLastIndexOfString(y, '.');
            var ifx = GetLastIndexOfString(x, '/') + 1;
            var ify = GetLastIndexOfString(y, '/') + 1;
            iex = iex < 0 ? ifx : iex + 1;
            iey = iey < 0 ? ify : iey + 1;

            var lex = x.Length - iex;
            var ley = y.Length - iey;
            int min = Mathf.Min(lex, ley);
            var re = string.Compare(x, iex, y, iey, min);
            if (re == 0)
                re = lex - ley;

            var lfx = iex - ifx;
            var lfy = iey - ify;
            min = Mathf.Min(lfx, lfy);
            var rf = string.Compare(x, ifx, y, ify, min);
            if (rf == 0)
                rf = lfx - lfy;

            var ldx = x.Length - lfx;
            var ldy = y.Length - lfy;
            min = Mathf.Min(ldx, ldy);
            var rd = string.Compare(x, 0, y, 0, min);
            if (rd == 0)
                rd = ldx - ldy;

            return GetPathCompareScrore(re, rf, rd);
        }

        public static int CompareExtensionThenFolderThenName(string x, string y)
        {
            int iex = GetLastIndexOfString(x, '.');
            int iey = GetLastIndexOfString(y, '.');
            var ifx = GetLastIndexOfString(x, '/') + 1;
            var ify = GetLastIndexOfString(y, '/') + 1;
            iex = iex < 0 ? ifx : iex + 1;
            iey = iey < 0 ? ify : iey + 1;

            var lex = x.Length - iex;
            var ley = y.Length - iey;
            int min = Mathf.Min(lex, ley);
            var re = string.Compare(x, iex, y, iey, min);
            if (re == 0)
                re = lex - ley;

            var lfx = iex - ifx;
            var lfy = iey - ify;
            min = Mathf.Min(lfx, lfy);
            var rf = string.Compare(x, ifx, y, ify, min);
            if (rf == 0)
                rf = lfx - lfy;

            var ldx = x.Length - lfx;
            var ldy = y.Length - lfy;
            min = Mathf.Min(ldx, ldy);
            var rd = string.Compare(x, 0, y, 0, min);
            if (rd == 0)
                rd = ldx - ldy;

            return GetPathCompareScrore(re, rd, rf);
        }

        public static int CompareNameWithExtension(string x, string y)
        {
            int iex = GetLastIndexOfString(x, '.');     //:     aaa     aaa.c   aaa/bbb.c 
            int iey = GetLastIndexOfString(y, '.');
            var ifx = GetLastIndexOfString(x, '/') + 1; //:     aaa     aaa.c   aaa/bbb.c
            var ify = GetLastIndexOfString(y, '/') + 1;
            iex = iex < 0 ? ifx : iex + 1;
            iey = iey < 0 ? ify : iey + 1;

            var lex = x.Length - iex;
            var ley = y.Length - iey;
            int min = Mathf.Min(lex, ley);
            var re = string.Compare(x, iex, y, iey, min);
            if (re == 0)
                re = lex - ley;

            var lfx = iex - ifx;
            var lfy = iey - ify;
            min = Mathf.Min(lfx, lfy);
            var rf = string.Compare(x, ifx, y, ify, min);
            if (rf == 0)
                rf = lfx - lfy;

            return GetPathCompareScrore(rf, re);
        }

        public static int CompareNameWithoutExtension(string x, string y)
        {
            int iex = GetLastIndexOfString(x, '.');     //:     aaa     aaa.c   aaa/bbb.c 
            int iey = GetLastIndexOfString(y, '.');
            var ifx = GetLastIndexOfString(x, '/') + 1; //:     aaa     aaa.c   aaa/bbb.c
            var ify = GetLastIndexOfString(y, '/') + 1;
            iex = iex < 0 ? ifx : iex + 1;
            iey = iey < 0 ? ify : iey + 1;

            var lx = iex - ifx;
            var ly = iey - ify;
            var min = Mathf.Min(lx, ly);
            var rf = string.Compare(x, ifx, y, ify, min);
            if (rf == 0)
            { return lx - ly; }
            return rf;
        }

        public static int GetLastIndexOfString(string input, char c)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == c)
                    return i;
            }
            return -1;
        }

        private static int GetPathCompareScrore(int a, int b, int c = 0)
        {
            a = Mathf.Clamp(a, -1, 1);
            b = Mathf.Clamp(b, -1, 1);
            c = Mathf.Clamp(c, -1, 1);
            return a * 8 + b * 4 + c;
        }

        public static void RemoveFilesNotInPatterns(List<string> paths, List<string> patternList)
        {
            if (patternList.Count == 0)
                return;

            for (int i = paths.Count - 1; i >= 0; i--)
            {
                bool found = false;
                foreach (var mustPath in patternList)
                {
                    if (paths[i].StartsWith(mustPath))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    paths.RemoveAt(i);
                }
            }
        }
    }
}

