using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzHuman : MonoBehaviour
{
    public const float LIMIT_IMPULSE = 100f;


    /* Body Parts */
    GameObject armature;
    GameObject spine1, spine2, spine3;
    GameObject leg1L, leg2L, footL;
    GameObject leg1R, leg2R, footR;
    GameObject shoulderL, arm1L, arm2L, handL;
    GameObject shoulderR, arm1R, arm2R, handR;
    GameObject head;
    GameObject oar;
    GameObject boat;

    /* Lassitude */
    bool lassitude = false;

    /* Target Angle (ya = latitude, xa = longitude) */
    float xa = 0f, ya = 0f;
    public float txa = 0f, tya = 0f;
    Vector3 targetPos;

    /* Reserved Action */
    Action action;
    
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeBodyObjects();
        action = new TestHSwing1(this).Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(action != null && !action.RunStep()) {
            action = null;
        }
        if(!lassitude) {
            UpdateTargetPos();
            UpdateArmPos(handL, shoulderL, new Vector3(0, 0, 0));
            UpdateArmPos(handR, shoulderR, new Vector3(0, 0, 0));
            UpdateOarPos();
        }
    }

    public void CascadeImpulse(Vector3 impulse, Vector3 point) {
        if(impulse.magnitude > LIMIT_IMPULSE) {
            MakeLassitude();
        }
        spine2.GetComponent<Rigidbody>().AddForceAtPosition(impulse, point, ForceMode.Impulse);
    }


    private void MakeLassitude() {
        lassitude = true;
        spine1.GetComponent<BoxCollider>().enabled = false;
        spine1.GetComponent<Rigidbody>().mass = 5;
        armature.GetComponent<BoxCollider>().enabled = false;
    }

    
    private void UpdateArmPos(GameObject hand, GameObject shoulder, Vector3 targetOffset) {
        var rHand = hand.GetComponent<Rigidbody>();
        var s = shoulder.transform.position;
        var h = hand.transform.position;
        var a = (h - s).normalized;
        var r = (targetPos + targetOffset - s).normalized;
        var d = (Vector3.Cross(Vector3.Cross(a, r), a)).normalized;
        var m = Mathf.Acos(Vector3.Dot(a, r)) * 10f;
        rHand.velocity = d * m + a * 2f;
    }

    private void UpdateOarPos() {
        oar.transform.position = handL.transform.position;
        var dv = handR.transform.position - handL.transform.position;
        oar.transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), dv.normalized);
    }

    private void UpdateTargetPos() {
        if(txa < -120f) txa = -120f;
        else if(txa > 120f) txa = 120f;
        if(tya < -85f) tya = -85f;
        else if(ya > 85f) tya = 85f;
        xa = (xa + txa) / 2;
        ya = (ya + tya) / 2;
        var sx = Mathf.Sin(Mathf.Deg2Rad * xa);
        var cx = Mathf.Cos(Mathf.Deg2Rad * xa);
        var sy = Mathf.Sin(Mathf.Deg2Rad * ya);
        var cy = Mathf.Cos(Mathf.Deg2Rad * ya);
        targetPos = spine2.transform.position + new Vector3(cy * sx, sy, cy * cx) * 1.5f;
    }


    private void InitializeBodyObjects() {
        var top = transform.parent;
        armature = transform.Find("Armature").gameObject;
        spine1 = armature.transform.Find("spine1").gameObject;
        spine2 = spine1.transform.Find("spine2").gameObject;
        spine3 = spine2.transform.Find("spine3").gameObject;
        leg1L = spine1.transform.Find("leg1_l").gameObject;
        leg2L = leg1L.transform.Find("leg2_l").gameObject;
        footL = leg2L.transform.Find("foot_l").gameObject;
        leg1R = spine1.transform.Find("leg1_r").gameObject;
        leg2R = leg1R.transform.Find("leg2_r").gameObject;
        footR = leg2R.transform.Find("foot_r").gameObject;
        shoulderL = spine2.transform.Find("shoulder_l").gameObject;
        arm1L = shoulderL.transform.Find("arm1_l").gameObject;
        arm2L = arm1L.transform.Find("arm2_l").gameObject;
        handL = arm2L.transform.Find("hand_l").gameObject;
        shoulderR = spine2.transform.Find("shoulder_r").gameObject;
        arm1R = shoulderR.transform.Find("arm1_r").gameObject;
        arm2R = arm1R.transform.Find("arm2_r").gameObject;
        handR = arm2R.transform.Find("hand_r").gameObject;
        head = spine3.transform.Find("head").gameObject;
        oar = top.Find("oar").gameObject;
        boat = null;
    }


    abstract class Action {
        public OzHuman oz;
        public float beginningTime;
        public float lastTime;
        public float duration;
        public Action(OzHuman oz, float duration) {
            this.oz = oz;
            this.duration = duration;
        }
        public Action Start() {
            beginningTime = Time.time;
            lastTime = beginningTime;
            RunStep();
            return this;
        }
        public bool RunStep() {
            Step(Time.time - beginningTime, Time.time - lastTime);
            lastTime = Time.time;
            if(IsFinished()) {
                OnFinish();
                return false;
            }
            return true;
        }
        public virtual void OnStart() {}
        public virtual void Step(float time, float deltaTime) {}
        public virtual void OnFinish() {}
        public bool IsFinished() {
            return Time.time - beginningTime >= duration;
        }
    }

    class TestHSwing1 : Action {
        public const float DURATION = 1f;
        public TestHSwing1(OzHuman oz) : base(oz, DURATION) {}
        public override void OnStart() {
            oz.txa = -90f;
            oz.tya = 0f;
        }
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            if(prog < 0.5f) {
                oz.txa = -90f;
            } else {
                oz.txa = - (prog - 0.5f) / 0.5f * 180 + 90f;
            }
            
            Debug.Log("aa");
        }
    }
}