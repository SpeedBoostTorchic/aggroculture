using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerData {

    //Used for character selection
    public static int p1Character;
    public static int p1ControlScheme;
    public static int p2Character;
    public static int p2ControlScheme;

    //Used to track number of victories during a given mutliplayer session
    public static int p1Victories;
    public static int p1Defeats;

    //Audio Manager settings
    public static float musicVol;
    public static float sfxVol;
}
