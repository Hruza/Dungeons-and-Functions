using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {

    public GameObject player;
    public GameObject cam;
    public Portal connectedPortal;
    public float playerDistance;

    public float angle = 0f;

    public float length = 1f;

    public int rayCount = 50;

    public int openingCoefficient = 3;

	public GameObject screen;

    Mesh viewMesh;

    private List<Vector3> points;
    private Vector3 normal;
    private Quaternion rot;
    private GameObject playerCam;

    private class Passer
    {
        public Transform transform;
        public float lastDot;

        public Passer(Transform tr, float dot) {
            transform = tr;
            lastDot = dot;
        }
    }
    private List<Passer> passers;

    void Start() {
        player = Player.player;
        playerCam = player.GetComponent<Player>().cam;
        points = new List<Vector3>();
        rot = Quaternion.Euler(0, 0,angle);
        for (int i = 0; i < rayCount; i++)
        {
            Vector3 newPoint = transform.position - (rot * (Vector3.up * 0.5f * length)) + (rot * (Vector3.up * (1f*i / (rayCount - 1)) * length));
            points.Add(newPoint);
        }
        normal = rot * (-1 * Vector3.right);
        viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        passers = new List<Passer>();
        if (viewMesh!=null) screen.GetComponent<MeshFilter>().mesh = viewMesh;
	}

    private RenderTexture viewTexture;
    void CreateViewTexture() {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            Debug.Log("retexture");
            if (viewTexture != null) {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            viewTexture.depth = 16;
            connectedPortal.cam.GetComponentInChildren<Camera>().targetTexture = viewTexture;

            screen.GetComponent<MeshRenderer>().material.SetTexture("_MainTex",viewTexture);

        }
    }


    private Vector3 playerVector;
    void LateUpdate() {
        playerVector = player.transform.position - transform.position;
        dist = PlayerMetric();
        if (playerDistance > dist && Vector3.Dot(normal, playerVector) > 0)
        {
            connectedPortal.cam.SetActive(true);
            CreateViewTexture();
            DrawPortalView();
        }
        else {
            viewMesh.Clear();
            connectedPortal.cam.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        passers.Add(new Passer(collision.transform, Vector3.Dot(collision.transform.position - transform.position, normal)));
        if (collision.GetComponentInChildren<TrailRenderer>() != null)
        {
            foreach (TrailRenderer trail in collision.GetComponentsInChildren<TrailRenderer>())
            {
                trail.emitting = false;
            }

        }
        Debug.Log(passers.Count);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInChildren<TrailRenderer>() != null)
        {
            foreach (TrailRenderer trail in collision.GetComponentsInChildren<TrailRenderer>())
            {
                trail.emitting = true;
            }

        }
        passers.Remove(passers.Find(x => x.transform == collision.transform));
        Debug.Log(passers.Count);
    }

    private void Update()
    {
        foreach (Passer passer in passers)
        {
            if (passer.lastDot > 0)
            {
                passer.lastDot = Vector3.Dot(passer.transform.position - transform.position, normal);
                if (passer.lastDot < 0) {
                    Vector3 dir=passer.transform.position - transform.position;
                    if (Projection(dir, rot * Vector3.up) - (length / 2) <= 0 && Projection(normal,dir)<1)
                    {
                        //Rigidbody2D rb = passer.transform.GetComponent<Rigidbody2D>();
                        if (passer.transform == player.transform)
                        {
                            Debug.Log("player teleported");
                            playerCam.transform.position = connectedPortal.cam.transform.position;
                        }
                        passer.transform.position = connectedPortal.transform.TransformPoint(transform.InverseTransformPoint(passer.transform.position));
                        //if(rb!=null) 
                    }
                }
            }
            else
            {
                passer.lastDot = Vector3.Dot(passer.transform.position - transform.position, normal);
            }
        }
    }

    private float Projection(Vector3 a, Vector3 b) {
        return Mathf.Abs(Vector3.Dot(a, b));
    }

    private Vector3 realPlayerPos;
    private float PlayerMetric() {
        realPlayerPos = player.transform.position - transform.position;
        return Projection(realPlayerPos, normal) + Mathf.Clamp(Projection(realPlayerPos, rot * Vector3.up) - (length / 2), 0, playerDistance);
    }

    private Vector3 firstDir;
    private float dist;

    void DrawPortalView() {
        connectedPortal.cam.transform.localPosition=transform.InverseTransformPoint(playerCam.transform.position);

        Vector3[] vertices = new Vector3[rayCount*2];
        int[] triangles = new int[(rayCount-1)*6];


        for (int i = 0; i < rayCount; i++)
        {
            vertices[i] = transform.InverseTransformPoint(points[i]);
            vertices[i].z = 0;
        }

        firstDir =  points[0] - player.transform.position;
        firstDir.z = 0;
        Vector3 direction = Mathf.Clamp(playerDistance - dist, 0, playerDistance) * openingCoefficient * firstDir.normalized;

        vertices[rayCount] = transform.InverseTransformPoint(points[0]+direction);
        vertices[rayCount].z = 0;

        Vector3 lastDir = points[rayCount - 1] - player.transform.position;
        lastDir.z = 0;

        float deltaAngle = Vector3.Angle(firstDir, lastDir)/(rayCount-1);
        Quaternion rot = Quaternion.Euler(0,0,deltaAngle);

        for (int i = 0; i < rayCount - 1; i++)
        {
            direction = rot * direction;
            vertices[rayCount + i + 1] = transform.InverseTransformPoint( points[i+1]+direction);
            vertices[rayCount+i+1].z = 0;
            vertices[rayCount].z = vertices[0].z;

            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i+1;
            triangles[i * 6 + 2] = rayCount + i;
            triangles[i * 6 + 3] = i+1;
            triangles[i * 6 + 4] = rayCount + i +1 ;
            triangles[i * 6 + 5] = rayCount + i ;

        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }




}