using UnityEngine;
using System.Collections;
using ActionGameLibrary;

public class KeyStateLinker : MonoBehaviour
{
    public void setKey_A_Down()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.A, true);
    }
    public void setKey_A_Up()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.A, false);
    }

    public void setKey_B_Down()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.B, true);
    }
    public void setKey_B_Up()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.B, false);
    }

    public void set_Attack_Down()
    {

        VirtualKeyState.SetKeyDown(VirtualKeyState.A, false);

    }
    public void set_Attack_Up()
    {

        VirtualKeyState.SetKeyDown(VirtualKeyState.A, true);
    }
    public void setKey_L_Down()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.LEFT, true);
    }
    public void setKey_L_Up()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.LEFT, false);
    }

    public void setKey_L_A_Down()
    {
        VirtualKeyState.lockDirection = -1;
        VirtualKeyState.SetKeyDown(VirtualKeyState.A, true);
    }
    public void setKey_L_A_Up()
    {
        VirtualKeyState.lockDirection = 0;
        VirtualKeyState.SetKeyDown(VirtualKeyState.A, false);
    }

    public void setKey_L_B_Down()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.B, true);
    }
    public void setKey_L_B_Up()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.B, false);
    }
    //===========
    public void setKey_R_Down()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.RIGHT, true);
    }
    public void setKey_R_Up()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.RIGHT, false);
    }
    public void setKey_R_A_Down()
    {
        VirtualKeyState.lockDirection = 1;
        VirtualKeyState.SetKeyDown(VirtualKeyState.A, true);
    }
    public void setKey_R_A_Up()
    {
        VirtualKeyState.lockDirection = 0;
        VirtualKeyState.SetKeyDown(VirtualKeyState.A, false);
    }

    public void setKey_R_B_Down()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.B, true);
    }
    public void setKey_R_B_Up()
    {
        VirtualKeyState.SetKeyDown(VirtualKeyState.B, false);
    }
}
