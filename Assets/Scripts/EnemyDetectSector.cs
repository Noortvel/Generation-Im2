using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectSector : MonoBehaviour
{
    [SerializeField]
    private float angleArg;
    [SerializeField]
    private float radius;
    private LineRenderer lineRender;
    private List<Vector3> positions;
    private float angle;
    private float angleCosinus;

    private Character target;
    private EnemyAIController controller;

    private bool isInit = false;
    public void Initialize(Character character, EnemyAIController ai)
    {
        target = character;
        //radius += target.radius;
        controller = ai;
        
        isInit = true;
    }

    void Awake()
    {
        angle = 90 - angleArg;
        
        lineRender = GetComponent<LineRenderer>();
        positions = new List<Vector3>(10);
        //transform;
        positions.Add(Vector3.zero);
        angleCosinus = Mathf.Cos(angle * Mathf.Deg2Rad);
        float cx = -Mathf.Cos(angle * Mathf.Deg2Rad);
        float cy = Mathf.Sin(angle * Mathf.Deg2Rad);
        positions.Add(new Vector3(cx, 0, cy) * radius);
        float step = 0.1f;
        for(float x = cx; x < -cx; x+=step)
        {
            float y = Mathf.Sqrt(radius * radius - x * x);
            positions.Add(new Vector3(x, 0, y));
        }
        positions.Add(new Vector3(-cx, 0, cy) * radius);
        positions.Add(Vector3.zero);


        lineRender.positionCount = positions.Count;
        lineRender.SetPositions(positions.ToArray());
        
    }
    private bool isSectorCheck = true;
    private bool isCollisionCheck = true;
    private void SectorCheck()
    {
        Vector3 p1 = controller.controlledPawn.center.position;
        p1.y = 0;
        Vector3 p2 = target.center.position;
        p2.y = 0;

        Vector3 dt = (p2 - p1);
        if (dt.magnitude - target.radius <= radius)
        {
            float scp = Vector3.Dot(dt, transform.forward);
            float anglbtw = Vector3.Angle(transform.forward, dt);
            if (Mathf.Abs(scp) < angleCosinus && Mathf.Abs(anglbtw) < 90)
            {
                //Debug.Log("Sector");
                //Debug.Break();

                controller.PlayerDetected();
                
                isSectorCheck = false;
                //isCollisionCheck = true;
            }
        }
    }
    private void CollisionCheck()
    {
        Vector3 p1 = controller.controlledPawn.center.position;
        p1.y = 0;
        Vector3 p2 = target.center.position;
        p2.y = 0;
        Vector3 dt = (p2 - p1);
        if(dt.magnitude < (target.radius + controller.controlledPawn.radius))
        {
            target.gameField.EndGame(GameField.GameEndType.LOSE);
            //Debug.Log("Collision");
            //Debug.Break();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit)
        {
            if (isSectorCheck)
            {
                SectorCheck();
            }
            if (isCollisionCheck)
            {
                CollisionCheck();
            }
            transform.rotation = Quaternion.Euler(controller.controlledPawn.toMoveRotation);
        }
    }
}
