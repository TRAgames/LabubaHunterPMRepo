using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickFinder.Engine.Utility
{
    public static class HierarchyUtility
    {
        public static Transform FindTransform(Transform rootTransform, List<string> hierarchys)
        {
            Transform Find(Transform trans, int ihierarchy)
            {
                if(trans.name == hierarchys[ihierarchy])
                {
                    if(ihierarchy == hierarchys.Count - 1)
                    { return trans; }

                    ihierarchy++;
                    for (int ichild = 0; ichild < trans.childCount; ichild++)
                    {
                        var r = Find(trans.GetChild(ichild), ihierarchy);
                        if (r != null)
                        { return r; }
                    }

                    { return null; }
                }

                return null;
            }

            return Find(rootTransform, 0);
        }

        public static int FindTransforms(Transform rootTransform, List<string> hierarchys, List<Transform> results)
        {
            int count = 0;
            Transform Find(Transform trans, int ihierarchy)
            {
                if (trans.name == hierarchys[ihierarchy])
                {
                    if (ihierarchy == hierarchys.Count - 1)
                    {
                        count++;
                        results.Add(trans);
                        return trans; 
                    }

                    ihierarchy++;
                    for (int ichild = 0; ichild < trans.childCount; ichild++)
                    {
                        var r = Find(trans.GetChild(ichild), ihierarchy);
                    }
                }

                return null;
            }

            Find(rootTransform, 0);
            return count;
        }

        public const string AREA_SPLIT = "%%%";
        public const string PATH_SPLIT = "&&&";

        public static void ParsePath(string path, out string subSceneGuid, out string hostPath, out string subPath)
        {
            subSceneGuid = null; hostPath = null; subPath = null;
            if (path.StartsWith(AREA_SPLIT))
            {
                subSceneGuid = path.Substring(AREA_SPLIT.Length, 32);
                var hostPathBegin = AREA_SPLIT.Length * 2 + 32;
                var hostPathEnd = path.LastIndexOf(AREA_SPLIT);
                hostPath = path.Substring(hostPathBegin, hostPathEnd - hostPathBegin);
                subPath = path.Substring(hostPathEnd + AREA_SPLIT.Length);
            }
            else
            {
                subPath = path;
            }
        }
    }
}

