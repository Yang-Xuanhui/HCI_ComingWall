using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 25个关键点索引对应的身体部位
public enum KeypointIdx_25
{
    Nose,      // 0
    Neck,      // 1
    RShoulder, // 2
    RElbow,    // 3
    RWrist,    // 4
    LShoulder, // 5
    LElbow,    // 6
    LWrist,    // 7
    MidHip,    // 8
    RHip,      // 9
    RKnee,     // 10
    RAnkle,    // 11
    LHip,      // 12
    LKnee,     // 13
    LAnkle,    // 14
    REye,      // 15
    LEye,      // 16
    REar,      // 17
    LEar,      // 18
    LBigToe,   // 19
    LSmallToe, // 20
    LHeel,     // 21
    RBigToe,   // 22
    RSmallToe, // 23
    RHeel      // 24
}

// 目前只考虑了单人的情况
public class PoseManager : MonoBehaviour
{
    List<Vector3> keyPoints = new List<Vector3>();

    public GameObject part; // 用来画线
    public float error = 0.05f; // 姿势误差，设成多少合适呢？
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // 测试用的动作关键点
        float z = transform.position.z;
        List<Vector3> testPost = new List<Vector3>
        {
            //new Vector3(570.582f,229.784f,z), // nose
            //new Vector3(590.253f,276.731f,z), // neck
            ////new Vector3(552.957f,274.801f,z), // right shoulder
            ////new Vector3(551.043f,290.421f,z), // right elbow
            ////new Vector3(558.874f,268.9f,z),   // right wrist
            ////new Vector3(633.362f,276.784f,z), // left shoulder
            ////new Vector3(637.201f,339.5f,z),   // left elbow
            ////new Vector3(613.678f,294.411f,z), // left wrist
            //new Vector3(552.957f,276.731f,z), // right shoulder
            //new Vector3(551.043f,276.731f,z), // right elbow
            //new Vector3(400.874f,276.731f,z),   // right wrist
            //new Vector3(633.362f,276.731f,z), // left shoulder
            //new Vector3(637.201f,276.731f,z),   // left elbow
            //new Vector3(613.678f,276.731f,z), // left wrist
            //new Vector3(554.935f,409.95f,z),  // mid hip
            //new Vector3(554.935f,409.95f,z),  // right hip
            //new Vector3(568.669f,523.622f,z), // right knee
            //new Vector3(570.542f,648.986f,z), // right ankle
            //new Vector3(605.93f,411.927f,z),  // left hip
            //new Vector3(592.158f,525.637f,z), // left knee
            //new Vector3(584.266f,646.987f,z), // left ankle
            //new Vector3(558.858f,225.793f,z), // right eye
            //new Vector3(578.473f,223.79f,z),  // left eye
            //new Vector3(554.941f,231.686f,z), // right ear
            //new Vector3(601.915f,225.827f,z)  // left ear
            new Vector3(0.3f,5.8f,z),
            new Vector3(0f,5.1f,z),
            new Vector3(-0.7f,5.1f,z),
            new Vector3(-1.7f,5.0f,z),
            new Vector3(-2.6f,5.1f,z),
            new Vector3(0.7f,5.1f,z),
            new Vector3(1.7f,5.1f,z),
            new Vector3(2.8f,5.1f,z),

        };
        //Vector3 reverse = new Vector3(0, -1, 0);
        Vector3 move = new Vector3(5f, -0.5f, 0);
        //move = new Vector3(0f, -0.5f, 0);
        for (int i = 0; i < testPost.Count; i++)
        {
            Vector3 tmp = testPost[i];
            tmp += move;
            testPost[i] = tmp;
        }
        SetPost(testPost);
        DrawHuman();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 对外的function，设置动作
    public void SetPost(List<Vector3> keyPoints_3d)
    {
        this.keyPoints = keyPoints_3d;
    }

    // 测试用
    public void DrawHuman()
    {
        
        GameObject rArm_obj = Instantiate(part);
        LineRenderer rArm = rArm_obj.GetComponent<LineRenderer>();
        GameObject lArm_obj = Instantiate(part);
        LineRenderer lArm = lArm_obj.GetComponent<LineRenderer>();
        GameObject rLeg_obj = Instantiate(part);
        LineRenderer rLeg = rLeg_obj.GetComponent<LineRenderer>();
        GameObject lLeg_obj = Instantiate(part);
        LineRenderer lLeg = lLeg_obj.GetComponent<LineRenderer>();
        GameObject body_obj = Instantiate(part);
        LineRenderer body = body_obj.GetComponent<LineRenderer>();

        rArm.positionCount = 4;
        rArm.SetPositions(keyPoints.GetRange(1, 4).ToArray());

        lArm.positionCount = 4;
        lArm.SetPosition(0, keyPoints[1]);
        lArm.SetPosition(1, keyPoints[5]);
        lArm.SetPosition(2, keyPoints[6]);
        lArm.SetPosition(3, keyPoints[7]);

        //rLeg.positionCount = 3;
        //rLeg.SetPositions(keyPoints.GetRange(8, 3).ToArray());

        //lLeg.positionCount = 3;
        //lLeg.SetPosition(0, keyPoints[11]);
        //lLeg.SetPosition(1, keyPoints[12]);
        //lLeg.SetPosition(2, keyPoints[13]);
        //lLeg.SetPosition(3, keyPoints[14]);

        body.positionCount = 2;
        body.SetPosition(0, keyPoints[0]);
        body.SetPosition(1, keyPoints[1]);
        //body.SetPosition(2, (keyPoints[8]+keyPoints[11])/2);
        //body.SetPosition(3, keyPoints[8]);
        //body.SetPosition(4, (keyPoints[8] + keyPoints[11]) / 2);
        //body.SetPosition(5, keyPoints[11]);

    }


    /* 
     * 检查是否做了准备动作（T Pose)并保持一段时间
     * 如果做了符合的动作，timer++
     * 否则timer清零
     * 返回当前计时或错误提示
     * -1: 手臂伸平
     * -2: 身体对准墙
     */
    public float Prepare(List<Collider> colliders)
    {
        // 首先直接检测手臂是否伸平
        if(keyPoints[2].y - keyPoints[3].y > error
            || keyPoints[3].y - keyPoints[4].y > error
            || keyPoints[5].y - keyPoints[6].y > error
            || keyPoints[6].y - keyPoints[7].y > error)
        {
            timer = 0;
            return -1;
        }

        // 手臂伸平后判断整体位置
        for (int i = 0; i < keyPoints.Count; i++)
        {
            Vector3 point = keyPoints[i];
            bool wall = false;
            bool hole = false;
            for (int j = 0; j < colliders.Count; j++)
            {
                Collider collider = colliders[j];
                // 在墙范围内
                if (collider.tag == "Wall" && collider.bounds.Contains(point))
                {
                    wall = true;
                }
                // 在洞的范围内
                if (collider.tag == "Hole" && collider.bounds.Contains(point))
                {
                    hole = true;
                    break;
                }
                // 位置在洞的范围内,提前结束这一层循环
                if (wall && hole)
                {
                    break;
                }
            }
            // 有一个点在外面就结束判断
            if (!hole)
            {
                timer = 0;
                return -2;              
            }
        }
        timer += Time.deltaTime;
        return timer;
    }

    // 清空准备时的计时
    public float ClearTimer()
    {
        timer = 0;
        return timer;
    }

    /* 
     * 判断关键点与墙的位置
     * 关键点同时与“wall”和“hole”相交说明通过
     * 只和“wall”相交说明撞墙
     * 都不相交可能是位置太偏或者判断太早（需要其他函数来判断）
     * 
     * 返回一个分数
     */
    public float Play(List<Collider> colliders)
    {
        float score = 0;
        for(int i = 0; i < keyPoints.Count; i++)
        {
            Vector3 point = keyPoints[i];
            bool wall = false;
            bool hole = false;
            for(int j = 0; j < colliders.Count; j++)
            {
                Collider collider = colliders[j];
                if (collider.tag == "Wall" && collider.bounds.Contains(point))
                {
                    wall = true;
                }
                if (collider.tag == "Hole" && collider.bounds.Contains(point))
                {
                    hole = true;
                }
            }
            if(wall && hole)
            {
                score += 1;
            }
            //else if(wall && !hole)
            //{
            //    Debug.Log("hit the wall at " + point);
            //}
            //else
            //{
            //    Debug.Log("error "+i+" "+point);
            //}
        }
        return score;
    }
}
