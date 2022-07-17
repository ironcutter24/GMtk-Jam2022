using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

[ExecuteInEditMode]
public class SplinePlacer : PathSceneTool
{
    [SerializeField]
    private List<GameObject> prefabs = new List<GameObject>();

    [SerializeField]
    GameObject holder;

    [SerializeField]
    float spacing = 3;

    [SerializeField]
    private float minionSpeed = 5f;
    public float MinionSpeed { get => minionSpeed; }
    
    const float minSpacing = .1f;

    private void Awake()
    {
        Generate();
    }

    void Generate()
    {
        if (pathCreator != null && prefabs.Count > 0 && holder != null)
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

                int index = counter % prefabs.Count;
                if (prefabs[index] != null)
                {
                    var obj = Instantiate(prefabs[index], point, rot, holder.transform);
                    Follower follower = obj.GetComponent<Follower>();
                    follower.SetPath(this);
                }

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
