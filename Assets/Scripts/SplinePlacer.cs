using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

[ExecuteInEditMode]
public class SplinePlacer : PathSceneTool
{
    public GameObject prefab;

    public List<GameObject> prefabs = new List<GameObject>();

    public GameObject holder;
    public float spacing = 3;
    
    const float minSpacing = .1f;

    private void Start()
    {
        Generate();
    }

    void Generate()
    {
        if (pathCreator != null && prefab != null && holder != null)
        {
            DestroyObjects();
                
            VertexPath path = pathCreator.path;
                
            spacing = Mathf.Max(minSpacing, spacing);
            float dst = 0;
                
            /*
            while (dst < path.length)
            {
                Vector3 point = path.GetPointAtDistance(dst);
                Quaternion rot = Quaternion.identity;  // path.GetRotationAtDistance(dst);

                var obj = Instantiate(prefab, point, rot, holder.transform);
                Follower follower = obj.GetComponent<Follower>();
                follower.SetPath(pathCreator);

                dst += spacing;
            }
            */

            int counter = prefabs.Count;
            while (dst < path.length)
            {
                Vector3 point = path.GetPointAtDistance(dst);
                Quaternion rot = Quaternion.identity;  // path.GetRotationAtDistance(dst);

                var obj = Instantiate(prefabs[counter % prefabs.Count], point, rot, holder.transform);
                Follower follower = obj.GetComponent<Follower>();
                follower.SetPath(pathCreator);

                dst += spacing;
                counter++;
            }
        }
    }
    
    void DestroyObjects()
    {
        int numChildren = holder.transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(holder.transform.GetChild(i).gameObject, false);
        }
    }

    protected override void PathUpdated()
    {
        if (pathCreator != null)
        {
            Generate();
        }
    }
}
