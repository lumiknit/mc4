using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzAI : MonoBehaviour
{
    GameObject humanObject;
    OzHuman human;

    AI ai;

    // Start is called before the first frame update
    void Start()
    {
        humanObject = transform.Find("human2a").gameObject;
        human = humanObject.GetComponent<OzHuman>();

        ai = new SeekingAI1();

        /*switch(Random.Range(0, 4)) {
            case 0:
                ai = new WanderingAI1();
                break;
            case 1:
                ai = new WanderingAI2();
                break;
            case 2:
                ai = new ChasingAI1(GameObject.Find("Oz").transform.Find("human2a").GetComponent<OzHuman>());
                break;
            case 3:
                ai = new ChasingAI2(GameObject.Find("Oz").transform.Find("human2a").GetComponent<OzHuman>());
                break;
        } */
    }

    // Update is called once per frame
    void Update()
    {
        if(human.lassitude) {
            if(human.transform.position.y < -20) {
                Destroy(human.gameObject.transform.parent.gameObject);
                GameManager.IncreaseKillCount();
            }
        } else if(ai != null) {
            ai.RunStep(this, human);
        }
    }

    public abstract class AI {
        float actionTimer = 0f;
        private OzAI ai;
        private OzHuman human;
        public abstract void Step(OzAI ai, OzHuman human);
        public void SetUp(OzAI ai, OzHuman human) {
            this.ai = ai;
            this.human = human;
        }
        public void RunStep(OzAI ai, OzHuman human) {
            actionTimer -= Time.deltaTime;
            this.ai = ai;
            this.human = human;
            if(actionTimer < 0) {
                Step(ai, human);
            }
        }
        public void PauseFor(float f) {
            actionTimer = f;
        }
        public void RowFront() {
            human.action = new OzHuman.TestRowFront(human);
        }
        public void RowBack() {
            human.action = new OzHuman.TestRowBack(human);
        }
        public void RowLeft() {
            human.action = new OzHuman.TestRowLeft(human);
        }
        public void RowRight() {
            human.action = new OzHuman.TestRowRight(human);
        }
        public void HSwing() {
            human.action = new OzHuman.TestHSwing1(human);
        }
        public bool HasAction() {
            return human.action != null;
        }
        public void ChangeAI(AI newAI) {
            ai.ai = newAI;
        }
    }
    public class SimpleAI1 : AI {
        public override void Step(OzAI ai, OzHuman human) {
            if(!HasAction()) {
                RowFront();
            }
        }
    }

    public class WanderingAI1 : AI {
        float targetVelo = 4f;
        int targetDir = 0;
        public override void Step(OzAI ai, OzHuman human) {
            if(human.rigid.velocity.magnitude < targetVelo) {
                RowFront();
                PauseFor(1f);
                targetDir = Random.Range(0, 2);
            } else {
                if(targetDir == 0) {
                    RowLeft();
                } else {
                    RowRight();
                }
                PauseFor(0.4f);
            }
            targetVelo = Random.Range(3f, 6f);
        }
    }

    public class WanderingAI2 : AI {
        float targetVelo = 4f;
        int targetDir = 0;
        public override void Step(OzAI ai, OzHuman human) {
            if(human.rigid.velocity.magnitude < targetVelo) {
                RowFront();
                PauseFor(1f);
                targetDir = Random.Range(0, 2);
            } else {
                if(targetDir == 0) {
                    RowLeft();
                } else {
                    RowRight();
                }
                PauseFor(0.4f);
            }
            targetVelo = Random.Range(5f, 8f);
        }
    }

    public class ChasingAI1 : AI {
        OzHuman target;
        public ChasingAI1(OzHuman target) {
            this.target = target;
        }
        public override void Step(OzAI ai, OzHuman human) {
            if(target == null) return;
            if(target.lassitude) {
                ChangeAI(new SeekingAI1());
                return;
            }
            Debug.Log("Chasing");
            var d = target.gameObject.transform.position - human.transform.position;
            var v = d.normalized;
            var w = human.transform.rotation * new Vector3(0, 0, 1);
            var dot = Vector3.Dot(v, w);
            var cross = Vector3.Cross(v, w).y;
            //Debug.Log("d = " + d);
            if(d.magnitude < 6f && cross < 0 && dot > 0) {
                HSwing();
                PauseFor(1f);
            } else if(human.rigid.velocity.magnitude < 2f) {
                RowFront();
                PauseFor(1f);
            } else {
                if(dot >= 0.9f) {
                    RowFront();
                    PauseFor(0.8f);
                } else if(cross > 0.1) {
                    RowLeft();
                    PauseFor(0.3f);
                } else {
                    RowRight();
                    PauseFor(0.3f);
                }
            }
        }
    }

    public class ChasingAI2 : AI {
        OzHuman target;
        public ChasingAI2(OzHuman target) {
            this.target = target;
        }
        public override void Step(OzAI ai, OzHuman human) {
            if(target == null) return;
            if(target.lassitude) {
                ChangeAI(new SeekingAI1());
                return;
            }
            var d = target.gameObject.transform.position - human.transform.position;
            var v = d.normalized;
            var w = human.transform.rotation * new Vector3(0, 0, 1);
            var dot = Vector3.Dot(v, w);
            var cross = Vector3.Cross(v, w).y;
            //Debug.Log("d = " + d);
            if(d.magnitude < 6f && cross < 0 && dot > 0.4f) {
                HSwing();
                PauseFor(1f);
            } else if(human.rigid.velocity.magnitude < 3f) {
                RowFront();
                PauseFor(1f);
            } else {
                if(dot >= 0.95f) {
                    RowFront();
                    PauseFor(0.8f);
                } else if(cross > 0.1) {
                    RowLeft();
                    PauseFor(0.4f);
                } else {
                    RowRight();
                    PauseFor(0.4f);
                }
            }
        }
    }

    public class SeekingAI1 : AI {
        float targetVelo = 2f;
        int targetDir = 0;
        public override void Step(OzAI ai, OzHuman human) {
            Debug.Log("Seeking");
            var sight = human.transform.rotation * new Vector3(0f, 0f, 1f);
            var objects = GameObject.FindGameObjectsWithTag("Player");
            foreach(var obj in objects) {
                var h = obj.transform.Find("human2a");
                var d = h.position - human.transform.position;
                if(d.magnitude < 15f && Vector3.Dot(sight, d) > 0f) {
                    ChangeAI(new ChasingAI1(h.GetComponent<OzHuman>()));
                    return;
                }
            }
            if(human.rigid.velocity.magnitude < targetVelo) {
                RowFront();
                PauseFor(1f);
                targetDir = Random.Range(0, 2);
            } else {
                if(targetDir == 0) {
                    RowLeft();
                } else {
                    RowRight();
                }
                PauseFor(0.4f);
            }
        }
    }
}
