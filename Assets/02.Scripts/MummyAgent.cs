using System.Collections;
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

    //에이젼트 초기화 함수
    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
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
    }

    //주변 환경을 관측해서 머신러닝 알고리즘(TF)로 전달하는 역학
    public override void CollectObservations(VectorSensor sensor)
    {

    }

    //브레인으로 부터 전달 받은 데이터 대로 행동을 취하는 함수
    public override void OnActionReceived(float[] vectorAction)
    {

    }


    public override void Heuristic(float[] actionsOut)
    {

    }


}
