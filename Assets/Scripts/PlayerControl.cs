using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("基础设置")]
    public float MaxMoveSpeed;
    private float MoveSpeed;
    public Vector2 PlayerMoveRegion;
    public float[] PlayerExtremumZ = new float[2];
    private Vector3 BaseImgV;

    //public float[] ExtremunZ=new float[2];
    // Start is called before the first frame update
    void Start()
    {
        MoveSpeed = MaxMoveSpeed;
        BaseImgV = transform.GetChild(0).localPosition;
    }

#region 玩家移动数据设置
    public Vector2 PlayerMove()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        return new Vector2(inputX, inputY);
    }
    public float PlayerZ(Transform aim)
    {
        return PlayerExtremumZ[0] + (PlayerExtremumZ[1] - PlayerExtremumZ[0]) * (PlayerMoveRegion.y - aim.position.y) / (2 * PlayerMoveRegion.y);
    }
#endregion

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = PlayerMove() * MoveSpeed;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -PlayerMoveRegion.x, PlayerMoveRegion.x), 
            Mathf.Clamp(transform.position.y, -PlayerMoveRegion.y, PlayerMoveRegion.y), 0);
        transform.GetChild(0).localPosition = Vector3.forward * PlayerZ(transform) + BaseImgV;
    }
}
