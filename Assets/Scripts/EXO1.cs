using System;
using System.Collections.Generic;
using UnityEngine;

public class EXO1 : MonoBehaviour
{

    [SerializeField] GameObject cube;
    List<Compo> cubes;
    List<Sphere> spheres;
    [SerializeField] int maxDepth = 1;
    [SerializeField] float résolution;


    [System.Serializable]
    public struct Sphere
    {
        public Vector3 center;
        public float radius;
    }

    [System.Serializable]
    public struct Compo
    {
        public Vector3 position;
        public int div;
        public GameObject obj;
    }

    void Start()
    {
        GameObject first=Instantiate(cube, new Vector3(0, 0, 0), Quaternion.identity);
        Compo firstCompo = new Compo();
        firstCompo.position = first.transform.position;
        firstCompo.div = 1;
        firstCompo.obj = first;
        cubes = new List<Compo>();
        cubes.Add(firstCompo);
        Sphere sphere1 = new Sphere();
        sphere1.center = new Vector3(0, 0, 0);
        sphere1.radius = 0.5f;
        Sphere sphere2 = new Sphere();
        sphere2.center = new Vector3(-0.1f, 0, 0);
        sphere2.radius = 0.2f;
        spheres = new List<Sphere>();
        spheres.Add(sphere1);
        spheres.Add(sphere2);
        IntersectSphere();
        ResolutionChange();
    }

    void CreateSphere()
    {

        for (int depth = 0; depth < maxDepth; depth++)
        {
            for (int i = cubes.Count - 1; i >= 0; i--)
            {
                Compo c = cubes[i];
                float size = c.obj.transform.localScale.x;
                float halfDiagonal =(float)Math.Sqrt(2*Math.Pow(size/2f,2));
                float minDist = float.MaxValue;
                float radius = 0f;
                for (int j=0;j<spheres.Count;j++)
                {
                    Sphere s = spheres[j];
                    float currdist = Vector3.Distance(s.center, c.position);
                    currdist -= s.radius;
                    if (currdist < minDist)
                    {
                        minDist = currdist;
                        radius = s.radius;
                    }
                }

                bool intersectsSphere = minDist <= radius+halfDiagonal;
                if (halfDiagonal/1.4 + minDist <= radius)
                {
                    
                }
                else if(intersectsSphere)
                {
                    Divide(i);
                }
                else
                {
                    Destroy(c.obj);
                    cubes.RemoveAt(i);
                }
            }
        }
    }

    void IntersectSphere()
    {
        for (int depth = 0; depth < maxDepth; depth++)
        {
            for (int i = cubes.Count - 1; i >= 0; i--)
            {
                Compo c = cubes[i];
                float size = c.obj.transform.localScale.x;
                float halfDiagonal = (float)Math.Sqrt(2 * Math.Pow(size / 2f, 2));
                int count = 0;
                for (int j = 0; j < spheres.Count; j++)
                {
                    Sphere s = spheres[j];
                    float currdist = Vector3.Distance(s.center, c.position);
                    float radius = s.radius;
                    if (currdist <= s.radius + halfDiagonal)
                    {
                        count++;
                    }
                }

                if (count >=spheres.Count)
                {
                    Divide(i);
                }
                else
                {
                    Destroy(c.obj);
                    cubes.RemoveAt(i);
                }
            }
        }
    }


    void Divide(int index)
    {
        Vector3 position = cubes[index].position;
        float oldSize = cubes[index].obj.transform.localScale.x;

        GameObject oldObj = cubes[index].obj;

        int oldDiv = cubes[index].div;

        GameObject[] newCubes = new GameObject[8];

        float newSize = oldSize / 2f;
        float offsetDist = oldSize / 4f;

        int i = 0;
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 offset = new Vector3(x, y, z) * offsetDist;
                    Vector3 newPos = position + offset;

                    GameObject inst = Instantiate(cube, newPos, Quaternion.identity);
                    inst.transform.localScale = new Vector3(newSize, newSize, newSize);

                    newCubes[i++] = inst;
                }
            }
        }

        Destroy(oldObj);
        cubes.RemoveAt(index);

        for (int j = 0; j < 8; j++)
        {
            Compo newCompo = new Compo();
            newCompo.position = newCubes[j].transform.position;
            newCompo.obj = newCubes[j];
            newCompo.div = oldDiv + 1;
            cubes.Add(newCompo);
        }
    }

    void ResolutionChange()
    {
        for (int i = cubes.Count - 1; i >= 0; i--)
        {
            GameObject c = cubes[i].obj;
            c.transform.localScale = c.transform.localScale * résolution;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered by " + other.name);
    }
}
