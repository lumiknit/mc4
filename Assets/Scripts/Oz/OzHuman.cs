using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzHuman : MonoBehaviour
{
    public const float LIMIT_IMPULSE = 5f;

    public Rigidbody rigid;


    /* Body Parts */
    GameObject armature;
    GameObject spine1, spine2, spine3;
    GameObject leg1L, leg2L, footL;
    GameObject leg1R, leg2R, footR;
    GameObject shoulderL, arm1L, arm2L, handL;
    GameObject shoulderR, arm1R, arm2R, handR;
    GameObject head;
    GameObject oar, oarEnd;
    GameObject boat;
    GameObject targetSphere, lhSphere;
    GameObject rotationSphere;

    /* Lassitude */
    public bool lassitude = false;

    /* Target Angle (ya = latitude, xa = longitude) */
    float xa = 0f, ya = 0f, ra = 1.5f;
    public float txa = 0f, tya = 0f, tra = 1.5f;
    public float armSpd = 10f;
    public Vector3 lOff = new Vector3(0, 0, 0);
    Vector3 targetPos;

    /* Reserved Action */
    public Action action;
    public bool rowing = false;
    public int rowingDir = 0;

    
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeBodyObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if(action == null) {
            txa = 20f;
            tya = 0f;
            tra = 3f;
            lOff = Vector3.zero;
            armSpd = 10f;
            rowing = false;
        } else if(!action.RunStep()) {
            action = null;
        }
        if(!lassitude) {
            UpdateTargetPos();
            lhSphere.transform.position = UpdateArmPos(handL, shoulderL, lOff);
            UpdateArmPos(handR, shoulderR, new Vector3(0, 0, 0));
            UpdateOarPos();
            targetSphere.transform.position = targetPos;
        }
        if(boat != null) {
            if(rowing) {
                var ex = oarEnd.transform.position - transform.position;
                var py = 0.2f;
                if(ex.y < py) {
                    var dx = ex - oarEndLastPos;
                    dx.y = 0;
                    var dl = dx.magnitude;
                    var rBoat = boat.GetComponent<Rigidbody>();
                    var point = rBoat.worldCenterOfMass + rBoat.rotation * new Vector3(0.1f, 0f, 0f);
                    Vector3 support;
                    switch(rowingDir) {
                        case 0: support = new Vector3(0, 0, 1); break;
                        case 1: support = new Vector3(0, 0, -1); break;
                        case 2: support = new Vector3(-0.05f, 0, 0); break;
                        case 3: support = new Vector3(0.05f, 0, 0); break;
                        default: support = new Vector3(0, 0, 0); break;
                    }
                    var oriMag = support.magnitude;
                    support = transform.rotation * support;
                    support.y = 0;
                    support = support.normalized * oriMag;
                    rBoat.AddForce(support * dl * dl * 0.2f, ForceMode.VelocityChange);
                    if(rowingDir == 2) {
                        var v = rBoat.velocity.magnitude * 0.0005f + 0.01f;
                        rBoat.AddTorque(new Vector3(0f, -v, 0f), ForceMode.VelocityChange);
                        rBoat.velocity = Quaternion.Euler(0f, -0.5f, 0f) * rBoat.velocity;
                    } else if(rowingDir == 3) {
                        var v = rBoat.velocity.magnitude * 0.0005f + 0.015f;
                        rBoat.AddTorque(new Vector3(0f, v, 0f), ForceMode.VelocityChange);
                        rBoat.velocity = Quaternion.Euler(0f, 0.5f, 0f) * rBoat.velocity;
                    } else {
                        rBoat.velocity *= 0.99f;
                    }
                }
                oarEndLastPos = ex;
            }
            footL.GetComponent<Rigidbody>().velocity += new Vector3(0f, -4f, 0f);
            footR.GetComponent<Rigidbody>().velocity += new Vector3(0f, -4f, 0f);
        }
    }

    Vector3 oarEndLastPos;

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
        SetBoat(null);
        oar.GetComponent<Rigidbody>().useGravity = true;
        oar.GetComponent<BoxCollider>().enabled = false;
    }

    
    private Vector3 UpdateArmPos(GameObject hand, GameObject shoulder, Vector3 targetOffset) {
        var rHand = hand.GetComponent<Rigidbody>();
        var s = shoulder.transform.position;
        var h = hand.transform.position;
        var a = (h - s).normalized;
        var r = (targetPos + transform.rotation * targetOffset - s).normalized;
        var d = (Vector3.Cross(Vector3.Cross(a, r), a)).normalized;
        var m = Mathf.Acos(Vector3.Dot(a, r)) * armSpd;
        rHand.velocity = rigid.velocity + d * m + a * 2f;
        return r + s;
    }

    private void UpdateOarPos() {
        oar.transform.position = handL.transform.position;
        var dv = handR.transform.position - handL.transform.position;
        oar.transform.rotation = Quaternion.FromToRotation(new Vector3(-1, 0, 0), dv.normalized);
    }

    private void UpdateTargetPos() {
        xa = (xa + txa) / 2;
        ya = (ya + tya) / 2;
        ra = (ra + tra) / 2;
        var sx = Mathf.Sin(Mathf.Deg2Rad * xa);
        var cx = Mathf.Cos(Mathf.Deg2Rad * xa);
        var sy = Mathf.Sin(Mathf.Deg2Rad * ya);
        var cy = Mathf.Cos(Mathf.Deg2Rad * ya);
        targetPos = spine3.transform.position + transform.rotation * new Vector3(cy * sx, sy, cy * cx) * ra;
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
        targetSphere = top.transform.Find("TargetSphere").gameObject;
        lhSphere = top.transform.Find("LHSphere").gameObject;
        oar = top.Find("oar").gameObject;
        oarEnd = oar.transform.Find("Endpiece").gameObject;
        boat = top.Find("boat").gameObject;
        SetBoat(boat);
        rigid = GetComponent<Rigidbody>();

        Physics.IgnoreCollision(armature.GetComponent<Collider>(), oar.GetComponent<Collider>());
    }

    public void SetBoat(GameObject boat) {
        this.boat = boat;
        FixedJoint joint;
        if((joint = spine1.GetComponent<FixedJoint>()) != null) {
            Destroy(joint);
        }
        if(boat != null) {
            joint = spine1.AddComponent<FixedJoint>();
            joint.connectedBody = boat.GetComponent<Rigidbody>();
        }
    }


    public abstract class Action {
        public OzHuman oz;
        public float beginningTime;
        public float lastTime;
        public float duration;
        public Action(OzHuman oz, float duration) {
            this.oz = oz;
            this.duration = duration;
            beginningTime = Time.time;
            lastTime = beginningTime;
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
        public virtual void Step(float time, float deltaTime) {}
        public virtual void OnFinish() {}
        public bool IsFinished() {
            return Time.time - beginningTime >= duration;
        }

        public float Progress(float t) { return (t - Time.time) / duration;}
        public float TRange(float prog, float from, float to) {
            if(from <= prog && prog < to) {
                return (prog - from) / (to - from);
            }
            return float.NaN;
        }
    }

    public class TestHSwing1 : Action {
        public const float DURATION = 2f;
        public TestHSwing1(OzHuman oz) : base(oz, DURATION) {}
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            float x;
            if(!float.IsNaN(x = TRange(prog, 0.0f, 0.15f))) {
                oz.txa = 90f;
                oz.tya = 0f;
                oz.tra = 2f;
                oz.lOff = new Vector3(0, 0, 0);
                oz.armSpd = 10f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.15f, 0.25f))) {
                oz.txa = 0f;
                oz.tya = 0f;
                oz.tra = 2f;
                oz.lOff = new Vector3(0.0f, 0.0f, 0.0f);
                oz.armSpd = 15f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.25f, 0.30f))) {
                oz.txa = -90f;
                oz.tya = 0f;
                oz.tra = 2f;
                oz.lOff = new Vector3(1.7f, 0.0f, 0.5f);
                oz.armSpd = 15f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.30f, 0.5f))) {
                oz.txa = -120f;
                oz.tya = 0f;
                oz.tra = 2.5f;
                oz.lOff = new Vector3(1.9f, 0.1f, 0.4f);
                oz.armSpd = 20f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.4f, 0.6f))) {
                oz.txa = -60f;
                oz.tya = -20f;
                oz.tra = 2f;
                oz.lOff = new Vector3(0, 0, 0);
                oz.armSpd = 10f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.6f, 0.9f))) {
                oz.txa = 0f;
                oz.tya = -20f;
                oz.tra = 2f;
                oz.lOff = new Vector3(0, 0, 0);
                oz.armSpd = 10f;
                oz.rowing = false;
            }
        }
    }

    public class TestRowFront : Action {
        public const float DURATION = 1.8f;
        public TestRowFront(OzHuman oz) : base(oz, DURATION) {}
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            float x;
            oz.rowingDir = 0;
            if(!float.IsNaN(x = TRange(prog, 0.0f, 0.125f))) {
                oz.txa = -30f;
                oz.tya = 0f;
                oz.tra = 2f;
                oz.lOff = new Vector3(-0.4f, 0.5f, 0);
                oz.armSpd = 40f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.125f, 0.25f))) {
                oz.txa = -30f;
                oz.tya = -10f;
                oz.tra = 2f;
                oz.lOff = new Vector3(0, 4f, 0);
                oz.armSpd = 20f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.25f, 0.5f))) {
                oz.txa = 60f;
                oz.tya = -10f;
                oz.tra = 2f;
                oz.lOff = new Vector3(0.1f, 2f, 0);
                oz.armSpd = 30f;
                oz.rowing = true;
            } else if(!float.IsNaN(x = TRange(prog, 0.5f, 1.0f))) {
                oz.txa = 30f;
                oz.tya = 10f;
                oz.tra = 1f;
                oz.lOff = new Vector3(0, 0f, 0);
                oz.armSpd = 15f;
                oz.rowing = false;
            }
        }
    }

    public class TestRowBack : Action {
        public const float DURATION = 1.5f;
        public TestRowBack(OzHuman oz) : base(oz, DURATION) {}
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            float x;
            oz.rowingDir = 1;
            if(!float.IsNaN(x = TRange(prog, 0.0f, 0.4f))) {
                oz.txa = 80f;
                oz.tya = -30f;
                oz.tra = 3.5f;
                oz.lOff = new Vector3(-1.8f, 2.1f, -0.0f);
                oz.armSpd = 20f;
                oz.rowing = false;
            } else if(!float.IsNaN(x = TRange(prog, 0.4f, 0.5f))) {
                oz.txa = -30f;
                oz.tya = -20f;
                oz.tra = 4f;
                oz.lOff = new Vector3(-0.8f, 2.3f, -1.6f);
                oz.armSpd = 50f;
                oz.rowing = true;
            } else if(!float.IsNaN(x = TRange(prog, 0.5f, 1f))) {
                oz.txa = -30f;
                oz.tya = -20f;
                oz.tra = 4f;
                oz.lOff = new Vector3(-0.8f, 1f, -1.6f);
                oz.armSpd = 50f;
                oz.rowing = false;
            }
        }
    }

    public class TestRowLeft : Action {
        public const float DURATION = 0.5f;
        public TestRowLeft(OzHuman oz) : base(oz, DURATION) {}
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            float x;
            oz.rowingDir = 2;
            if((int)(prog * 2) % 2 == 0) {
                oz.txa = 24f;
                oz.tya = -30f;
                oz.tra = 3.5f;
                oz.lOff = new Vector3(0f, 1.6f, -0.5f);
                oz.armSpd = 20f;
                oz.rowing = true;
            } else {
                oz.txa = 30f;
                oz.tya = -33f;
                oz.tra = 3.5f;
                oz.lOff = new Vector3(0f, 1.6f, -0.5f);
                oz.armSpd = 20f;
                oz.rowing = true;
            }
        }
    }

    public class TestRowRight : Action {
        public const float DURATION = 0.5f;
        public TestRowRight(OzHuman oz) : base(oz, DURATION) {}
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            float x;
            oz.rowingDir = 3;
            if((int)(prog * 2) % 2 == 0) {
                oz.txa = 30f;
                oz.tya = -30f;
                oz.tra = 3.5f;
                oz.lOff = new Vector3(0f, 1.6f, -0.5f);
                oz.armSpd = 20f;
                oz.rowing = true;
            } else {
                oz.txa = 37f;
                oz.tya = -33f;
                oz.tra = 3.5f;
                oz.lOff = new Vector3(0f, 1.6f, -0.5f);
                oz.armSpd = 20f;
                oz.rowing = true;
            }
        }
    }

    public class TestAction : Action {
        public const float DURATION = 2f;
        public TestAction(OzHuman oz) : base(oz, DURATION) {}
        public override void Step(float t, float dt) {
            float prog = t / DURATION;
            float x;
            if(!float.IsNaN(x = TRange(prog, 0.0f, 1f))) {
                oz.txa = -20f;
                oz.tya = 0f;
                oz.tra = 1.1f;
                oz.lOff = new Vector3(0.5f, 0f, -1f);
                oz.armSpd = 40f;
            }
        }
    }
}