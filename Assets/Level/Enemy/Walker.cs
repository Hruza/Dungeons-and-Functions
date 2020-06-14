using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Navigator
{
    protected IEnumerator currentWalk;
    public float maxSpeed = 50;
    public float walkForce = 50;
    public float giveUpTime = 5;
    private Vector2 detectionSize;
    public float followPeriod = 0.25f;
    public enum FollowType {basic, persistent, allknowing }
    public FollowType followType = FollowType.persistent;

    public bool IsWalking {
        get {
            return currentWalk != null;
        }
    }

    public override void Start() {
        base.Start();
        currentWalk = null;
        detectionSize=GetComponent<Collider2D>().bounds.size;
    }


    override public void GoToTarget(GameObject target, float tolerance)
    {
        if (currentWalk != null)
        {
            StopCoroutine(currentWalk);
        }
        currentWalk = Follow(target, tolerance);
        StartCoroutine(currentWalk);
    }

    override public void GoToTarget(Vector3 target, float tolerance)
    {
        if (currentWalk != null) {
            StopCoroutine(currentWalk);
        }
        currentWalk = Walk(target, tolerance);
        StartCoroutine(currentWalk);
    }

    private Vector2 Planify(Vector3 vect) {
        return new Vector2(vect.x,vect.y);
    }

    private IEnumerator Follow(GameObject target, float tolerance) {
        float timeSicneLastSeen = 0;
        IEnumerator currentPartialWalk = null;
        Vector3 lastPos= target.transform.position;
        while ((Planify(target.transform.position - transform.position)).sqrMagnitude > tolerance * tolerance && (followType == FollowType.persistent || timeSicneLastSeen <= giveUpTime))
        {
            if (followType == FollowType.allknowing) {
                if(currentPartialWalk!=null)
                    StopCoroutine(currentPartialWalk);
                currentPartialWalk = Walk(target.transform.position, tolerance);
                StartCoroutine(currentPartialWalk);
            }
            else if (currentPartialWalk != null)
            {
                if (IsInLineOfSight(target) || (followType == FollowType.persistent && timeSicneLastSeen > giveUpTime) || (transform.position-lastPos).sqrMagnitude<=tolerance*tolerance)
                {
                    timeSicneLastSeen = 0;
                    lastPos = target.transform.position;
                }
                else
                    timeSicneLastSeen += followPeriod;

                StopCoroutine(currentPartialWalk);
                currentPartialWalk = Walk(lastPos, tolerance);
                StartCoroutine(currentPartialWalk);
            }
            else if (followType == FollowType.persistent)
            {
                Debug.Log("He was there, searching for him");
                if (!IsInLineOfSight(target))
                    timeSicneLastSeen += 0.5f;
                currentPartialWalk = Walk(target.transform.position, tolerance);
                StartCoroutine(currentPartialWalk);
            }
                yield return new WaitForSeconds(followPeriod);
        }
        if (currentPartialWalk != null)
        {
            StopCoroutine(currentPartialWalk);
        }
        if ((target.transform.position - transform.position).sqrMagnitude > tolerance * tolerance && timeSicneLastSeen > giveUpTime)
        {
            Debug.Log("Target lost");
            SendOutput(WalkingOutput.gaveUp);
        }
        else
            Debug.Log("Target Found!!");
            SendOutput(WalkingOutput.success);
        yield return null;
    }

    private IEnumerator Walk(Vector3 target, float tolerance, bool sendResult=true)
    {
        float startTime = Time.realtimeSinceStartup;
        Vector2 dir;
        Vector2 acceleration;
        float norm;
        while (Vector2.SqrMagnitude(target - transform.position) > tolerance * tolerance && Time.realtimeSinceStartup - startTime < giveUpTime)
        {
            dir = target - transform.position;
            acceleration =(maxSpeed*dir.normalized)- rb.velocity;
            norm = acceleration.magnitude;
            if (Vector2.SqrMagnitude(target - transform.position) >(detectionSize.x+ (defaultTargetTolerance))*( detectionSize.y + (defaultTargetTolerance))) {
                ExtDebug.DrawBoxCast2D(Planify(transform.position) + (0.5f * detectionSize.x * dir.normalized), detectionSize, 0, dir, defaultTargetTolerance , Color.red);
                if (Physics2D.BoxCast(Planify(transform.position) + (0.5f * detectionSize.x * dir.normalized), detectionSize, 0, dir, defaultTargetTolerance, LayerMask.GetMask("Map")))
                {
                    switch (obstacleAvoidance)
                    {
                        case Avoidance.none:
                            yield break;
                        case Avoidance.avoidNearest:
                            if(rb.velocity!=Vector2.zero)
                                acceleration = norm * NewDirection(rb.velocity);
                            else
                                acceleration = norm * NewDirection(dir);
                            break;
                        default:
                            break;
                    }
                }
            }
            if (norm > 0)
            {
                rb.AddRelativeForce(Mathf.Min(1, walkForce / norm) * acceleration * 100 * Time.fixedDeltaTime);
            }
            yield return new WaitForFixedUpdate();
        }
        if (Time.realtimeSinceStartup - startTime >= giveUpTime)
        {
            if(sendResult) SendOutput(WalkingOutput.success);
        }
        else
        {
            if(sendResult) SendOutput(WalkingOutput.success);
        }
        yield return null;
    }

    private Vector2 ParalelDirection(Vector2 dir) {
        Vector2 newDir=Vector2.Perpendicular(dir);
        RaycastHit2D hit= Physics2D.Raycast(transform.position, dir, detectionSize.x+ defaultTargetTolerance *2, LayerMask.GetMask("Map"));
        Debug.DrawRay(transform.position, dir,Color.blue);
        Debug.DrawRay(hit.point,hit.normal,Color.cyan);
        if (hit!=null) {
            newDir = (dir - (Vector2.Dot(dir, hit.normal) * hit.normal)).normalized;
            Debug.DrawRay(transform.position, newDir,Color.green);
            return newDir;
        }
        return newDir;
    }

    private Vector2 NewDirection(Vector2 dir)
    {
        int raycount = 7;
        Vector2 newDir = Vector2.Perpendicular(dir);
        RaycastHit2D hit;
        Quaternion rot;
        for (int i = 0; i < raycount; i++)
        {
            rot = Quaternion.Euler(0,0, (4*((i % 2)-0.5f)*((i + 1)/2)/(raycount-1))*120);
            Debug.DrawRay(transform.position, (detectionSize.x + defaultTargetTolerance * 2)*(rot*dir).normalized, Color.blue);

            bool possible = !Physics2D.CircleCast(transform.position + (0.5f * detectionSize.x * (rot * dir).normalized), 0.4f*(detectionSize.x+detectionSize.y), rot * dir, defaultTargetTolerance * 2, LayerMask.GetMask("Map"));
            ExtDebug.DrawBoxCast2D(transform.position + (0.5f * detectionSize.x * (rot * dir).normalized), detectionSize, 0, rot * dir, defaultTargetTolerance *2,possible?Color.green:Color.blue);
            if (possible)
            {
                Debug.DrawRay(transform.position, rot * dir, Color.green);
                return rot * dir.normalized;
            }
        }
        return -dir.normalized;
    }

}


public static class ExtDebug
{
    public static void DrawBoxCast2D(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, Color color)
    {
        Quaternion angle_z = Quaternion.Euler(0f, 0f, angle);
        DrawBoxCastBox(origin, size / 2f,  angle_z, direction, distance, color);
    }

    //Draws just the box at where it is currently hitting.
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }
    public static void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }

    public struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    //This should work for all cast types
    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
    {
        return origin + (direction.normalized * hitInfoDistance);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }
}