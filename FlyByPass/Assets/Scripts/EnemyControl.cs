using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private GameObject player;
    [SerializeField] private float destroyTimer;
    [SerializeField] private List<GameObject> stackList;
    [SerializeField] private Transform playerParent;
    [SerializeField] private Transform childPlayer;
    [SerializeField] private Transform chestChild;
    [SerializeField] private bool stackBool;
    //[SerializeField] private TextMesh stackCountText;
    [SerializeField] private bool isOnRoad;
    [SerializeField] private bool wingBool;
    [SerializeField] private bool flyBool;
    [SerializeField] private bool addForceBool;
    [SerializeField] private bool enemyPlayBool;
    private Rigidbody rb;
    private Animator anim;
    [SerializeField] private List<Vector3> wayPointsList;
    [SerializeField] private int speed;
    [SerializeField] private int wayPointIndex;
    [SerializeField] private bool wayPointBool;
    private float dist;
    private float lastDist;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool(StringClass.TAG_ISIDLE, true);
        wayPointIndex = 0;
        enemyPlayBool = false;

        //for (int i = 1; i < 8; i++)
        //{
        //    wayPoints.Add(GameObject.Find("Sphere (" + i.ToString() + ")").transform);
        //}
    }

    private void Start()
    {
        wayPointsList.Add(new Vector3(Random.Range(-11f, 12f), 7.48f, -204.5f));
        wayPointsList.Add(new Vector3(Random.Range(-25f, 90f), 7.48f, 100f));
        wayPointsList.Add(new Vector3(Random.Range(165f, 190f), 7.48f, 78f));
        wayPointsList.Add(new Vector3(Random.Range(159f, 195f), 7.48f, Random.Range(-56.1f, 17)));
        wayPointsList.Add(new Vector3(244.5f, 7.48f, -38.9f));
        wayPointsList.Add(new Vector3(244.5f, 7.48f, 53f));
        wayPointsList.Add(new Vector3(368.4f, 7.48f, Random.Range(60, 88)));
        wayPointsList.Add(new Vector3(Random.Range(345f, 401), 7.49f, 380f));
    }

    private void Update()
    {
        if (enemyPlayBool)
        {
            if (wayPointIndex < 8)
            {
                //transform.LookAt(new Vector3( wayPoints[wayPointIndex].transform.position.x, transform.position.y, wayPoints[wayPointIndex].transform.position.z));
                transform.DOLookAt(new Vector3( wayPointsList[wayPointIndex].x, transform.position.y, wayPointsList[wayPointIndex].z), 0.5f, AxisConstraint.None, Vector3.zero);
                dist = Vector3.Distance(transform.localPosition, wayPointsList[wayPointIndex]);
                if (dist < 15)
                {
                    IncreaseIndex();
                }
            }
                lastDist = Vector3.Distance(transform.localPosition, wayPointsList[wayPointsList.Count-1]);
                if (lastDist<2)
                {
                    transform.DOLookAt(new Vector3(380, transform.position.y, 2250), 0.5f, AxisConstraint.None, Vector3.zero);
                }

            Patrol();
        }


        {
            if (flyBool && stackList.Count < 1)
            {
                anim.SetBool(StringClass.TAG_ISFLYING, false);
                anim.SetBool(StringClass.TAG_ISFALLING, true);
            }
            //stackCountText.text = stackList.Count.ToString();

            if (stackList.Count < 1)
            {
                wingBool = false;
                rb.drag = 0;
            }
            if (wingBool)
            {
                Invoke("Drag", 1);
            }
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                destroyTimer = 0.5f;
            }
            if (Input.GetMouseButtonDown(0))
            {
                enemyPlayBool = true;
                settings.isPlaying = true;
                anim.SetBool(StringClass.TAG_ISRUNNING, true);
                anim.SetBool(StringClass.TAG_ISIDLE, false);
            }
            if (transform.position.y < 7 && isOnRoad)
            {
                childPlayer.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                anim.SetBool(StringClass.TAG_ISRUNNING, false);
                anim.SetBool(StringClass.TAG_ISFLYING, true);
                isOnRoad = false;
            }
            if (!isOnRoad && flyBool)
            {
                StackWing();

                if (addForceBool == true && stackList.Count > 0)
                {
                    rb.AddForce(transform.up * 1300);
                    addForceBool = false;
                }
            }
            if (!isOnRoad)
            {

                    if (wingBool && destroyTimer <= 0.02f && stackList.Count > 0)
                    {
                        GameObject destroyingObject = stackList[stackList.Count - 1];

                        if (destroyingObject != null)
                        {
                            stackList.Remove(destroyingObject);
                            destroyingObject.transform.parent = null;
                            destroyingObject.AddComponent<Rigidbody>();
                            destroyingObject.GetComponent<Rigidbody>().velocity = new Vector3(0, -10, 0);
                            destroyingObject.GetComponent<Renderer>().material.DOFade(0, 1f).OnComplete(() => Destroy(destroyingObject));
                        }
                    }
            }
            for (int i = stackList.Count - 1; i >= 0; i--)
            {
                if (stackList[i] == null)
                {
                    stackList.Remove(stackList[i]);
                }
            }
        }
    }

    void IncreaseIndex()
    {
        //if (wayPointIndex +1 < wayPointsList.Count)
        {
            wayPointIndex++;
            //transform.LookAt(wayPointsList[wayPointIndex]);
        }

    }   

    void Patrol()
    {
        rb.velocity = (transform.forward * speed + new Vector3(0,rb.velocity.y, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("WayPoint"))
        //{
        //    if (wayPointIndex < 7)
        //    {
        //        transform.LookAt(wayPointsList[wayPointIndex]);
        //    }
        //    wayPointIndex++;
        //    other.gameObject.SetActive(false);
        //}

        if (other.CompareTag(StringClass.TAG_ROAD))
        {
            flyBool = false;
            wingBool = false;
            isOnRoad = true;
            childPlayer.rotation = transform.rotation;
            rb.drag = 0;
            addForceBool = true;

            if (Input.GetMouseButtonDown(0))
            {
                settings.isPlaying = true;
                anim.SetBool(StringClass.TAG_ISRUNNING, true);
                anim.SetBool(StringClass.TAG_ISIDLE, false);

            }
            if (stackList.Count < 1)
            {
                anim.SetBool(StringClass.TAG_ISFALLING, false);
                anim.SetBool(StringClass.TAG_ISBIGJUMP, true);
                Invoke("FallRunAnim", 1);
            }
            else
            {
                anim.SetBool(StringClass.TAG_ISFLYING, false);
                anim.SetBool(StringClass.TAG_ISRUNNING, true);
            }

            StackCollect();
        }
        if (stackList.Count <= 149)
        {
            if (other.CompareTag(StringClass.TAG_STACK) && isOnRoad)
            {
                stackList.Add(other.gameObject);
                other.GetComponent<Collider>().enabled = false;
                StackCollect();
            }
        }
        if (other.CompareTag(StringClass.TAG_FINISHPLANE) || other.CompareTag(StringClass.TAG_X1) || other.CompareTag("Target"))
        {
            if (stackList.Count < 1)
            {
                anim.SetBool(StringClass.TAG_ISFALLING, false);
                anim.SetBool(StringClass.TAG_ISHARDLANDING, true);
                Invoke("LandDanceAnim", 1);
            }
            else
            {
                anim.SetBool(StringClass.TAG_ISFLYING, false);
                anim.SetBool(StringClass.TAG_ISDANCING, true);
                childPlayer.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            rb.isKinematic = true;
            wingBool = false;
            flyBool = false;
            StackCollect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringClass.TAG_ROAD))
        {
            flyBool = true;
        }
    }

    private void StackCollect()
    {
        wingBool = false;
        //if (flyBool)
        //{
        //    rb.drag = 0;
        //}

        for (int i = 0; i < stackList.Count; i++)
        {
            if (i % 2 == 0)
            {
                stackList[i].transform.parent = playerParent;
                stackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f);
                stackList[i].transform.localEulerAngles = new Vector3(90, 90, 0);
                stackList[i].transform.DOLocalMove(Vector3.back * stackList.Count * 0.00002f + Vector3.back * 0.00005f * i + Vector3.up * 0.0015f + new Vector3(0.95f, 0, 0) * 0.00125f, 0.2f).
                    OnComplete(() => wingBool = false);
                //stackCountText.text = stackList.Count.ToString();
            }
            else
            {
                stackList[i].transform.parent = playerParent;
                stackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f);
                stackList[i].transform.localEulerAngles = new Vector3(90, 90, 0);
                stackList[i].transform.DOLocalMove(Vector3.back * stackList.Count * 0.00002f + Vector3.back * 0.00005f * i + Vector3.up * 0.0015f + new Vector3(-0.95f, 0, 0) * 0.00125f, 0.2f).
                    OnComplete(() => wingBool = false);
                //stackCountText.text = stackList.Count.ToString();
            }
        }
    }

    private void StackWing()
    {
        //Invoke("Drag", 1);
        wingBool = true;

        for (int i = 0; i < stackList.Count; i += 1)
        {
            if (i % 2 == 0)
            {
                stackList[i].transform.parent = chestChild;
                stackList[i].transform.DOLocalMove(new Vector3(0 + (-i * 0.001f), 0.002f, 0.00182f + (-i * i * 0.00001f)), 0.2f);
                stackList[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                stackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f + (i * 0.0002f));
            }
            else
            {
                stackList[i].transform.parent = chestChild;
                stackList[i].transform.DOLocalMove(new Vector3(0.00315f + (i * 0.001f), 0.002f, 0.00182f + (-i * i * 0.00001f)), 0.2f).OnComplete(() => wingBool = true);
                stackList[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                stackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f + (i * 0.0002f));
            }
        }
    }
    private void Drag()
    {
        rb.drag = 4;
    }
    void FallRunAnim()
    {
        anim.SetBool(StringClass.TAG_ISBIGJUMP, false);
        anim.SetBool(StringClass.TAG_ISRUNNING, true);
    }
    void LandDanceAnim()
    {
        anim.SetBool(StringClass.TAG_ISHARDLANDING, false);
        anim.SetBool(StringClass.TAG_ISDANCING, true);
        childPlayer.transform.DORotate(new Vector3(0, 180, 0), 8);
    }
}
