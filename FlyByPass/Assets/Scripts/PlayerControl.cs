using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerControl : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject speedLine;
    [SerializeField] private GameObject barTriangle;
    [SerializeField] private float forceTimer;
    [SerializeField] private float destroyTimer;
    [SerializeField] private List<GameObject> stackList;
    [SerializeField] private Transform playerParent;
    [SerializeField] private Transform childPlayer;
    [SerializeField] private Transform chestChild;
    [SerializeField] private bool stackBool;
    [SerializeField] private TextMesh stackCountText;
    [SerializeField] private bool isOnRoad;
    [SerializeField] private bool wingBool;
    [SerializeField] private bool flyBool;
    [SerializeField] private bool barBool;
    [SerializeField] private CoinScript coinScript;
    private Rigidbody rb;
    private Animator anim;
    private Vector3 mousePos;
    private Vector3 firstPos;
    private Vector3 diff;
    private Vector3 finishLinePos = new Vector3(362, 4, 267);
    [SerializeField] private float dist;
    [SerializeField] private float dist2;
    [SerializeField] private Camera ortho;

    public List<GameObject> StackList { get => stackList; set => stackList = value; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool(StringClass.TAG_ISIDLE, true);
        settings.isPlaying = false;
        coinScript = FindObjectOfType<CoinScript>();
        barBool = true;
    }

    private void Update()
    {
        if (flyBool && StackList.Count < 1)
        {
            anim.SetBool(StringClass.TAG_ISFLYING, false);
            anim.SetBool(StringClass.TAG_ISFALLING, true);
        }
        stackCountText.text = StackList.Count.ToString();

        if (StackList.Count < 1)
        {
            wingBool = false;
            rb.drag = 0;
        }
        if (wingBool)
        {
            rb.drag = 4;
            speedLine.SetActive(true);
            Move(transform.forward);
        }
        else
        {
            speedLine.SetActive(false);
        }
            destroyTimer -= Time.deltaTime;
        if (destroyTimer<=0)
        {
            destroyTimer = 0.5f;
        }
        if (Input.GetMouseButtonDown(0))
        {
            settings.isPlaying = true;
            anim.SetBool(StringClass.TAG_ISRUNNING, true);
            anim.SetBool(StringClass.TAG_ISIDLE, false);
        }
        if (settings.isPlaying == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseDown(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                MouseHold(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                MouseUp();
            }
        }
        if (transform.position.y < 7 && isOnRoad)
        {
            childPlayer.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            anim.SetBool(StringClass.TAG_ISRUNNING, false);
            anim.SetBool(StringClass.TAG_ISFLYING, true);
            isOnRoad = false;
        }
        if (Input.GetMouseButtonDown(0) && !isOnRoad && flyBool)
        {
            StackWing();

            if (forceTimer > 0 && StackList.Count > 0)
            {
               rb.AddForce(transform.up * 5100);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            StackCollect();

            if (rb.drag == 4)
            {
                rb.drag = 0;
            }
        }
        if (!isOnRoad)
        {
            if (Input.GetMouseButton(0))
            {
                forceTimer -= Time.deltaTime;

                if (wingBool && destroyTimer <= 0.02f)
                {
                    GameObject destroyingObject = StackList[StackList.Count - 1];
          
                    if (destroyingObject != null)
                    {
                        StackList.Remove(destroyingObject);
                        destroyingObject.transform.parent = null;
                        destroyingObject.AddComponent<Rigidbody>();
                        destroyingObject.GetComponent<Rigidbody>().velocity = new Vector3(0, -10, 0);
                        destroyingObject.GetComponent<Renderer>().material.DOFade(0, 1f).OnComplete(() => Destroy(destroyingObject));
                    }
                }
            }
        }
        for (int i = StackList.Count-1; i >= 0; i--)
        {
            if (StackList[i] == null)
            {
                StackList.Remove(StackList[i]);
            }
        }
        if (barBool && transform.position.y > 5.5f)
        {
            dist = Vector3.Distance(transform.position, finishLinePos);
            dist2 = ((805.5742f - dist) * 9) / 10;
            barTriangle.GetComponent<RectTransform>().anchoredPosition = new Vector2((dist2)-370, 60);
        }
    }

    private void FixedUpdate()
    {
        if (settings.isPlaying == true && !wingBool)
        {
            Move(new Vector3());
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(StringClass.TAG_ROAD))
        {
            flyBool = false;
            wingBool = false;
            isOnRoad = true;
            childPlayer.rotation = transform.rotation;
            rb.drag = 0;
            forceTimer = 0.05f;

            if (Input.GetMouseButtonDown(0))
            {
                settings.isPlaying = true;
                anim.SetBool(StringClass.TAG_ISRUNNING, true);
                anim.SetBool(StringClass.TAG_ISIDLE, false);

            }
            if (StackList.Count<1)
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
        if (StackList.Count <= 149)
        {
            if (other.CompareTag(StringClass.TAG_STACK) && isOnRoad)
            {
                StackList.Add(other.gameObject);
                other.GetComponent<Collider>().enabled = false;
                    StackCollect();
            }
        }
        if (other.CompareTag(StringClass.TAG_FINISHPLANE) || other.CompareTag(StringClass.TAG_X1) || other.CompareTag("Target"))
        {
            if (StackList.Count < 1)
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
            coinScript.CollectButton.SetActive(true);
        }
        if (other.CompareTag(StringClass.TAG_FINISHLINEDEDECTOR))
        {
            barBool = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringClass.TAG_ROAD))
        {
            flyBool = true;
        }
    }

    void Move(Vector3 speed)
    {
        rb.velocity = (transform.forward) * settings.ForwardSpeed + new Vector3(0, rb.velocity.y, 0) + (speed*settings.FlySpeed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + diff.x / 10, transform.eulerAngles.z);
    }

    private void MouseDown(Vector3 inputPos)
    {
        mousePos = ortho.ScreenToWorldPoint(inputPos);
        firstPos = mousePos;
    }

    private void MouseHold(Vector3 inputPos)
    {
        mousePos = ortho.ScreenToWorldPoint(inputPos);
        diff = (mousePos - firstPos) * settings.sensitivity;
    }

    private void MouseUp()
    {
        firstPos = mousePos;
        diff = Vector3.zero;
    }

    private void StackCollect()
    {
        wingBool = false;
        if (flyBool)
        {
            rb.drag = 4;
        }

        for (int i = 0; i < StackList.Count; i++)
        {
            if (i % 2 == 0)
            {
                StackList[i].transform.parent = playerParent;
                StackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f);
                StackList[i].transform.localEulerAngles = new Vector3(90, 90, 0);
                StackList[i].transform.DOLocalMove(Vector3.back * StackList.Count * 0.00002f + Vector3.back * 0.00005f * i + Vector3.up * 0.0015f + new Vector3(0.95f, 0, 0) * 0.00125f, 0.2f).
                    OnComplete(() => wingBool = false);
                stackCountText.text = StackList.Count.ToString();
            }
            else
            {
                StackList[i].transform.parent = playerParent;
                StackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f);
                StackList[i].transform.localEulerAngles = new Vector3(90, 90, 0);
                StackList[i].transform.DOLocalMove(Vector3.back * StackList.Count * 0.00002f + Vector3.back * 0.00005f * i + Vector3.up * 0.0015f + new Vector3(-0.95f, 0, 0) * 0.00125f, 0.2f).
                    OnComplete(() => wingBool = false);
                stackCountText.text = StackList.Count.ToString();
            }
        }
    }

    private void StackWing()
    {
        rb.drag = 0;
        wingBool = true;

        for (int i = 0; i < StackList.Count; i += 1)
        {
            if(i % 2 == 0)
            {
                StackList[i].transform.parent = chestChild;
                StackList[i].transform.DOLocalMove(new Vector3(0 + (-i * 0.001f), 0.002f, 0.00182f + (-i * i * 0.00001f)), 0.2f);
                StackList[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                StackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f +(i*0.0002f));
            }
            else
            {
                StackList[i].transform.parent = chestChild;
                StackList[i].transform.DOLocalMove(new Vector3(0.00315f + (i * 0.001f), 0.002f, 0.00182f + (-i * i * 0.00001f)), 0.2f).OnComplete(() => wingBool = true);
                StackList[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                StackList[i].transform.localScale = new Vector3(0.0003f, 0.002f, 0.009f + (i *0.0002f));
            }
        }
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
