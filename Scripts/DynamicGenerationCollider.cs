using UnityEngine;
using System.Collections;

public class DynamicGenerationCollider : UVStretch
{

    Mesh m_dGCollider;

    GameObject m_raycastCube1;          //获得发射射线的Cube
    GameObject m_raycastCube2;
    GameObject m_raycastCube3;
    GameObject m_raycastCube4;

    ChangeHeadModel m_changeHeadModel;

    Vector2 uniCubeHit1;                //获得碰撞点单位坐标
    Vector2 uniCubeHit2;
    Vector2 uniCubeHit3;
    Vector2 uniCubeHit4;

    void Update()
    {
        /*if(m_uVObj != null)
        {
            Vector3 fwd3 = m_raycastCube3.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(m_raycastCube3.transform.position, fwd3, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube3.transform.position, hit.point, Color.red);
                //Debug.Log("hit_3 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);
            }
        }*/


        if (Input.GetKeyDown(KeyCode.B))
        {
            print("U have Key Down B");
            SetDGCollider();
            SetRaycast();
            //ChangeModelStep2();
        }

    }

    //动态生成碰撞体
    void SetDGCollider()
    {
        m_uVObj.GetComponent<SkinnedMeshRenderer>().BakeMesh(m_mesh);
        for (int i = 0; i < m_allChangedVector3.Length; i++)
        {
            m_allChangedVector3[i] = m_uVObj.transform.localToWorldMatrix.MultiplyPoint3x4(m_mesh.vertices[i]);
            m_allChangedVector3[i] = new Vector3(-m_allChangedVector3[i].x, m_allChangedVector3[i].y, -m_allChangedVector3[i].z);
        }
        m_mesh.vertices = m_originMeshVertex;

        if (m_dGCollider != null)
        {
            m_dGCollider = null;
            m_dGCollider = new Mesh();
            m_dGCollider.vertices = new Vector3[m_allChangedVector3.Length];
            m_dGCollider.vertices = m_allChangedVector3;
            m_dGCollider.triangles = m_mesh.triangles;
            m_uVObj.GetComponent<MeshCollider>().sharedMesh = m_dGCollider;
        }
        else
        {
            m_dGCollider = new Mesh();
            m_dGCollider.vertices = new Vector3[m_allChangedVector3.Length];
            m_dGCollider.vertices = m_allChangedVector3;
            m_dGCollider.triangles = m_mesh.triangles;
            m_uVObj.GetComponent<MeshCollider>().sharedMesh = m_dGCollider;
        }
    }

    //清除碰撞体
    void clearDGCollider()
    {
        m_uVObj.GetComponent<MeshCollider>().sharedMesh = null;
    }

    //产生射线
    void SetRaycast()
    {
        SetDGCollider();

        RaycastHit hit;
        if (m_raycastCube4 != null)
        {
            //获取射线方向
            Vector3 fwd1 = m_raycastCube1.transform.TransformDirection(Vector3.forward);
            Vector3 fwd2 = m_raycastCube2.transform.TransformDirection(Vector3.forward);
            Vector3 fwd3 = m_raycastCube3.transform.TransformDirection(Vector3.forward);
            Vector3 fwd4 = m_raycastCube4.transform.TransformDirection(Vector3.forward);

            //获取底层坐标数据并转化为单位坐标
            m_changeHeadModel = this.GetComponent<ChangeHeadModel>();
            Vector2 bottomData_noseDown_L = m_changeHeadModel.m_bottomData_noseDown_L;
            Vector2 bottomData_noseDown_R = m_changeHeadModel.m_bottomData_noseDown_R;
            Vector2 bottomData_mailla_L = m_changeHeadModel.m_bottomData_Mailla_L;
            Vector2 bottomData_mailla_R = m_changeHeadModel.m_bottomData_Mailla_R;
            Debug.Log("Get Key B Down: BottomData is ============>>>" + bottomData_noseDown_L + "   " + bottomData_noseDown_R + "   " + bottomData_mailla_L + "   " + bottomData_mailla_R);

            //获得形变后单位距离
            Vector2 tearL = UVStretch.m_allChangedVector3[UVPointsID.ID_tearL];
            Vector2 lipMid = UVStretch.m_allChangedVector3[UVPointsID.ID_lipMid];
            float uniValChanged = Mathf.Abs(tearL.y - lipMid.y) / 100f;

            //将底层坐标转化为世界坐标
            Vector2 centerPointChanged = new Vector2(0f, tearL.y);
            Vector2 worldData_noseDown_L = bottomData_noseDown_L * uniValChanged + centerPointChanged;
            Vector2 worldData_noseDown_R = bottomData_noseDown_R * uniValChanged + centerPointChanged;
            Vector2 worldData_mailla_L = bottomData_mailla_L * uniValChanged + centerPointChanged;
            Vector2 worldData_mailla_R = bottomData_mailla_R * uniValChanged + centerPointChanged;
            //Debug.Log("Get Key B Down: worldData_noseDown_L is ====>>>" + worldData_noseDown_L.x + "   " + worldData_noseDown_L.y);

            //将射线移动至相应的Y轴高度
            m_raycastCube1.transform.position = new Vector3(m_raycastCube1.transform.position.x, worldData_noseDown_L.y, m_raycastCube1.transform.position.z);
            m_raycastCube2.transform.position = new Vector3(m_raycastCube2.transform.position.x, worldData_noseDown_R.y, m_raycastCube2.transform.position.z);
            m_raycastCube3.transform.position = new Vector3(m_raycastCube3.transform.position.x, worldData_mailla_L.y, m_raycastCube3.transform.position.z);
            m_raycastCube4.transform.position = new Vector3(m_raycastCube4.transform.position.x, worldData_mailla_R.y, m_raycastCube4.transform.position.z);

            if (Physics.Raycast(m_raycastCube1.transform.position, fwd1, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube1.transform.position, hit.point, Color.red);
                Debug.Log("hit_1 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit1 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_noseDown_L.x + "  " + bottomData_noseDown_L.y);
                Debug.Log("uniCube1Hit1 is " + uniCubeHit1.x + "   " + uniCubeHit1.y);
            }

            if (Physics.Raycast(m_raycastCube2.transform.position, fwd2, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube2.transform.position, hit.point, Color.red);
                Debug.Log("hit_2 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit2 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_noseDown_R.x + "  " + bottomData_noseDown_R.y);
                Debug.Log("uniCube1Hit2 is " + uniCubeHit2.x + "   " + uniCubeHit2.y);
            }

            if (Physics.Raycast(m_raycastCube3.transform.position, fwd3, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube3.transform.position, hit.point, Color.red);
                Debug.Log("hit_3 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit3 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_mailla_L.x + "  " + bottomData_mailla_L.y);
                Debug.Log("uniCube1Hit3 is " + uniCubeHit3.x + "   " + uniCubeHit3.y);
            }

            if (Physics.Raycast(m_raycastCube4.transform.position, fwd4, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube4.transform.position, hit.point, Color.red);
                Debug.Log("hit_4 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit4 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_mailla_R.x + "  " + bottomData_mailla_R.y);
                Debug.Log("uniCube1Hit4 is " + uniCubeHit4.x + "   " + uniCubeHit4.y);
            }

            clearDGCollider();
        }
        else
        {
            m_raycastCube1 = GameObject.Find("RaycastCube1");
            m_raycastCube2 = GameObject.Find("RaycastCube2");
            m_raycastCube3 = GameObject.Find("RaycastCube3");
            m_raycastCube4 = GameObject.Find("RaycastCube4");

            //获取相应射线碰撞到的模型点坐标
            Vector3 fwd1 = m_raycastCube1.transform.TransformDirection(Vector3.forward);
            Vector3 fwd2 = m_raycastCube2.transform.TransformDirection(Vector3.forward);
            Vector3 fwd3 = m_raycastCube3.transform.TransformDirection(Vector3.forward);
            Vector3 fwd4 = m_raycastCube4.transform.TransformDirection(Vector3.forward);

            //获取底层坐标数据并转化为单位坐标
            m_changeHeadModel = this.GetComponent<ChangeHeadModel>();
            Vector2 bottomData_noseDown_L = m_changeHeadModel.m_bottomData_noseDown_L;
            Vector2 bottomData_noseDown_R = m_changeHeadModel.m_bottomData_noseDown_R;
            Vector2 bottomData_mailla_L = m_changeHeadModel.m_bottomData_Mailla_L;
            Vector2 bottomData_mailla_R = m_changeHeadModel.m_bottomData_Mailla_R;
            Debug.Log("Get Key B Down: BottomData is ============>>>" + bottomData_noseDown_L + "   " + bottomData_noseDown_R + "   " + bottomData_mailla_L + "   " + bottomData_mailla_R);

            //获得形变后单位距离
            Vector2 tearL = UVStretch.m_allChangedVector3[UVPointsID.ID_tearL];
            Vector2 lipMid = UVStretch.m_allChangedVector3[UVPointsID.ID_lipMid];
            float uniValChanged = Mathf.Abs(tearL.y - lipMid.y) / 100f;

            //将底层坐标转化为世界坐标
            Vector2 centerPointChanged = new Vector2(0f, tearL.y);
            Vector2 worldData_noseDown_L = bottomData_noseDown_L * uniValChanged + centerPointChanged;
            Vector2 worldData_noseDown_R = bottomData_noseDown_R * uniValChanged + centerPointChanged;
            Vector2 worldData_mailla_L = bottomData_mailla_L * uniValChanged + centerPointChanged;
            Vector2 worldData_mailla_R = bottomData_mailla_R * uniValChanged + centerPointChanged;
            Debug.Log("Get Key B Down: worldData_noseDown_L is ====>>>" + worldData_noseDown_L.x + "   " + worldData_noseDown_L.y);

            //将射线移动至相应的Y轴高度
            m_raycastCube1.transform.position = new Vector3(m_raycastCube1.transform.position.x, worldData_noseDown_L.y, m_raycastCube1.transform.position.z);
            m_raycastCube2.transform.position = new Vector3(m_raycastCube2.transform.position.x, worldData_noseDown_R.y, m_raycastCube2.transform.position.z);
            m_raycastCube3.transform.position = new Vector3(m_raycastCube3.transform.position.x, worldData_mailla_L.y, m_raycastCube3.transform.position.z);
            m_raycastCube4.transform.position = new Vector3(m_raycastCube4.transform.position.x, worldData_mailla_R.y, m_raycastCube4.transform.position.z);

            if (Physics.Raycast(m_raycastCube1.transform.position, fwd1, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube1.transform.position, hit.point, Color.red);
                Debug.Log("hit_1 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit1 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_noseDown_L.x + "  " + bottomData_noseDown_L.y);
                Debug.Log("uniCube1Hit1 is " + uniCubeHit1.x + "   " + uniCubeHit1.y);
            }

            if (Physics.Raycast(m_raycastCube2.transform.position, fwd2, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube2.transform.position, hit.point, Color.red);
                Debug.Log("hit_2 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit2 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_noseDown_R.x + "  " + bottomData_noseDown_R.y);
                Debug.Log("uniCube1Hit2 is " + uniCubeHit2.x + "   " + uniCubeHit2.y);
            }

            if (Physics.Raycast(m_raycastCube3.transform.position, fwd3, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube3.transform.position, hit.point, Color.red);
                Debug.Log("hit_3 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit3 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_mailla_L.x + "  " + bottomData_mailla_L.y);
                Debug.Log("uniCube1Hit3 is " + uniCubeHit3.x + "   " + uniCubeHit3.y);
            }

            if (Physics.Raycast(m_raycastCube4.transform.position, fwd4, out hit, 1))
            {
                Debug.DrawLine(m_raycastCube4.transform.position, hit.point, Color.red);
                Debug.Log("hit_4 potint is " + hit.point.x + "   " + hit.point.y + "   " + hit.point.z);

                //将碰撞得到的点坐标转换为单位坐标
                uniCubeHit4 = ((Vector2)hit.point - centerPointChanged) / uniValChanged;
                Debug.Log("bottomData is " + bottomData_mailla_R.x + "  " + bottomData_mailla_R.y);
                Debug.Log("uniCube1Hit4 is " + uniCubeHit4.x + "   " + uniCubeHit4.y);
            }
            clearDGCollider();
        }
    }

    public void ChangeModelStep2()
    {
        float val = 100f;

        //寻找物体发射射线用
        m_raycastCube1 = GameObject.Find("RaycastCube1");
        m_raycastCube2 = GameObject.Find("RaycastCube2");
        m_raycastCube3 = GameObject.Find("RaycastCube3");
        m_raycastCube4 = GameObject.Find("RaycastCube4");

        SetRaycast();

        //获取碰撞点与底层数据的差值
        //float D_Value = uniCubeHit1.x - m_changeHeadModel.m_bottomData_noseDown_L.x;
        /*float D_Value_1 = uniCubeHit1.x - m_changeHeadModel.m_bottomData_noseDown_L.x;
        float D_Value_2 = uniCubeHit2.x - m_changeHeadModel.m_bottomData_noseDown_R.x;
        float D_Value_3 = uniCubeHit3.x - m_changeHeadModel.m_bottomData_Mailla_L.x;
        float D_Value_4 = uniCubeHit4.x - m_changeHeadModel.m_bottomData_Mailla_R.x;*/
        float D_Value_1, D_Value_2, D_Value_3, D_Value_4;
        if (m_changeHeadModel.m_bottomData_noseDown_R.y > 0.15f)
        {
            D_Value_1 = 0f;
            D_Value_2 = 0f;
        }
        else
        {
            D_Value_1 = uniCubeHit1.x - m_changeHeadModel.m_bottomData_noseDown_L.x;
            D_Value_2 = uniCubeHit2.x - m_changeHeadModel.m_bottomData_noseDown_R.x;
        }
        if (m_changeHeadModel.m_bottomData_Mailla_R.y > 0.15f)
        {
            D_Value_3 = 0f;
            D_Value_4 = 0f;
        }
        else
        {
            D_Value_3 = uniCubeHit3.x - m_changeHeadModel.m_bottomData_Mailla_L.x;
            D_Value_4 = uniCubeHit4.x - m_changeHeadModel.m_bottomData_Mailla_R.x;
        }
        Debug.Log("The D_Value is " + D_Value_1 + "  " + D_Value_2 + "   " + D_Value_3 + "   " + D_Value_4);

        //根据差值求形变
        if (D_Value_1 > 0f)            //m_bottomData_noseDown_L
        {
            setBlendShape(m_uVObj, "Origin_NoseDown_L" + "_L", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_1 = uniCubeHit1.x - m_changeHeadModel.m_bottomData_noseDown_L.x;
                Debug.Log("For turn is : " + i.ToString() +"  &&  The D_Value_1_1 is " + D_Value_1);
                if (D_Value_1 >= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_L" + "_L", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 2f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_L" + "_L", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 2f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else if (D_Value_1 < 0f)
        {
            setBlendShape(m_uVObj, "Origin_NoseDown_L" + "_R", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_1 = uniCubeHit1.x - m_changeHeadModel.m_bottomData_noseDown_L.x;
                Debug.Log("The D_Value_1_2 is " + D_Value_1);
                if (D_Value_1 <= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_L" + "_R", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_L" + "_R", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else
        {
            return;
        }

        if (D_Value_2 > 0f)            //m_bottomData_noseDown_R
        {
            setBlendShape(m_uVObj, "Origin_NoseDown_R" + "_L", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_2 = uniCubeHit2.x - m_changeHeadModel.m_bottomData_noseDown_R.x;
                Debug.Log("The D_Value_2_1 is " + D_Value_2);
                if (D_Value_2 >= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_R" + "_L", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_R" + "_L", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else if (D_Value_2 < 0f)
        {
            setBlendShape(m_uVObj, "Origin_NoseDown_R" + "_R", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_2 = uniCubeHit2.x - m_changeHeadModel.m_bottomData_noseDown_R.x;
                Debug.Log("The D_Value_2_2 is " + D_Value_2);
                if (D_Value_2 <= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_R" + "_R", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_NoseDown_R" + "_R", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else
        {
            return;
        }

        if (D_Value_3 > 0f)            //m_bottomData_mailla_L
        {
            setBlendShape(m_uVObj, "Origin_Mailla_L" + "_L", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_3 = uniCubeHit3.x - m_changeHeadModel.m_bottomData_Mailla_L.x;
                Debug.Log("=============== For Turn is : " + i.ToString() + "   The D_Value_3_1 is " + D_Value_3);
                if (D_Value_3 >= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_L" + "_L", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 2f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_L" + "_L", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 2f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else if (D_Value_3 < 0f)
        {
            setBlendShape(m_uVObj, "Origin_Mailla_L" + "_R", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_3 = uniCubeHit3.x - m_changeHeadModel.m_bottomData_Mailla_L.x;
                Debug.Log("=============== For Turn is : " + i.ToString() + "   The D_Value_3_2 is " + D_Value_3);
                if (D_Value_3 <= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_L" + "_R", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_L" + "_R", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else
        {
            return;
        }

        if (D_Value_4 > 0f)            //m_bottomData_noseDown_R
        {
            setBlendShape(m_uVObj, "Origin_Mailla_R" + "_L", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_4 = uniCubeHit4.x - m_changeHeadModel.m_bottomData_Mailla_R.x;
                Debug.Log("The D_Value_4_1 is " + D_Value_4 + "uniCubeHit4.x is " + uniCubeHit4.x + "m_changeHeadModel.m_bottomData_Mailla_R.x" + m_changeHeadModel.m_bottomData_Mailla_R.x);
                if (D_Value_4 >= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_R" + "_L", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_R" + "_L", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else if(D_Value_4 < 0f)
        {
            setBlendShape(m_uVObj, "Origin_Mailla_R" + "_R", val / 2f);
            for (int i = 0; i < 8; i++)
            {
                SetRaycast();
                D_Value_4 = uniCubeHit4.x - m_changeHeadModel.m_bottomData_Mailla_R.x;
                Debug.Log("The D_Value_4_2 is " + D_Value_4);
                if (D_Value_4 <= 0f)
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_R" + "_R", val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
                else
                {
                    setBlendShape(m_uVObj, "Origin_Mailla_R" + "_R", -val / Mathf.Pow(2f, (float)i + 2f));
                    Debug.Log("val / Mathf.Pow(2f, (float)i + 1f) is ===>>" + val / Mathf.Pow(2f, (float)i + 2f));
                }
            }
        }
        else
        {
            return;
        }

    }

    void setBlendShape(GameObject obj, string BlendShapeName1, float val1, string BlendShapeName2, float val2, string BlendShapeName3, float val3, string BlendShapeName4, float val4)
    {
        string[] blendshapes = m_changeHeadModel.getBlendShapeNames(obj);
        for (int i = 0; i < blendshapes.Length; i++)
        {
            if (BlendShapeName1 != null && blendshapes[i].IndexOf(BlendShapeName1) != -1)
            {
                float oriVal = obj.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                obj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, oriVal + val1);
            }
            if (BlendShapeName2 != null && blendshapes[i].IndexOf(BlendShapeName2) != -1)
            {
                float oriVal = obj.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                obj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, oriVal + val2);
            }
            if (BlendShapeName3 != null && blendshapes[i].IndexOf(BlendShapeName3) != -1)
            {
                float oriVal = obj.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                obj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, oriVal + val3);
            }
            if (BlendShapeName4 != null && blendshapes[i].IndexOf(BlendShapeName4) != -1)
            {
                float oriVal = obj.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                obj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, oriVal + val4);
            }
        }
    }
    void setBlendShape(GameObject obj, string BlendShapeName1, float val1)
    {
        string[] blendshapes = m_changeHeadModel.getBlendShapeNames(obj);
        for (int i = 0; i < blendshapes.Length; i++)
        {
            if (BlendShapeName1 != null && blendshapes[i].IndexOf(BlendShapeName1) != -1)
            {
                float oriVal = obj.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(i);
                obj.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i, oriVal + val1);
            }
        }
    }


}
