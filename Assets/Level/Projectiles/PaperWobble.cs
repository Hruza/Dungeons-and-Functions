using UnityEngine;

public class PaperWobble : MonoBehaviour
{
    private float t;
    Mesh mesh;
    Vector3[] vertices;
    public float spacialFrequency=2;
    public float timeFrequency = 5;
    public float amplitude = 0.1f;
    public Quaternion rotation;

    void Start()
    {
        t = 0;
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].z = amplitude * Mathf.Sin(timeFrequency * t + (spacialFrequency * Vector3.Dot(vertices[i], rotation * Vector3.right))); 
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
