﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

/*
- 주변환경을 관측 (Observation)
- 정책에 따라서 행동 (Action)
- 보상 (+/- Reward)
*/
public class MummyAgent : Agent
{
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    
    public Renderer floorRd;
    public Material goodMt;
    public Material badMt;
    private Material originMt;

    //에이젼트 초기화 함수
    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        originMt = floorRd.material;
    }

    //학습(Episode)이 시작될 때 마다
    public override void OnEpisodeBegin()
    {
        //Rigidbody의 속도와 회전을 초기화
        rb.velocity = rb.angularVelocity = Vector3.zero;
        //Target, Agent의 위치를 불규칙하게 변경
        tr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f)
                                      ,0.05f
                                      ,Random.Range(-4.0f, 4.0f));

        targetTr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f)
                                            ,0.55f
                                            ,Random.Range(-4.0f, 4.0f));
    }

    //주변 환경을 관측해서 머신러닝 알고리즘(TF)로 전달하는 역학
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition);  //3
        sensor.AddObservation(tr.localPosition);        //3
        sensor.AddObservation(rb.velocity.x);           //1
        sensor.AddObservation(rb.velocity.z);           //1
    }

    //브레인으로 부터 전달 받은 데이터 대로 행동을 취하는 함수
    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log($"[0]={vectorAction[0]} / [1]={vectorAction[1]}");
        float v = Mathf.Clamp(vectorAction[0], -1.0f, 1.0f);
        float h = Mathf.Clamp(vectorAction[1], -1.0f, 1.0f);

        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        rb.AddForce(dir.normalized * 30.0f);

        //지속적인 이동을 위해 마이너스 페널티를 줌(빨리 목적지에 도달할수록 좋다.)
        SetReward(-0.001f);
    }


    //테스트
    public override void Heuristic(float[] actionsOut)
    {
        //전/후진
        actionsOut[0] = Input.GetAxis("Vertical"); // -1.0f ~ 0.0f ~ +1.0f
        //좌/우
        actionsOut[1] = Input.GetAxis("Horizontal");
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("DEAD_ZONE"))
        {
            StartCoroutine(this.ChangeMaterial(badMt));
            SetReward(-1.0f);
            EndEpisode();
        }

        if (coll.collider.CompareTag("TARGET"))
        {
            StartCoroutine(this.ChangeMaterial(goodMt));
            SetReward(+1.0f);
            EndEpisode();
        }
    }

    IEnumerator ChangeMaterial(Material changeMt)
    {
        floorRd.material = changeMt;
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }

}
