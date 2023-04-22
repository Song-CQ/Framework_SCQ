using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
//����ű��Ž��˵����е�Camera-Control/Mouse Look
public class MouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    //����һ��ö������{����˫��ת����������X��ת����������Y��ת��}
    public RotationAxes axes = RotationAxes.MouseXAndY;
    //����һ��ö�٣�Ĭ��Ϊ RotationAxes.MouseXAndY(����˫��ת��)
    public float sensitivityX = 15F;
    //x��ת��������
    public float sensitivityY = 15F;
    //Y��ת��������
    public float minimumX = -360F;
    public float maximumX = 360F;
    //����X���ֵ
    public float minimumY = -60F;
    public float maximumY = 60F;
    //����y���ֵ
    float rotationY = 0F;
    //���õ�ǰ�����X����ת��ʼΪ0
    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        //������û�и���,����ifΪ�գ�
        //���裬�ں��������ж�̬���Ӹ��壬��Ѹ����������ת������
    }
    void Update()
    {
        if (axes == RotationAxes.MouseXAndY)//�������Ϊ����˫��ת��
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            //��ǰ����Y��Ҫ��ת��ֵ = ��ǰ��ǰ�����ŷ���ǵ�Yֵ+��ǰ����X���ֵ*X��ת��������
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            //��ǰ����X��Ҫ��ת��ֵ = ��ǰ�����ŷ���ǵ�xֵ+��ǰ����Y���ֵ*Y��ת��������
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            //���Ƶ�ǰ����X��Ҫ��ת��ֵ( Ҫ���Ƶ�ֵ,��СΪminimum�����ΪmaximumY)
            rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
            //���Ƶ�ǰ����X��Ҫ��ת��ֵ( Ҫ���Ƶ�ֵ,��СΪminimum�����ΪmaximumY)
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            //��ǰ�����ŷ����= new Vector��-��ǰ����X��Ҫ��ת��ֵ����ǰ����Y��Ҫ��ת��ֵ��0 )
        }
        else if (axes == RotationAxes.MouseX)//�������Ϊ������X��ת��
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            //��ǰ������תһ��ŷ����(0����ǰ����X���ֵ*X��ת����������0)
        }
        else//�������Ϊ������y��ת��
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            //��ǰ����X��Ҫ��ת��ֵ = ��ǰ�����ŷ���ǵ�xֵ+��ǰ����Y���ֵ*Y��ת��������
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            //���Ƶ�ǰ����X��Ҫ��ת��ֵ( Ҫ���Ƶ�ֵ,��СΪminimum�����ΪmaximumY)
            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            //��ǰ�����ŷ����= new Vector��-��ǰ����X��Ҫ��ת��ֵ����ǰ����ŷ����Y���ֵ��0 )
        }

    }


}