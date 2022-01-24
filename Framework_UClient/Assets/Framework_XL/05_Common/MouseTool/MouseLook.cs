using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
//这个脚本放进菜单键中的Camera-Control/Mouse Look
public class MouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    //创建一个枚举类型{允许双轴转动，仅允许X轴转动，仅允许Y轴转动}
    public RotationAxes axes = RotationAxes.MouseXAndY;
    //创建一个枚举，默认为 RotationAxes.MouseXAndY(允许双轴转动)
    public float sensitivityX = 15F;
    //x轴转动的增量
    public float sensitivityY = 15F;
    //Y轴转动的增量
    public float minimumX = -360F;
    public float maximumX = 360F;
    //限制X轴的值
    public float minimumY = -60F;
    public float maximumY = 60F;
    //限制y轴的值
    float rotationY = 0F;
    //设置当前物体的X的旋转初始为0
    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        //？？？没有刚体,所以if为空，
        //假设，在后续代码有动态添加刚体，则把刚体的物理旋转给冻结
    }
    void Update()
    {
        if (axes == RotationAxes.MouseXAndY)//如果类型为允许双轴转动
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            //当前物体Y轴要旋转的值 = 当前当前物体的欧拉角的Y值+当前鼠标的X轴的值*X轴转动的增量
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            //当前物体X轴要旋转的值 = 当前物体的欧拉角的x值+当前鼠标的Y轴的值*Y轴转动的增量
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            //限制当前物体X轴要旋转的值( 要限制的值,最小为minimum，最大为maximumY)
            rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
            //限制当前物体X轴要旋转的值( 要限制的值,最小为minimum，最大为maximumY)
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            //当前物体的欧拉角= new Vector（-当前物体X轴要旋转的值，当前物体Y轴要旋转的值，0 )
        }
        else if (axes == RotationAxes.MouseX)//如果类型为仅允许X轴转动
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            //当前物体旋转一个欧拉角(0，当前鼠标的X轴的值*X轴转动的增量，0)
        }
        else//如果类型为仅允许y轴转动
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            //当前物体X轴要旋转的值 = 当前物体的欧拉角的x值+当前鼠标的Y轴的值*Y轴转动的增量
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            //限制当前物体X轴要旋转的值( 要限制的值,最小为minimum，最大为maximumY)
            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            //当前物体的欧拉角= new Vector（-当前物体X轴要旋转的值，当前物体欧拉角Y轴的值，0 )
        }

    }


}